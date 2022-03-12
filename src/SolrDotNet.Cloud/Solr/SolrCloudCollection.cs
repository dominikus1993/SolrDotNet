namespace SolrDotNet.Cloud.Solr;


/// <summary>
/// Represents cloud collection
/// </summary>
public class SolrCloudCollection
{
    /// <summary>
    /// Collection name
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Collection router type
    /// </summary>
    public SolrCloudRouter Router { get; }

    /// <summary>
    /// Collection shards
    /// </summary>
    public IReadOnlyDictionary<string, SolrCloudShard> Shards { get; }

    /// <summary>
    /// Constructor
    /// </summary>
    public SolrCloudCollection(string name, SolrCloudRouter? router, IReadOnlyDictionary<string, SolrCloudShard>? shards)
    {
        ArgumentNullException.ThrowIfNull(router, nameof(router));
        ArgumentNullException.ThrowIfNull(shards, nameof(shards));
        Name = name;
        Router = router;
        Shards = shards;
    }
}