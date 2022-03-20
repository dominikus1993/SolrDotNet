using System.Text;

using Newtonsoft.Json;

using org.apache.zookeeper;

using Serilog;

using SolrDotNet.Cloud.Auth;
using SolrDotNet.Cloud.Extensions;
using SolrDotNet.Cloud.Solr;
using SolrDotNet.Cloud.Solr.Parsers;

namespace SolrDotNet.Cloud.Zookeeper;

public class Aliases
{
    [JsonProperty("collection")]
    public Dictionary<string, string> Collection { get; set; } = new();

    public bool IsEmpty()
    {
        return Collection.Count == 0;
    }
}

internal class SolrCloudClient : Watcher, ISolrCloudStateProvider
{
    public string Key { get; private set; }

    /// <summary>
    /// Is disposed
    /// </summary>
    private bool isDisposed;

    /// <summary>
    /// Is initialized
    /// </summary>
    private bool isInitialized;

    /// <summary>
    /// Actual cloud state
    /// </summary>
    private SolrCloudState? state;

    /// <summary>
    /// Object for lock
    /// </summary>
    private readonly System.Threading.SemaphoreSlim semaphoreSlim = new(1, 1);

    /// <summary>
    /// ZooKeeper client instance
    /// </summary>
    private ZooKeeper? zooKeeper;

    private ISolrCloudAuthorization? solrCloudAuthorization;

    /// <summary>
    /// ZooKeeper connection string
    /// </summary>
    private readonly string zooKeeperConnection;

    private readonly string? zkRoot;
    private List<string>? liveNodes;

    private readonly int zooKeeperTimeoutMs;

    /// <summary>
    /// Constuctor
    /// </summary>
    public SolrCloudClient(string zooKeeperConnection, string? zkRoot = null,
        ISolrCloudAuthorization? auth = null, int zooKeeperTimeoutMs = 10_000)
    {
        if (string.IsNullOrEmpty(zooKeeperConnection))
            throw new ArgumentNullException(nameof(zooKeeperConnection));

        this.zooKeeperConnection = zooKeeperConnection;
        this.zkRoot = zkRoot;
        this.zooKeeperTimeoutMs = zooKeeperTimeoutMs;
        Key = zooKeeperConnection;
        solrCloudAuthorization = auth;
    }

