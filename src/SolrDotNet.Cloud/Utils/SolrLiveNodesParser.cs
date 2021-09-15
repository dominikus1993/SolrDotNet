using SolrDotNet.Cloud.Solr.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("SolrDotNet.Cloud.Tests")]
namespace SolrDotNet.Cloud.Utils
{
    internal static class SolrLiveNodesParser
    {
        public static IEnumerable<SolrLiveNode> Parse(IList<string>? nodes)
        {
            if (nodes is null || nodes.Count == 0)
            {
                throw new Exception("No live node");
            }

            return nodes
                    .Select(node => node.Split("_"))
                    .Where(addresAndSuffix => addresAndSuffix.Length == 2 && addresAndSuffix[1] == "solr")
                    .Select(addresAndSuffix => new SolrLiveNode($"http://{addresAndSuffix[0]}/solr"));
        }
    }
}
