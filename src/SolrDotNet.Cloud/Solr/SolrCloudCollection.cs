using SolrDotNet.Cloud.Exceptions;
using SolrDotNet.Cloud.Extensions;

namespace SolrDotNet.Cloud.Solr;

public interface ISolrCloudCollection
{
    Uri? GetUrl(bool leader);
    string Name { get; }
}

internal class SolrCloudAlias : ISolrCloudCollection
{
    public string Name { get; }
    private readonly IReadOnlyList<SolrLiveNode> _nodes;
    private readonly Random random = new();

    public SolrCloudAlias(string name, IList<string>? liveNodes)
    {
        Name = name;
        _nodes = SolrLiveNodesParser.ParseAlias(liveNodes, name).ToList();
    }

    public Uri? GetUrl(bool leader)
    {
        var node = _nodes.Count switch
        {
            0 => throw new Exception("No Alive Node"),
            1 => _nodes[0],
            int count => _nodes[random.Next(0, count)]
        };
        return node.Url;
    }
}

/// <summary>
/// Represents cloud collection
/// </summary>
internal class SolrCloudCollection : ISolrCloudCollection
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

    private readonly List<SolrCloudReplica> _activeReplicas;


    /// <summary>
    /// Constructor
    /// </summary>
    public SolrCloudCollection(string name, SolrCloudRouter? router,
        IReadOnlyDictionary<string, SolrCloudShard>? shards)
    {
        ArgumentNullException.ThrowIfNull(router, nameof(router));
        ArgumentNullException.ThrowIfNull(shards, nameof(shards));
        Name = name;
        Router = router;
        Shards = shards;
        _activeReplicas = shards.Values
            .Where(shard => shard.IsActive)
            .SelectMany(shard => shard.Replicas?.Values ?? Enumerable.Empty<SolrCloudReplica>())
            .Where(shard => shard.IsActive)
            .ToList();
    }

    public Uri? GetUrl(bool leader)
    {
        // TODO Optimize this shit !!
        var replica = _activeReplicas
            .Where(replica => (!leader || replica.IsLeader))
            .OrderBy(_ => Guid.NewGuid())
            .FirstOrDefault();

        if (replica is null)
        {
            throw new NoAppropriateNodeWasSelectedException();
        }

        return replica.Url;
    }
}