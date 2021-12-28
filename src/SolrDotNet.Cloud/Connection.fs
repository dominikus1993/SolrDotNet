namespace SolrDotNet.Cloud
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

