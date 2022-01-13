﻿namespace SolrDotNet.Cloud

open SolrDotNet.Cloud.Types
open System
open System.Threading.Tasks
open org.apache.zookeeper
open SolrDotNet.Cloud.Utils

type ISolrCloudState =
    interface
    end

type internal SolrCloudState(router: SolrCollectionRouter.T) =
    class
    end

type ISolrCloudConnection =
    inherit IAsyncDisposable
    inherit IDisposable
    abstract member GetState : unit -> ISolrCloudState
    abstract member Key : string
    abstract member Init : unit -> Task

[<AbstractClass>]
type ZookeeperSolrCloudConnection() =
    inherit Watcher()
    abstract member Zookeeper : ZooKeeper

module internal SolrAlias =
    open org.apache.zookeeper
    open SolrDotNet.Cloud.Utils
    open Newtonsoft.Json
    open System.Text

    let loadAliases (z: ZookeeperSolrCloudConnection) (root: string) =
        task {
            let! aliasesJson = z.Zookeeper.getDataAsync (Zookeeper.getAliasesUrlPath (root), true)

            return
                aliasesJson
                |> Option.ofObj
                |> Option.bind (fun json -> json.Data |> Option.ofObj)
                |> Option.map (fun data -> JsonConvert.DeserializeObject<SolrAliases>(Encoding.UTF8.GetString(data)))
        }

module internal LiveNodes =
    open org.apache.zookeeper
    open SolrDotNet.Cloud.Utils
    open Newtonsoft.Json
    open System.Text
    open SolrDotNet.Cloud.Utils

    let load (z: ZookeeperSolrCloudConnection) (root: string) =
        task {
            let! liveNodesChildren = z.Zookeeper.getChildrenAsync (Zookeeper.getLiveNodesUrlPath (root), z)

            let children =
                liveNodesChildren.Children
                |> Option.ofObj
                |> Option.filter (fun x -> x.Count > 0)
                |> Option.map (fun x -> x.ToArray())

            return SolrLiveNodesParser.parse (children)
        }

module SolrCollections =
    let map (z: ZookeeperSolrCloudConnection) (root: string) (data: string seq) =
        task {
            return 1
        }

    let load (z: ZookeeperSolrCloudConnection) (root: string) =
        task {
            let! children =
                z.Zookeeper.getChildrenAsync(Zookeeper.getCollectionsUrlPath(root), true)

            let! collections =
                children
                    |> Option.ofObj
                    |> Option.map(fun x -> x.Children)
                    |> Option.filter(fun x -> x.Count > 0)
                    |> Option.mapTask(map(z)(root))



        }

type ZooKeeperSolrCloudConnection() =
    class
    end
