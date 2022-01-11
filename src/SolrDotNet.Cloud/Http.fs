namespace SolrDotNet.Http
open System.Net.Http
open SolrNet.Impl
open SolrNet
open System;
open System.Collections.Generic;
open System.IO;
open System.Linq;
open System.Net.Http;
open System.Net;
open System.Text;
open SolrNet.Exceptions
open SolrDotNet.Cloud.Utils
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

    let getQuery(parameters: KeyValuePair<string, string> seq) =
        let param = ResizeArray<KeyValuePair<string, string>>();
        if isNull parameters then
            param.AddRange(parameters);
        param.Add(new KeyValuePair<string, string>("version", Version));
        param.Add(new KeyValuePair<string, string>("wt", "xml"));
        String.Join("&", param |> Seq.map(fun kv -> $"{WebUtility.UrlEncode(kv.Key)}={WebUtility.UrlEncode(kv.Value)}"));

    let getOrPost(u: UriBuilder, parameters, cancellationToken) =
         if (Url.length(u) > MaxUriLength) then
            u.Query <- null;
            let p = if isNull parameters then Enumerable.Empty<KeyValuePair<string, string>>() else parameters
            httpClient.PostAsync(u.Uri, new FormUrlEncodedContent(p), cancellationToken);
         else
            httpClient.GetAsync(u.Uri, cancellationToken);

    let getAsStreamAsync(relativeUrl, parameters, cancellationToken) =
        task {
            let u = new UriBuilder(url);
            u.Path <- u.Path + relativeUrl;
            u.Query <- getQuery(parameters);
            use! response = getOrPost(u, parameters, cancellationToken).ConfigureAwait(false);
            if not response.IsSuccessStatusCode then
                let! resp = response.Content.ReadAsStringAsync(cancellationToken)
                raise (SolrConnectionException(resp, null, u.Uri.ToString()))
            return! response.Content.ReadAsStreamAsync(cancellationToken);
        }

    let postStreamAsStreamAsync(relativeUrl, contentType, content,  parameters, cancellationToken) =
        task {
            let u = UriBuilder(url);
            u.Path <- u.Path + relativeUrl;
            u.Query <- getQuery(parameters);
            use sc = new StreamContent(content);
            if not (isNull content) then
                sc.Headers.ContentType <- System.Net.Http.Headers.MediaTypeHeaderValue.Parse(contentType);
            use! response =  httpClient.PostAsync(u.Uri, sc, cancellationToken);
            if not response.IsSuccessStatusCode then
                let! res = response.Content.ReadAsStringAsync(cancellationToken)
                raise (SolrConnectionException(res, null, u.Uri.ToString()))
            return! response.Content.ReadAsStreamAsync(cancellationToken);
        }

    interface IStreamSolrConnection with
        member _.Get(relativeUrl, parameters) = syncFallbackConenction.Get(relativeUrl, parameters)

        member this.GetAsStreamAsync(relativeUrl, parameters, cancellationToken) = getAsStreamAsync(relativeUrl, parameters, cancellationToken)

        member this.GetAsync(relativeUrl, parameters, cancellationToken) =
            task {
                use! responseStream =  getAsStreamAsync(relativeUrl, parameters, cancellationToken)
                use sr = new StreamReader(responseStream);
                return! sr.ReadToEndAsync();
            }

        member _.Post(relativeUrl, s) = syncFallbackConenction.Post(relativeUrl, s);

        member _.PostStreamAsStreamAsync(relativeUrl, contentType, content,  parameters, cancellationToken) = postStreamAsStreamAsync(relativeUrl,contentType, content, parameters, cancellationToken)
