namespace SolrDotNet.Cloud.Types.Solr;

internal class SolrCloudReplica
{
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


    public SolrCloudReplica(bool isActive, bool isLeader, string name, string url)
    {
        IsActive = isActive;
        IsLeader = isLeader;
        Name = name;
        Url = url;
    }
}
