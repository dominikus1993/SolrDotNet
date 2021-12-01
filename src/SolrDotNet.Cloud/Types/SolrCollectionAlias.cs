using SolrDotNet.Cloud.Solr.Nodes;
using SolrDotNet.Cloud.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolrDotNet.Cloud.Types
{
    internal class SolrCollectionAlias : ISolrCollection
    {
        public string Name { get; }
        private readonly List<string> _urls;
        private readonly Random random = new();

        public SolrCollectionAlias(string name, List<string>? liveNodes)
        {
            Name = name;
            _urls = SolrLiveNodesParser.Parse(liveNodes).Select(node => node.GetAliasUrl(name)).ToList();
        }

        public string? GetUrl()
        {
            return _urls.Count switch
            {
                0 => throw new Exception("No Alive Node"),
                1 => _urls[0],
                int count => _urls[random.Next(0, count)],
                _ => throw new NotImplementedException(),
            };
        }
    }
}
