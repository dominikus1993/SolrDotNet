module UtilsTests

open System
open Xunit
open FsUnit.Xunit
open SolrDotNet.Cloud
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


[<Fact>]
let ``ParseLiveNodesTest`` () =
    let nodes = [| "172.18.0.3:8983_solr" |]
    let subject = SolrLiveNodesParser.parse(Some(nodes))
    subject.IsSome |> should be True
    subject.Value |> should not' (be Empty)
    subject.Value |> should haveLength 1
    let node = subject.Value[0]
    node |> should equal ({Name = "172.18.0.3:8983_solr"; Url = "http://172.18.0.3:8983/solr"})

[<Fact>]
let ``ParseLiveNodesTestWhenListIsEmpty`` () =
    let nodes = Array.empty
    let subject = SolrLiveNodesParser.parse(Some(nodes))
    subject.IsNone |> should be True

[<Fact>]
let ``GetAliasUrlFromNodeTests`` () =
    let nodes = [| "172.18.0.3:8983_solr" |]
    let subject = SolrLiveNodesParser.parse(Some(nodes))
    subject.IsSome |> should be True
    subject.Value |> should not' (be Empty)
    subject.Value |> should haveLength 1
    let node = subject.Value[0]
    node |> should equal ({Name = "172.18.0.3:8983_solr"; Url = "http://172.18.0.3:8983/solr"})
    let alias = "xd"
    let aliasUrl = node |> SolrLiveNode.getAliasUrl alias
    aliasUrl |> should equal "http://172.18.0.3:8983/solr/xd"