    /// <summary>
    /// Initialize cloud state
    /// </summary>
    public async Task InitAsync()
    {
        if (isInitialized)
        {
            return;
        }

        await semaphoreSlim.WaitAsync().ConfigureAwait(false);
        try
        {
            if (!isInitialized)
            {
                await UpdateAsync().ConfigureAwait(false);

                isInitialized = true;
            }

            if (state is null || !state.HasAnyCollections())
            {
                throw new ApplicationException("Can't download any collection or alias from zookeeper");
            }
        }
        catch (Exception ex)
        {
            Log.Logger.Fatal(ex, "Error when trying connect to zookeeper");
            throw;
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }

    /// <summary>
    /// Get cloud state
    /// </summary>
    /// <returns>Solr Cloud State</returns>
    public SolrCloudState? GetCloudState()
    {
        return state;
    }

    /// <summary>
    /// Reinitialize connection and get fresh cloud state.
    /// Not included in ISolrCloudStateProvider interface due to the testing purpose only 
    /// (causes reloading all cloud data and too slow to use in production)
    /// </summary>
    /// <returns>Solr Cloud State</returns>
    public async Task<SolrCloudState?> GetFreshCloudStateAsync()
    {
        await SynchronizedUpdateAsync(cleanZookeeperConnection: true).ConfigureAwait(false);
        return GetCloudState();
    }

    public void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        if (!isDisposed)
        {
            if (zooKeeper != null)
            {
                zooKeeper.closeAsync().Wait();
            }

            isDisposed = true;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (!isDisposed && zooKeeper != null)
        {
            await zooKeeper.closeAsync().ConfigureAwait(false);
            isDisposed = true;
        }
    }

    /// <summary>
    /// Watcher for zookeeper events
    /// </summary>
    /// <param name="event">zookeeper event</param>
    public override async Task process(WatchedEvent @event)
    {
        if (@event is null)
        {
            throw new ArgumentNullException(nameof(@event));
        }

        if (@event.get_Type() != Event.EventType.None && !string.IsNullOrEmpty(@event.getPath()))
        {
            await SynchronizedUpdateAsync().ConfigureAwait(false);
        }
        else if (@event.get_Type() == Event.EventType.None && @event.getState() == Event.KeeperState.Disconnected)
        {
            await SynchronizedUpdateAsync(cleanZookeeperConnection: true).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Synchronized updates of zookeeper connection and actual cloud state
    /// </summary>
    /// <param name="cleanZookeeperConnection">clean zookeeper connection and create new one</param>
    private async Task SynchronizedUpdateAsync(bool cleanZookeeperConnection = false)
    {
        await semaphoreSlim.WaitAsync().ConfigureAwait(false);
        try
        {
            await UpdateAsync(cleanZookeeperConnection).ConfigureAwait(false);
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }

    /// <summary>
    /// Updates zookeeper connection and actual cloud state
    /// </summary>
    /// <param name="cleanZookeeperConnection">clean zookeeper connection and create new one</param>
    private async Task UpdateAsync(bool cleanZookeeperConnection = false)
    {
        if (zooKeeper == null || cleanZookeeperConnection)
        {
            if (zooKeeper != null)
            {
                await zooKeeper.closeAsync().ConfigureAwait(false);
            }

            zooKeeper = new ZooKeeper(zooKeeperConnection, zooKeeperTimeoutMs, this);

            if (solrCloudAuthorization is not null)
            {
                zooKeeper.addAuthInfo(solrCloudAuthorization.Name, solrCloudAuthorization.GetAuthData().ToArray());
            }
        }

        liveNodes = await GetLiveNodesAsync().ConfigureAwait(false);

        if (liveNodes.Count > 0)
        {
            state = (await GetInternalCollectionsStateAsync().ConfigureAwait(false)).Merge(
                await GetExternalCollectionsStateAsync().ConfigureAwait(false));
        }
        else
        {
            Log.Logger.Warning("No live nodes");
            state = SolrCloudState.Zero();
        }
    }

    /// <summary>
    /// Get Live nodes from zookeeper
    /// </summary>
    private async Task<List<string>> GetLiveNodesAsync()
    {
        try
        {
            var liveNodesChildren = await zooKeeper
                .getChildrenAsync(ZookeeperUrlProvider.GetLiveNodesUrlPath(this.zkRoot), this).ConfigureAwait(false);
            return new List<string>(liveNodesChildren.Children);
        }
        catch (KeeperException ex)
        {
            Log.Logger.Warning(ex, "Can't download live nodes");
            return new List<string>();
        }
    }

    /// <summary>
    /// Returns parsed internal collections cloud state
    /// </summary>
    private async Task<SolrCloudState> GetInternalCollectionsStateAsync()
    {
        DataResult data;

        try
        {
            data = await zooKeeper.getDataAsync(ZookeeperUrlProvider.GetClusterStateUrlPath(this.zkRoot), true)
                .ConfigureAwait(false);
        }
        catch (KeeperException)
        {
            return SolrCloudState.Zero();
        }

        return
            data != null
                ? SolrCloudStateParser.Parse(Encoding.Default.GetString(data.Data), liveNodes)
                : SolrCloudState.Zero();
    }

    private async Task<SolrCloudState> GetAliases()
    {
        var resultState = SolrCloudState.Zero();
        try
        {
            var aliasesJson = await zooKeeper.getDataAsync(ZookeeperUrlProvider.GetAliasesUrlPath(this.zkRoot), true)
                .ConfigureAwait(false);
            if (aliasesJson?.Data is null)
            {
                return resultState;
            }

            var aliases = JsonConvert.DeserializeObject<Aliases>(Encoding.UTF8.GetString(aliasesJson.Data)) ??
                          new Aliases();
            if (!aliases.IsEmpty())
            {
                foreach (var alias in aliases.Collection)
                {
                    resultState = resultState.Add(new SolrCloudAlias(alias.Key, liveNodes));
                }
            }

            return resultState;
        }
        catch (Exception ex)
        {
            Log.Logger.Warning(ex, "Error when trying download aliases");
            return resultState;
        }
    }

    private async Task<SolrCloudState> GetCollections()
    {
        var resultState = SolrCloudState.Zero();
        ChildrenResult children;
        try
        {
            children = await zooKeeper.getChildrenAsync(ZookeeperUrlProvider.GetCollectionsUrlPath(this.zkRoot), true)
                .ConfigureAwait(false);
        }
        catch (KeeperException ex)
        {
            Log.Logger.Warning(ex, "Error when trying download collections");
            return resultState;
        }

        if (children == null || children.Children.Count == 0)
            return resultState;

        foreach (var child in children.Children)
        {
            DataResult? data;

            try
            {
                data = await zooKeeper.getDataAsync(GetCollectionPath(child, this.zkRoot), true).ConfigureAwait(false);
            }
            catch (KeeperException ex)
            {
                Log.Logger.Warning(ex, "Error when trying download data for collection: {Child}", child);
                data = null;
            }

            var collectionState =
                data != null
                    ? SolrCloudStateParser.Parse(Encoding.Default.GetString(data.Data), liveNodes)
                    : SolrCloudState.Zero();
            resultState = resultState.Merge(collectionState);
        }

        return resultState;
    }

    /// <summary>
    /// Returns parsed external collections cloud state
    /// </summary>
    private async Task<SolrCloudState> GetExternalCollectionsStateAsync()
    {
        var resultState = SolrCloudState.Zero();
        var (alias, coll) = await TaskExt.WhenAll(GetAliases(), GetCollections());
        return resultState.Merge(alias).Merge(coll);
    }

    /// <summary>
    /// Returns path to collection
    /// </summary>
    private static string GetCollectionPath(string collectionName, string? zkRoot) =>
        $"{ZookeeperUrlProvider.GetCollectionsUrlPath(zkRoot)}/{collectionName}/{ZookeeperUrlProvider.GetCollectionsStateUrlPath()}";
    
}