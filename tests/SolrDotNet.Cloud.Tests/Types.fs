module TypesTests

open System
open Xunit
open FsUnit.Xunit
open SolrDotNet.Cloud
open SolrDotNet.Cloud.Utils
open SolrDotNet.Cloud.Types


[<Fact>]
let ``Test Alias when has active nodes`` () =
    let urls = [|"172.18.0.3:8983_solr"; "172.18.0.5:8983_solr"|]
    let alias = SolrAlias("xd", Some(urls))
    let subject: ISolrCollection = alias
    subject.Name |> should equal "xd"
    let url = subject.GetUrl()
    url.IsSome |> should be True
    [url.Value] |> should be (subsetOf ["http://172.18.0.3:8983/solr/xd"; "http://172.18.0.5:8983/solr/xd"] )


[<Fact>]
let ``Test Alias when has no active nodes`` () =
    let urls = [||]
    let alias = SolrAlias("xd", Some(urls))
    let subject: ISolrCollection = alias
    subject.Name |> should equal "xd"
    let url = subject.GetUrl()
    url.IsSome |> should be False


[<Fact>]
let ``Test Alias when has none active nodes`` () =
    let alias = SolrAlias("xd", None)
    let subject: ISolrCollection = alias
    subject.Name |> should equal "xd"
    let url = subject.GetUrl()
    url.IsSome |> should be False
