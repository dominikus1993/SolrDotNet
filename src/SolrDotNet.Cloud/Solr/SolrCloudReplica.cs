namespace SolrDotNet.Cloud.Solr;

public class SolrCloudReplica
{
    /// <summary>
    /// Is active
    /// </summary>
    public bool IsActive { get; }

    /// <summary>
    /// Is leader
    /// </summary>
    public bool IsLeader { get; }

    /// <summary>
    /// Replica name
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Replica url
    /// </summary>
    public string Url { get; }

    /// <summary>
    /// Constructor
    /// </summary>
    public SolrCloudReplica(bool isActive, bool isLeader, string name, string url)
    {
        IsActive = isActive;
        IsLeader = isLeader;
        Name = name;
        Url = url;
    }
}