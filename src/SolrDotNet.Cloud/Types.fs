namespace SolrDotNet.Cloud.Types
open System

type internal ISolrCollection =
    abstract member GetUrl: unit -> string option
    abstract member Name: string with get


type SolrAlias(name: string, liveNodes: string array option) =
    let random = Random()
    let urls = liveNodes |> Option.filter(fun x -> x.Length > 0) |> Option.map(fun x -> x)

    member private _.getUrl(x: string array) =
            match x.Length with
            | 0 -> failwith "Impossible"
            | 1 -> Some(x[0])
            | count -> Some(x[random.Next(0, count)])

    interface ISolrCollection with
        member _.Name with get() = name

        member this.GetUrl() = urls |> Option.bind(this.getUrl)

