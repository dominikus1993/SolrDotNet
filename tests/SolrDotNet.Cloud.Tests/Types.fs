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


[<Fact>]
let ``get collection when exists in router`` () =
    let urls = [|"172.18.0.3:8983_solr"|]
    let alias = SolrAlias("xd", Some(urls))
    let router = SolrCollectionRouter.empty |> SolrCollectionRouter.add alias
    let url = router |> SolrCollectionRouter.getCollectionUrl "xd"
    url.IsSome |> should be True
    url.Value |> should equal "http://172.18.0.3:8983/solr/xd"


[<Fact>]
let ``get collection when no exists in router`` () =
    let urls = [|"172.18.0.3:8983_solr"|]
    let alias = SolrAlias("xd", Some(urls))
    let router = SolrCollectionRouter.empty |> SolrCollectionRouter.add alias
    let url = router |> SolrCollectionRouter.getCollectionUrl "xdd"
    url.IsSome |> should be False

[<Fact>]
let ``get collection when router is empty`` () =
    let router = SolrCollectionRouter.empty
    let url = router |> SolrCollectionRouter.getCollectionUrl "xdd"
    url.IsSome |> should be False


[<Fact>]
let ``check hasAnyCollections when is not empty`` () =
    let urls = [|"172.18.0.3:8983_solr"|]
    let alias = SolrAlias("xd", Some(urls))
    let router = SolrCollectionRouter.empty |> SolrCollectionRouter.add alias
    let subject  = router |> SolrCollectionRouter.hasAnyCollections
    subject |> should be True

[<Fact>]
let ``check hasAnyCollections when is empty`` () =
    let router = SolrCollectionRouter.empty
    let subject = router |> SolrCollectionRouter.hasAnyCollections
    subject |> should be False
