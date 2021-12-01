using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolrDotNet.Cloud.Types.Solr;

namespace SolrDotNet.Cloud.Types
{
    internal class SolrCollection : ISolrCollection
    {
        public string Name { get; }

        private readonly IReadOnlyDictionary<string, SolrCloudShard>? _shards;
        private readonly List<SolrCloudReplica> _activeReplicas;
        private readonly Random random = new();

        public SolrCollection(string name, IReadOnlyDictionary<string, SolrCloudShard>? shards)
        {
            Name = name;
            _shards = shards;
            _activeReplicas = shards.Values
                                .Where(shard => shard.IsActive)
                                .SelectMany(shard => shard.Replicas?.Values ?? Enumerable.Empty<SolrCloudReplica>())
                                .Where(shard => shard.IsActive)
                                .ToList();

        }
        public string GetUrl()
        {
            var replica = _activeReplicas.Count switch
            {
                0 => throw new Exception("No Alive Replica"),
                1 => _activeReplicas[0],
                int count => _activeReplicas[random.Next(0, count)],
            };

            return replica.Url;
        }
    }
}
