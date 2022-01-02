namespace SolrDotNet.Http
open System.Net.Http
open SolrNet.Impl

    // private const string Version = "2.2";
    // public const string HttpClientSolrConnectionClient = nameof(HttpClientSolrConnectionClient);
    // private readonly HttpClient _client;
    // private readonly PostSolrConnection _syncFallbackConnection;
    // private readonly string _baseUrl;
    // public int MaxUriLength { get; set; } = 7600;
type HttpClientSolrConnection internal(url: string, httpClient: HttpClient) =
    [<Literal>]
    let Version = "2.2"
    [<Literal>]
    let MaxUriLength = 7600
    let syncFallbackConenction = PostSolrConnection(SolrConnection(url), url)
