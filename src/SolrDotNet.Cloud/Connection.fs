﻿namespace SolrDotNet.Cloud
open SolrDotNet.Cloud.Types
open System
open System.Threading.Tasks

type ISolrCloudState =
    interface
    end

type internal SolrCloudState(router: SolrCollectionRouter.T) = class end

type ISolrCloudConnection =
    inherit IAsyncDisposable
    inherit IDisposable
    abstract member GetState: unit -> ISolrCloudState
    abstract member Key: string with get
    abstract member Init: unit -> Task

module internal SolrAlias =
    open org.apache.zookeeper
    open SolrDotNet.Cloud.Utils
    open Newtonsoft.Json
    open System.Text

    let loadAliases (z: ZooKeeper)(root: string) =
        task {
            let! aliasesJson = z.getDataAsync(Zookeeper.getAliasesUrlPath(root), true)
            if isNull aliasesJson.Data then
                return None
            else
                return Some(JsonConvert.DeserializeObject<SolrAliases>(Encoding.UTF8.GetString(aliasesJson.Data)))
        }
