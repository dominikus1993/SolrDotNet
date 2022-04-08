using System.Runtime.CompilerServices;

using SolrDotNet.Cloud.Exceptions;

[assembly: InternalsVisibleTo("SolrDotNet.Tests")]

namespace SolrDotNet.Cloud.Extensions;

internal record SolrLiveNode(Uri Url);

internal static class SolrLiveNodesParser
{
    public static IEnumerable<SolrLiveNode> ParseAlias(IList<string>? nodes, string? alias)
    {
        if (nodes is null || nodes.Count == 0)
        {
            throw new NoAliveNodeException();
        }

        if (string.IsNullOrEmpty(alias))
        {
            throw new ArgumentNullException(nameof(alias));
        }

        foreach (var node in nodes)
        {
            var addressAndSuffix = node.Split("_");
            if (addressAndSuffix.Length == 2 && addressAndSuffix[1] == "solr")
            {
                yield return new SolrLiveNode(new Uri($"http://{addressAndSuffix[0]}/solr/{alias}"));
            }
        }
    }
}