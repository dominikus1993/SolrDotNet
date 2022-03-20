namespace SolrDotNet.Cloud.Solr;

internal class SolrCloudRouter
{
    /// <summary>
    /// Router name
    /// </summary>
    public string? Name { get; }

    /// <summary>
    /// Explicit router
    /// </summary>
    public const string Explicit = "explicit";

    /// <summary>
    /// CompositeId router
    /// </summary>
    public const string CompositeId = "compositeId";

    /// <summary>
    /// Constructor
    /// </summary>
    public SolrCloudRouter(string? name)
    {
        Name = name;
    }
}