using org.apache.zookeeper;
using SolrDotNet.Cloud.Connection;
using SolrDotNet.Cloud.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolrDotNet.Cloud.ConnectionManager
{
    public record ZookeeperConnection(string Url, string ZkRoot, int ZooKeeperTimeoutMs);
    public class ZookeeperSolrCloudConnectionManager : ISolrCloudConnectionManager
    {
        private bool isDisposed;
        private readonly System.Threading.SemaphoreSlim semaphoreSlim = new System.Threading.SemaphoreSlim(1, 1);
        private readonly ZookeeperConnection _connection;
        private ZooKeeper? _zooKeeper;

        internal ZookeeperSolrCloudConnectionManager(ZookeeperConnection connection)
        {
            if (string.IsNullOrEmpty(connection.Url))
                throw new ArgumentNullException(nameof(connection.Url));
            if (connection.ZooKeeperTimeoutMs <= 0)
                throw new ArgumentOutOfRangeException(nameof(connection.ZooKeeperTimeoutMs));
            _connection = connection;
        }

        public Task ConnectAsync()
        {
            throw new NotImplementedException();
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
