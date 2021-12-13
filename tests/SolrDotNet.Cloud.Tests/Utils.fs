module UtilsTests

open System
open Xunit
open FsUnit.Xunit
open SolrDotNet.Cloud.Utils


[<Theory>]
[<InlineData(null, "/clusterstate.json")>]
[<InlineData("solr", "/solr/clusterstate.json")>]
let ``ZookeeperClusterStateUrlTests`` (root, expected: string) =
    let subject = Zookeeper.getClusterStateUrlPath(root)
    subject |> should equal expected

[<Theory>]
[<InlineData(null, "/aliases.json")>]
[<InlineData("solr", "/solr/aliases.json")>]
let ``ZookeeperAliasesUrlTests`` (root, expected: string) =
    let subject = Zookeeper.getAliasesUrlPath(root)
    subject |> should equal expected

[<Theory>]
[<InlineData(null, "/collections")>]
[<InlineData("solr", "/solr/collections")>]
let ``ZookeeperCollectionsUrlTests`` (root, expected: string) =
    let subject = Zookeeper.getCollectionsUrlPath(root)
    subject |> should equal expected

[<Theory>]
[<InlineData(null, "/live_nodes")>]
[<InlineData("solr", "/solr/live_nodes")>]
let ``ZookeeperLiveNodesUrlTests`` (root, expected: string) =
    let subject = Zookeeper.getLiveNodesUrlPath(root)
    subject |> should equal expected
