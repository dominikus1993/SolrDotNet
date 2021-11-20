namespace SolrDotNet.Cloud.Auth;

public interface ISolrCloudAuthorization
{
    string Name { get; }
    ReadOnlyMemory<byte> GetAuthData();
}

