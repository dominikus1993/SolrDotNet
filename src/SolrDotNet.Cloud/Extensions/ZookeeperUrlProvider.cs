using System.Runtime.CompilerServices;

namespace SolrDotNet.Cloud.Extensions;

internal static class ZookeeperUrlProvider
{
    private const string ClusterState = "clusterstate.json";
    private const string CollectionState = "state.json";
    private const string CollectionsZkNode = "collections";
    private const string LiveNodesZkNode = "live_nodes";
    private const string AliasesZkNode = "aliases.json";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetLiveNodesUrlPath(string? root) => string.IsNullOrEmpty(root) ? $"/{LiveNodesZkNode}" : $"/{root}/{LiveNodesZkNode}";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetAliasesUrlPath(string? root) => string.IsNullOrEmpty(root) ? $"/{AliasesZkNode}" : $"/{root}/{AliasesZkNode}";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetCollectionsUrlPath(string? root) => string.IsNullOrEmpty(root) ? $"/{CollectionsZkNode}" : $"/{root}/{CollectionsZkNode}";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetClusterStateUrlPath(string? root) => string.IsNullOrEmpty(root) ? $"/{ClusterState}" : $"/{root}/{ClusterState}";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetCollectionsStateUrlPath() => CollectionState;
}