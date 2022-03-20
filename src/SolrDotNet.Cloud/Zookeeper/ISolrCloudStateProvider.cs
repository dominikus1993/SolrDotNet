using SolrDotNet.Cloud.Solr;

namespace SolrDotNet.Cloud.Zookeeper;

/// <summary>
/// Solr cloud state provider interface
/// </summary>
public interface ISolrCloudStateProvider : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Provider key
    /// </summary>
    string Key { get; }

    /// <summary>
    /// Returns actual solr cloud state
    /// </summary>
    SolrCloudState? GetCloudState();

    /// <summary>
    /// Provider initialization
    /// </summary>
    Task InitAsync();
}