using System.Runtime.CompilerServices;
using System.Text;

using Microsoft.Extensions.Logging;

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsEmpty()
    {
        return Collection.Count == 0;
    }
}

public sealed class SolrCloudClient : Watcher, ISolrCloudStateProvider
{
    public string Key { get; private set; }

    /// <summary>
    /// Is disposed
    /// </summary>
    private bool _isDisposed;

    /// <summary>
    /// Is initialized
    /// </summary>
    private bool _isInitialized;

    /// <summary>
    /// Actual cloud state
    /// </summary>
    private SolrCloudState? _state;

    /// <summary>
    /// Object for lock
    /// </summary>
    private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

    /// <summary>
    /// ZooKeeper client instance
    /// </summary>
    private ZooKeeper? _zooKeeper;

    private readonly ISolrCloudAuthorization? _solrCloudAuthorization;

    /// <summary>
    /// ZooKeeper connection string
    /// </summary>
    private readonly string _zooKeeperConnection;
    private readonly string? _zkRoot;
    private List<string>? _liveNodes;
    private readonly int _zooKeeperTimeoutMs;

    /// <summary>
    /// Constuctor
    /// </summary>
    public SolrCloudClient(string zooKeeperConnection, string? zkRoot = null,
        ISolrCloudAuthorization? auth = null, int zooKeeperTimeoutMs = 10_000)
    {
        if (string.IsNullOrEmpty(zooKeeperConnection))
            throw new ArgumentNullException(nameof(zooKeeperConnection));

        this._zooKeeperConnection = zooKeeperConnection;
        this._zkRoot = zkRoot;
        this._zooKeeperTimeoutMs = zooKeeperTimeoutMs;
        Key = zooKeeperConnection;
        _solrCloudAuthorization = auth;
    }

    /// <summary>
    /// Initialize cloud state
    /// </summary>
    public async Task InitAsync()
    {
        if (_isInitialized)
        {
            return;
        }

        await _semaphoreSlim.WaitAsync().ConfigureAwait(false);
        try
        {
            if (!_isInitialized)
            {
                await UpdateAsync().ConfigureAwait(false);

                _isInitialized = true;
            }

            if (_state is null || !_state.HasAnyCollections())
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
            _semaphoreSlim.Release();
        }
    }

    /// <summary>
    /// Get cloud state
    /// </summary>
    /// <returns>Solr Cloud State</returns>
    public SolrCloudState? GetCloudState()
    {
        return _state;
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
        if (_isDisposed)
        {
            return;
        }

        if (!_isDisposed)
        {
            if (_zooKeeper is not null)
            {
                _zooKeeper.closeAsync().Wait();
            }

            _isDisposed = true;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (!_isDisposed && _zooKeeper is not null)
        {
            await _zooKeeper.closeAsync().ConfigureAwait(false);
            _isDisposed = true;
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
        await _semaphoreSlim.WaitAsync().ConfigureAwait(false);
        try
        {
            await UpdateAsync(cleanZookeeperConnection).ConfigureAwait(false);
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    /// <summary>
    /// Updates zookeeper connection and actual cloud state
    /// </summary>
    /// <param name="cleanZookeeperConnection">clean zookeeper connection and create new one</param>
    private async Task UpdateAsync(bool cleanZookeeperConnection = false)
    {
        if (_zooKeeper == null || cleanZookeeperConnection)
        {
            if (_zooKeeper != null)
            {
                await _zooKeeper.closeAsync().ConfigureAwait(false);
            }

            _zooKeeper = new ZooKeeper(_zooKeeperConnection, _zooKeeperTimeoutMs, this);

            if (_solrCloudAuthorization is not null)
            {
                _zooKeeper.addAuthInfo(_solrCloudAuthorization.Name, _solrCloudAuthorization.GetAuthData().ToArray());
            }
        }

        _liveNodes = await GetLiveNodesAsync().ConfigureAwait(false);

        if (_liveNodes.Count > 0)
        {
            _state = (await GetInternalCollectionsStateAsync().ConfigureAwait(false)).Merge(
                await GetExternalCollectionsStateAsync().ConfigureAwait(false));
        }
        else
        {
            Log.Logger.Warning("No live nodes");
            _state = SolrCloudState.Zero();
        }
    }

    /// <summary>
    /// Get Live nodes from zookeeper
    /// </summary>
    private async Task<List<string>> GetLiveNodesAsync()
    {
        try
        {
            var liveNodesChildren = await _zooKeeper
                !.getChildrenAsync(ZookeeperUrlProvider.GetLiveNodesUrlPath(this._zkRoot), this).ConfigureAwait(false);
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
            data = await _zooKeeper!.getDataAsync(ZookeeperUrlProvider.GetClusterStateUrlPath(this._zkRoot), true)
                .ConfigureAwait(false);
        }
        catch (KeeperException)
        {
            return SolrCloudState.Zero();
        }

        return
            data != null
                ? SolrCloudStateParser.Parse(Encoding.Default.GetString(data.Data), _liveNodes)
                : SolrCloudState.Zero();
    }

    private async Task<SolrCloudState> GetAliases()
    {
        var resultState = SolrCloudState.Zero();
        try
        {
            var aliasesJson = await _zooKeeper!.getDataAsync(ZookeeperUrlProvider.GetAliasesUrlPath(this._zkRoot), true)
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
                    resultState = resultState.Add(new SolrCloudAlias(alias.Key, _liveNodes));
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
            children = await _zooKeeper!.getChildrenAsync(ZookeeperUrlProvider.GetCollectionsUrlPath(this._zkRoot), true)
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
                data = await _zooKeeper.getDataAsync(GetCollectionPath(child, this._zkRoot), true).ConfigureAwait(false);
            }
            catch (KeeperException ex)
            {
                Log.Logger.Warning(ex, "Error when trying download data for collection: {Child}", child);
                data = null;
            }

            var collectionState =
                data != null
                    ? SolrCloudStateParser.Parse(Encoding.Default.GetString(data.Data), _liveNodes)
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
    [MethodImpl( MethodImplOptions.AggressiveInlining)]
    private static string GetCollectionPath(string collectionName, string? zkRoot) =>
        $"{ZookeeperUrlProvider.GetCollectionsUrlPath(zkRoot)}/{collectionName}/{ZookeeperUrlProvider.GetCollectionsStateUrlPath()}";
    
}