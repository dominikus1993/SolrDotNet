using SolrDotNet.Cloud.SolrRouter;

namespace SolrDotNet.Cloud.Connection;

public interface ISolrCloudConnection
{
    ISolrCloudRouter GetRouter();
}
