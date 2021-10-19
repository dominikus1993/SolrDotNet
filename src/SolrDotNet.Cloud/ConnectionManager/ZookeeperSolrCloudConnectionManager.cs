using Microsoft.Extensions.Logging;
using org.apache.zookeeper;
using SolrDotNet.Cloud.Auth;
using SolrDotNet.Cloud.Connection;
using SolrDotNet.Cloud.Solr.Nodes;
using SolrDotNet.Cloud.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolrDotNet.Cloud.ConnectionManager
{
    public record ZookeeperConnection(string Url, string? ZkRoot = null, int ZooKeeperTimeoutMs = 10_000, ISolrCloudAuthorization? Auth = null);
    public class ZookeeperSolrCloudConnectionManager : Watcher, ISolrCloudConnectionManager
    {
        private bool isDisposed;
        private readonly System.Threading.SemaphoreSlim semaphoreSlim = new System.Threading.SemaphoreSlim(1, 1);
        private readonly ZookeeperConnection _connection;
        private ZooKeeper? _zooKeeper;
        private bool _isInitialized;
        private ILogger<ZookeeperSolrCloudConnectionManager> _logger;
        private List<SolrLiveNode> _liveNodes = new();

        internal ZookeeperSolrCloudConnectionManager(ZookeeperConnection connection, ILogger<ZookeeperSolrCloudConnectionManager> logger)
        {
            if (string.IsNullOrEmpty(connection.Url))
                throw new ArgumentNullException(nameof(connection.Url));
            if (connection.ZooKeeperTimeoutMs <= 0)
                throw new ArgumentOutOfRangeException(nameof(connection.ZooKeeperTimeoutMs));
            _connection = connection;
            _logger = logger;
        }

        public async Task ConnectAsync()
        {
            if (_isInitialized)
            {
                return;
            }

            await semaphoreSlim.WaitAsync().ConfigureAwait(false);
            try
            {
                if (!_isInitialized)
                {
                    await UpdateAsync().ConfigureAwait(false);

                    _isInitialized = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Error when trying connect to zookeeper");
                throw;
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        public override async Task process(WatchedEvent evt)
        {
            if (evt.get_Type() != Event.EventType.None && !string.IsNullOrEmpty(evt.getPath()))
            {
                await SynchronizedUpdateAsync().ConfigureAwait(false);
            }
            else if (evt.get_Type() == Event.EventType.None && evt.getState() == Event.KeeperState.Disconnected)
            {
                await SynchronizedUpdateAsync(cleanZookeeperConnection: true).ConfigureAwait(false);
            }
        }

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

        private async Task UpdateAsync(bool cleanZookeeperConnection = false)
        {
            if (_zooKeeper == null || cleanZookeeperConnection)
            {
                if (_zooKeeper != null)
                {
                    await _zooKeeper.closeAsync().ConfigureAwait(false);
                }
                _zooKeeper = new ZooKeeper(_connection.Url, _connection.ZooKeeperTimeoutMs, this);

                if (_connection.Auth is not null)
                {
                    _zooKeeper.addAuthInfo(_connection.Auth.Name, _connection.Auth.GetAuthData().ToArray());
                }

            }

            _liveNodes = await GetLiveNodesAsync().ConfigureAwait(false);

            if (_liveNodes.Count > 0)
            {
               // TODO xD
            }
            else
            {
                _logger.LogWarning("No live nodes");
            }
        }

        private async Task<List<SolrLiveNode>> GetLiveNodesAsync()
        {
            try
            {
                if (_zooKeeper is not null)
                {
                    var liveNodesChildren = await _zooKeeper.getChildrenAsync(ZookeeperUrl.GetLiveNodesUrlPath(_connection.ZkRoot), this).ConfigureAwait(false);
                    return SolrLiveNodesParser.Parse(liveNodesChildren.Children).ToList();
                }
            }
            catch (KeeperException ex)
            {
                _logger.LogWarning(ex, "Can't download live nodes");             
            }
            return new List<SolrLiveNode>();
        }

        public void Dispose()
        {
            if (isDisposed)
            {
                return;
            }

            if (!isDisposed)
            {
                if (_zooKeeper is not null)
                {
                    AsyncHelper.RunSync(() => _zooKeeper.closeAsync());
                }
                isDisposed = true;
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (isDisposed)
            {
                return;
            }

            if (!isDisposed)
            {
                if (_zooKeeper is not null)
                {
                    await _zooKeeper.closeAsync().ConfigureAwait(false);
                }
                isDisposed = true;
            }
        }

        public ISolrCloudConnection GetCloudConnection()
        {
            throw new NotImplementedException();
        }

        public ValueTask<bool> IsConnected()
        {
            throw new NotImplementedException();
        }
    }
}
