namespace SolrDotNet.Cloud.Utls

module internal Url =
    open System

    let length (u: UriBuilder) =
            u.Scheme.Length +
            3 +
            u.Host.Length +
            (if u.Port <= 0 then 0 else (u.Port.ToString().Length + 1)) +
            u.UserName.Length +
            (if u.UserName.Length > 0 then 1 else 0) +
            u.Password.Length +
            (if u.Password.Length > 0 then 1 else 0) +
            u.Path.Length +
            u.Query.Length +
            u.Fragment.Length

module internal Zookeeper =
    open System

    [<Literal>]
    let private ClusterState = "clusterstate.json";
    [<Literal>]
    let private CollectionState = "state.json";
    [<Literal>]
    let private CollectionsZkNode = "collections";
    [<Literal>]
    let private LiveNodesZkNode = "live_nodes";
    [<Literal>]
    let private AliasesZkNode = "aliases.json";

    let private getUrlWitRootOrDefault(root, url) =
        if String.IsNullOrEmpty(root) then
            $"/{url}"
        else
            $"/{root}/{url}";

    let getLiveNodesUrlPath(root) = getUrlWitRootOrDefault(root, LiveNodesZkNode);

    let getAliasesUrlPath(root) = getUrlWitRootOrDefault(root, AliasesZkNode);

    let getCollectionsUrlPath(root) = getUrlWitRootOrDefault(root, CollectionsZkNode);

    let getClusterStateUrlPath(root) = getUrlWitRootOrDefault(root, ClusterState);

    let getCollectionsStateUrlPath() = CollectionState;
