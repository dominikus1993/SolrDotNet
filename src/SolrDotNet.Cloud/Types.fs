namespace SolrDotNet.Cloud.Types
open System
open SolrDotNet.Cloud.Utils
open SolrDotNet.Cloud
open System.Collections.Generic
open Newtonsoft.Json.Linq

type internal ISolrCollection =
    abstract member GetUrl: unit -> string option
    abstract member Name: string with get


type internal SolrAlias(name: string, liveNodes: string array option) =
    let random = Random()
    let urls = liveNodes |> Option.filter(fun x -> x.Length > 0) |> SolrLiveNodesParser.parse |> Option.map(fun nodes -> nodes |> Array.map(SolrLiveNode.getAliasUrl name))

    member private _.getUrl(x: string array) =
            match x.Length with
            | 0 -> failwith "Impossible"
            | 1 -> Some(x[0])
            | count -> Some(x[random.Next(0, count)])

    interface ISolrCollection with
        member _.Name with get() = name

        member this.GetUrl() = urls |> Option.bind(this.getUrl)

type internal SolrCollection(name: string, shards: IReadOnlyDictionary<string, SolrCloudShard>) =
    let random = Random()
    let activeReplicas = shards.Values
                            |> Seq.filter(fun n -> n.IsActive)
                            |> Seq.choose(fun x -> x.Replicas |> Option.ofObj)
                            |> Seq.collect(fun x -> x.Values)
                            |> Seq.filter(fun x -> x.IsActive)
                            |> Seq.toArray

    member private _.getUrl(x: SolrCloudReplica array) =
            match x.Length with
            | 0 -> failwith "Impossible"
            | 1 -> Some(x[0])
            | count -> Some(x[random.Next(0, count)])

    interface ISolrCollection with
        member _.Name with get() = name

        member this.GetUrl() = this.getUrl(activeReplicas) |> Option.map(fun x -> x.Url)

module internal SolrCollection =
    let parse (json: JProperty) (nodes) =
        2

module internal SolrCollectionRouter =
    type internal T = { Collections: Map<string, ISolrCollection>}

    let empty = { Collections = Map.empty }

    let add(collection: ISolrCollection) (solr: T) =
        { solr with Collections = Map.add collection.Name collection solr.Collections}

    let hasAnyCollections (solr: T) = not solr.Collections.IsEmpty

    let getCollectionUrl (name: string) (solr: T) =
        if solr.Collections.ContainsKey(name) then
            solr.Collections[name].GetUrl()
        else
            None

     


