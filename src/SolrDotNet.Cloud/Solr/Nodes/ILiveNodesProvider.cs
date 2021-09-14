using System.Threading.Tasks;

namespace SolrDotNet.Cloud.Solr.Nodes
{
    public record LiveNode(string Name);
    public interface ILiveNodesProvider
    {
        Task<LiveNode> Provide();
    }
}
