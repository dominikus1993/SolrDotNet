using System;
using System.Threading.Tasks;

namespace SolrDotNet.Cloud.Solr.Nodes
{
    public record SolrLiveNode(string Name, string Url)
    {
        public string GetAliasUrl(string alias)
        {
            if (string.IsNullOrEmpty(alias))
            {
                throw new ArgumentNullException(nameof(alias));
            }
            return $"{Url}/{alias}";
        }
    }

    public interface ILiveNodesProvider
    {
        Task<SolrLiveNode> Provide();
    }
}
