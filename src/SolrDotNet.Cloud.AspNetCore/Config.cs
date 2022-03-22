using SolrDotNet.Cloud.Auth;

namespace SolrDotNet.Cloud.AspNetCore;

public class SolrCloudAuth
{
    public string? Username { get; set; }
    public string? Password { get; set; }

    internal ISolrCloudAuthorization ToAuth()
    {
        if (string.IsNullOrEmpty(Username))
        {
            throw new ArgumentNullException(nameof(Username));
        }
        if (string.IsNullOrEmpty(Password))
        {
            throw new ArgumentNullException(nameof(Password));
        }
        return new DigestSolrCloudAuthorization(Username, Password);
    }
}

public class SolrCloudConfig
{
    public SolrCloudAuth? Auth { get; set; }

    public string? Connection { get; set; }

    public string? ZkRoot { get; set; } = "solr";
}