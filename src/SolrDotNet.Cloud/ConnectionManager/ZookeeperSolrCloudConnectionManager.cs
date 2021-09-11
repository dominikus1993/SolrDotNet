using SolrDotNet.Cloud.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolrDotNet.Cloud.ConnectionManager
{
    public class ZookeeperSolrCloudConnectionManager : ISolrCloudConnectionManager
    {
        private const string ClusterState = "/clusterstate.json";
        private const string CollectionState = "state.json";
        private const string CollectionsZkNode = "/collections";
        private const string LiveNodesZkNode = "/live_nodes";

        public ZookeeperSolrCloudConnectionManager(string zooKeeperConnection, int zooKeeperTimeoutMs = 10_000)
        {
            if (string.IsNullOrEmpty(zooKeeperConnection))
                throw new ArgumentNullException(nameof(zooKeeperConnection));
            if (zooKeeperTimeoutMs <= 0)
                throw new ArgumentOutOfRangeException(nameof(zooKeeperTimeoutMs));
        }

        public Task ConnectAsync()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public ValueTask DisposeAsync()
        {
            throw new NotImplementedException();
        }

        public ISolrCloudConnection GetCloudConnection()
        {
            throw new NotImplementedException();
        }
    }
}
