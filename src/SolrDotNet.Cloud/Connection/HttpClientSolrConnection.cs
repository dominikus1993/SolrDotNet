using SolrNet;
using SolrNet.Exceptions;
using SolrNet.Impl;
using SolrNet.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SolrDotNet.Cloud.Utils;

namespace SolrDotNet.Cloud.Connection
{
    internal class HttpClientSolrConnection : IStreamSolrConnection
    {
        private const string version = "2.2";
        public const string HttpClientSolrConnectionClient = nameof(HttpClientSolrConnectionClient);
        public HttpClient _client;
        private PostSolrConnection _syncFallbackConnection;
        private string _baseUrl;
        public int MaxUriLength { get; set; } = 7600;

        public HttpClientSolrConnection(string url, IHttpClientFactory factory)
        {
            _client = factory.CreateClient(HttpClientSolrConnectionClient);
            _baseUrl = url;
            _syncFallbackConnection = new PostSolrConnection(new SolrConnection(url), url);

        }
        public string Get(string relativeUrl, IEnumerable<KeyValuePair<string, string>> parameters) => _syncFallbackConnection.Get(relativeUrl, parameters);

        public async Task<Stream> GetAsStreamAsync(string relativeUrl, IEnumerable<KeyValuePair<string, string>>? parameters, CancellationToken cancellationToken)
        {
            var u = new UriBuilder(_baseUrl);
            u.Path += relativeUrl;
            u.Query = GetQuery(parameters);

            using var response = await GetOrPost(u, parameters, cancellationToken).ConfigureAwait(false);
 
            if (!response.IsSuccessStatusCode)
                throw new SolrConnectionException(await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false), null, u.Uri.ToString());

            return await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        }


        private Task<HttpResponseMessage> GetOrPost(UriBuilder u, IEnumerable<KeyValuePair<string, string>>? parameters, CancellationToken cancellationToken)
        {
            if (UriValidatorHelper.UriLength(u) > MaxUriLength)
            {
                u.Query = null;
                parameters ??= Enumerable.Empty<KeyValuePair<string, string>>();
                return _client.PostAsync(u.Uri, new FormUrlEncodedContent(parameters), cancellationToken);
            }
            else
                return _client.GetAsync(u.Uri, cancellationToken);
        }

        public async Task<string> GetAsync(string relativeUrl, IEnumerable<KeyValuePair<string, string>>? parameters, CancellationToken cancellationToken = default)
        {
            await using var responseStream = await GetAsStreamAsync(relativeUrl, parameters, cancellationToken).ConfigureAwait(false);
            using var sr = new StreamReader(responseStream);
            return await sr.ReadToEndAsync().ConfigureAwait(false);
        }

        public string Post(string relativeUrl, string s) => _syncFallbackConnection.Post(relativeUrl, s);

        public async Task<string> PostAsync(string relativeUrl, string s)
        {
            var bytes = Encoding.UTF8.GetBytes(s);
            await using MemoryStream content = new MemoryStream(bytes);
            await using var responseStream = await PostStreamAsStreamAsync(relativeUrl, "text/xml; charset=utf-8", content, null, default).ConfigureAwait(false);
            using var sr = new StreamReader(responseStream);
            return await sr.ReadToEndAsync().ConfigureAwait(false);
        }

        public string PostStream(string relativeUrl, string contentType, Stream content, IEnumerable<KeyValuePair<string, string>>? parameters) => _syncFallbackConnection.PostStream(relativeUrl, contentType, content, parameters);

        public async Task<Stream> PostStreamAsStreamAsync(string relativeUrl, string contentType, Stream content, IEnumerable<KeyValuePair<string, string>>? parameters, CancellationToken cancellationToken)
        {

            var u = new UriBuilder(_baseUrl);
            u.Path += relativeUrl;
            u.Query = GetQuery(parameters);

            using var sc = new StreamContent(content);
            if (contentType != null)
                sc.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse(contentType);

            using var response = await _client.PostAsync(u.Uri, sc, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
                throw new SolrConnectionException(await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false), null, u.Uri.ToString()); ;

            return await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<string> PostStreamAsync(string relativeUrl, string contentType, Stream content, IEnumerable<KeyValuePair<string, string>>? parameters)
        {
            await using var responseStream = await PostStreamAsStreamAsync(relativeUrl, contentType, content, parameters, CancellationToken.None);
            using var sr = new StreamReader(responseStream);
            return await sr.ReadToEndAsync().ConfigureAwait(false);
        }

        private static string GetQuery(IEnumerable<KeyValuePair<string, string>>? parameters)
        {
            var param = new List<KeyValuePair<string, string>>();
            if (parameters is not null)
                param.AddRange(parameters);

            param.Add(new KeyValuePair<string, string>("version", version));
            param.Add(new KeyValuePair<string, string>("wt", "xml"));

            return string.Join("&", param.Select(kv => $"{WebUtility.UrlEncode(kv.Key)}={WebUtility.UrlEncode(kv.Value)}"));

        }
    }
}
