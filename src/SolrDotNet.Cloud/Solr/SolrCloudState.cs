using System.Diagnostics.Contracts;

namespace SolrDotNet.Cloud.Solr;

/// <summary>
/// Represents cloud state
/// </summary>
public class SolrCloudState
{
    private readonly Random _random;

    /// <summary>
    /// State collections
    /// </summary>
    private readonly IReadOnlyDictionary<string, ISolrCloudCollection> Collections;

    /// <summary>
    /// Constructor
    /// </summary>
    public SolrCloudState(IReadOnlyDictionary<string, ISolrCloudCollection> collections)
    {
        Collections = collections;
        _random = new Random();
    }

    public bool HasAnyCollections() => Collections.Count != 0;


    public bool ContainsCollection(string collectionName) => Collections.ContainsKey(collectionName);

    public ISolrCloudCollection GetCollection(string? collectionName)
    {
        if (!HasAnyCollections())
        {
            throw new ApplicationException("Didn't get any collection's state from zookeeper.");
        }

        if (collectionName != null && !Collections.ContainsKey(collectionName))
        {
            throw new ApplicationException($"Didn't get '{collectionName}' collection state from zookeeper.");
        }

        return collectionName == null
            ? Collections.Values.First()
            : Collections[collectionName];
    }

    [Pure]
    public SolrCloudState Add(ISolrCloudCollection collection)
    {
        var r = Collections.ToDictionary(kv => kv.Key, kv => kv.Value);
        r.Add(collection.Name, collection);
        return new SolrCloudState(r);
    }

    /// <summary>
    /// Returns merged cloud states
    /// </summary>
    [Pure]
    public SolrCloudState Merge(SolrCloudState state)
    {
        if (state == null || state.Collections == null || state.Collections.Count == 0)
            return this;

        var r = Collections.ToDictionary(kv => kv.Key, kv => kv.Value);
        foreach (var element in state.Collections)
        {
            r.Add(element.Key, element.Value);
        }

        return new SolrCloudState(r);
    }

    public static SolrCloudState Zero()
    {
        return new SolrCloudState(new Dictionary<string, ISolrCloudCollection>());
    }
}