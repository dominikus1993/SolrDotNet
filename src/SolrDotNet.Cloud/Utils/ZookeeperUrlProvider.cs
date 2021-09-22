using System.Runtime.CompilerServices;

namespace SolrDotNet.Cloud.Utils
{
    internal static class ZookeeperUrl
    {
        private const string ClusterState = "clusterstate.json";
        private const string CollectionState = "state.json";
        private const string CollectionsZkNode = "collections";
        private const string LiveNodesZkNode = "live_nodes";
        private const string AliasesZkNode = "aliases.json";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string GetUrlWitRootOrDefault(string? root, string url)
        {
            if (string.IsNullOrEmpty(root))
            {
                return $"/{url}";
            }
            return $"/{root}/{url}";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetLiveNodesUrlPath(string? root) => GetUrlWitRootOrDefault(root, LiveNodesZkNode);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetAliasesUrlPath(string? root) => GetUrlWitRootOrDefault(root, AliasesZkNode);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetCollectionsUrlPath(string? root) => GetUrlWitRootOrDefault(root, CollectionsZkNode);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetClusterStateUrlPath(string? root) => GetUrlWitRootOrDefault(root, ClusterState);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetCollectionsStateUrlPath() => CollectionState;
    }
}
