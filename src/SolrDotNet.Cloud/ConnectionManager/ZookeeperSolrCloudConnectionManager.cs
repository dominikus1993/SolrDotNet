using Microsoft.Extensions.Logging;
using org.apache.zookeeper;
using SolrDotNet.Cloud.Auth;
using SolrDotNet.Cloud.Connection;
using SolrDotNet.Cloud.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolrDotNet.Cloud.ConnectionManager
{
    public record ZookeeperConnection(string Url, string? ZkRoot = null, int ZooKeeperTimeoutMs = 10_000, ISolrCloudAuthorization? Auth = null);
    public class ZookeeperSolrCloudConnectionManager : ISolrCloudConnectionManager
    {
        private bool isDisposed;
        private readonly System.Threading.SemaphoreSlim semaphoreSlim = new System.Threading.SemaphoreSlim(1, 1);
        private readonly ZookeeperConnection _connection;
        private ZooKeeper? _zooKeeper;
        private bool _isInitialized;

        internal ZookeeperSolrCloudConnectionManager(ZookeeperConnection connection, ILogger<ZookeeperSolrCloudConnectionManager> logger)
        {
            if (string.IsNullOrEmpty(connection.Url))
                throw new ArgumentNullException(nameof(connection.Url));
            if (connection.ZooKeeperTimeoutMs <= 0)
                throw new ArgumentOutOfRangeException(nameof(connection.ZooKeeperTimeoutMs));
            _connection = connection;
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
                Log.Logger.Fatal(ex, "Error when trying connect to zookeeper");
                throw;
            }
            finally
            {
                semaphoreSlim.Release();
            }
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
