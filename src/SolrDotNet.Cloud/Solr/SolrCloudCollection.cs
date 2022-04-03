using System.Runtime.CompilerServices;

using SolrDotNet.Cloud.Exceptions;
using SolrDotNet.Cloud.Extensions;

[assembly: InternalsVisibleTo("SolrDotNet.Tests")]
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

    private readonly SolrCloudReplica[] _activeReplicas;
    private readonly SolrCloudReplica[] _activeOnlyLeaderReplica;
    private readonly Random _random = new();


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
            .ToArray();
        _activeOnlyLeaderReplica = _activeReplicas.Where(replica => replica.IsLeader).ToArray();
    }

    public Uri? GetUrl(bool leader)
    {

        var replicaUri = leader ? GetUrl(_activeOnlyLeaderReplica) : GetUrl(_activeReplicas);

        if (replicaUri is null)
        {
            throw new NoAppropriateNodeWasSelectedException();
        }
        return replicaUri;
    }
    
    private Uri? GetUrl(SolrCloudReplica[] replicas)
    {
        var length = replicas.Length;
        if (length == 0)
        {
            return null;
        }

        if (length== 1)
        {
            return replicas[0].Url;
        }

        var replica = replicas[_random.Next(0, length)];

        return replica.Url;
    }
}