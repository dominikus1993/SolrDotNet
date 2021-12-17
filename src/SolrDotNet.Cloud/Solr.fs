namespace SolrDotNet.Cloud
open System
open System.Collections.Generic

type internal SolrLiveNode = { Name: string; Url: string }

module internal SolrLiveNode =
    let getAliasUrl (alias: string) (node: SolrLiveNode) = $"{node.Url}/{alias}"


type SolrCloudReplica = { IsActive: bool; IsLeader: bool; Name: string; Url: string}

type SolrCloudShard = { IsActive: bool; Name: string; RangeEnd: Nullable<int>; RangeStart: Nullable<int>; Replicas: IReadOnlyDictionary<string, SolrCloudReplica> }
