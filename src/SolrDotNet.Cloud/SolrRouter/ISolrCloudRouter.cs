using SolrDotNet.Cloud.Types;

namespace SolrDotNet.Cloud.SolrRouter
{
    public interface ISolrCloudRouter
    {
        ISolrCollection GetCollection(string name);
    }
}
