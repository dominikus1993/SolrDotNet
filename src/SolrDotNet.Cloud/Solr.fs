namespace SolrDotNet.Cloud

type internal SolrLiveNode = { Name: string; Url: string }

module internal SolrLiveNode =
    let getAliasUrl (alias: string) (node: SolrLiveNode) = $"{node.Url}/{alias}"
