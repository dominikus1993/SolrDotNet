namespace SolrDotNet.Cloud.Solr;

internal class SolrCloudShard
{
    /// <summary>
    /// Is active
    /// </summary>
    public bool IsActive { get; }

    /// <summary>
    /// Shard name
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Shard range end
    /// </summary>
    public int? RangeEnd { get; }

    /// <summary>
    /// Shard range start
    /// </summary>
    public int? RangeStart { get; }

    /// <summary>
    /// Shard replicas
    /// </summary>
    public IReadOnlyDictionary<string, SolrCloudReplica> Replicas { get; }

    /// <summary>
    /// Constructor
    /// </summary>
    public SolrCloudShard(bool isActive, string name, int? rangeEnd, int? rangeStart,
        IReadOnlyDictionary<string, SolrCloudReplica>? replicas)
    {
        ArgumentNullException.ThrowIfNull(replicas, nameof(replicas));
        IsActive = isActive;
        Name = name;
        RangeEnd = rangeEnd;
        RangeStart = rangeStart;
        Replicas = replicas;
    }
}