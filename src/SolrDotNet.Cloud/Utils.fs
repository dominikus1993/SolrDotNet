namespace SolrDotNet.Cloud.Utils

open System.Linq

module internal List =
    let toList(s: _ list) = s.ToList()

module internal Enumerable =
    let toList(s: _ seq) = s.ToList()

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

module internal TaskHelper =
    open System.Threading.Tasks
    open System.Threading
    let taskFactory = TaskFactory(CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default)

    let runSyncT<'T>(f: unit -> Task<'T>) = taskFactory.StartNew(f).Unwrap().GetAwaiter().GetResult()

    let runSync(f: unit -> Task) = taskFactory.StartNew(f).Unwrap().GetAwaiter().GetResult()

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

    let getCollectionPath(root)(collectionName) = $"{getCollectionsUrlPath(root)}/{collectionName}/{CollectionState}";

    let getCollectionsStateUrlPath() = CollectionState;

module internal SolrLiveNodesParser =
    open SolrDotNet.Cloud
    open System

    let private filterNodes (node: struct (string * string array)) =
        let struct (_, addressAndSuffix) = node
        addressAndSuffix.Length = 2 && addressAndSuffix[1] = "solr"

    let private mapLiveNode (node: struct (string * string array)): SolrLiveNode =
        let struct (node, addressAndSuffix) = node
        { Name = node; Url = $"http://{addressAndSuffix[0]}/solr"}

    let private parseNodes(nodes: string array) =
        nodes
            |> Array.map(fun node -> struct (node, node.Split("_")))
            |> Array.filter(filterNodes)
            |> Array.map(mapLiveNode)

    let parse(nodes: string array option): SolrLiveNode array option =
        nodes |> Option.filter(fun x -> x.Length > 0) |> Option.map(parseNodes)

module internal Option =
    open System.Threading.Tasks
    let mapTask<'a, 'b>(map: 'a -> 'b Task) (option: Option<'a>): Option<'b> Task =
        task {
            match option with
            | Some(x) ->
                let! res = map(x)
                return Some(res)
            | None ->
                return None
        }
