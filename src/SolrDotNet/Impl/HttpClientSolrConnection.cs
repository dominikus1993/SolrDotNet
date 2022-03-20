using System.Net;
using System.Text;

using SolrDotNet.Utils;

using SolrNet;
using SolrNet.Exceptions;
using SolrNet.Impl;

namespace SolrDotNet.Impl;

public class HttpClientSolrConnection : IStreamSolrConnection
{
    private const string Version = "2.2";
    public const string HttpClientSolrConnectionClient = nameof(HttpClientSolrConnectionClient);
    private readonly HttpClient _client;
    private readonly PostSolrConnection _syncFallbackConnection;
    private readonly string _baseUrl;
    public int MaxUriLength { get; set; } = 7600;

    public HttpClientSolrConnection(string url, IHttpClientFactory factory)
    {
        _client = factory.CreateClient(HttpClientSolrConnectionClient);
        _baseUrl = url;
        _syncFallbackConnection = new PostSolrConnection(new SolrConnection(url), url);

    }
    public string Get(string relativeUrl, IEnumerable<KeyValuePair<string, string>> parameters) => _syncFallbackConnection.Get(relativeUrl, parameters);

    /// <inheritdoc />
    public async Task<string> GetAsync(string relativeUrl, IEnumerable<KeyValuePair<string, string>> parameters, CancellationToken cancellationToken = default)
    {
        await using var responseStream = await GetAsStreamAsync(relativeUrl, parameters, cancellationToken);
        using var sr = new StreamReader(responseStream);
        return await sr.ReadToEndAsync();
    }

    /// <inheritdoc />
    public async Task<Stream> GetAsStreamAsync(string relativeUrl, IEnumerable<KeyValuePair<string, string>> parameters, CancellationToken cancellationToken)
    {
        var parameterList = parameters?.ToList();
        var u = new UriBuilder(_baseUrl);
        u.Path += relativeUrl;
        u.Query = GetQuery(parameterList);

        HttpResponseMessage response;
        if (u.UriLength() > MaxUriLength)
        {
            u.Query = null;
            response = await _client.PostAsync(u.Uri, new FormUrlEncodedContent(parameterList), cancellationToken);
        }
        else
            response = await _client.GetAsync(u.Uri, cancellationToken);

        return !response.IsSuccessStatusCode
            ? throw new SolrConnectionException(await response.Content.ReadAsStringAsync(cancellationToken), null, u.Uri.ToString())
            : await response.Content.ReadAsStreamAsync(cancellationToken);
    }

    /// <inheritdoc />
    public string Post(string relativeUrl, string s) => _syncFallbackConnection.Post(relativeUrl, s);

    /// <inheritdoc />
    public Task<string> PostAsync(string relativeUrl, string s) => PostAsync(relativeUrl, s, CancellationToken.None);

    /// <inheritdoc />
    public async Task<string> PostAsync(string relativeUrl, string s, CancellationToken cancellationToken)
    {
        var bytes = Encoding.UTF8.GetBytes(s);
        await using var content = new MemoryStream(bytes);
        await using var responseStream = await PostStreamAsStreamAsync(relativeUrl, "text/xml; charset=utf-8", content, null, cancellationToken);
        using var sr = new StreamReader(responseStream);
        return await sr.ReadToEndAsync();
    }

    /// <inheritdoc />
    public string PostStream(string relativeUrl, string contentType, Stream content, IEnumerable<KeyValuePair<string, string>> getParameters) => _syncFallbackConnection.PostStream(relativeUrl, contentType, content, getParameters);

    /// <inheritdoc />
    public async Task<string> PostStreamAsync(string relativeUrl, string contentType, Stream content, IEnumerable<KeyValuePair<string, string>> getParameters)
    {
        await using var responseStream = await PostStreamAsStreamAsync(relativeUrl, contentType, content, getParameters, CancellationToken.None);
        using var sr = new StreamReader(responseStream);
        return await sr.ReadToEndAsync();
    }

    /// <inheritdoc />
    public async Task<Stream> PostStreamAsStreamAsync(string relativeUrl, string contentType, Stream content, IEnumerable<KeyValuePair<string, string>>? getParameters, CancellationToken cancellationToken)
    {
        var u = new UriBuilder(_baseUrl);
        u.Path += relativeUrl;
        u.Query = GetQuery(getParameters?.ToList());

        using var sc = new StreamContent(content);
        if (contentType != null)
            sc.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse(contentType);

        var response = await _client.PostAsync(u.Uri, sc, cancellationToken);

        return !response.IsSuccessStatusCode
            ? throw new SolrConnectionException(await response.Content.ReadAsStringAsync(cancellationToken), null, u.Uri.ToString())
            : await response.Content.ReadAsStreamAsync(cancellationToken);
    }


    private static string GetQuery(IList<KeyValuePair<string, string>>? parameters)
    {
        var param = new List<string>();
        if (parameters is not null)
            param.AddRange(parameters.Select(kv => $"{WebUtility.UrlEncode(kv.Key)}={WebUtility.UrlEncode(kv.Value)}"));

        param.Add($"version={Version}");
        param.Add("wt=xml");

        return string.Join("&", param);

    }
}

