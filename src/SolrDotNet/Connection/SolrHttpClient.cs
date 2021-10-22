namespace SolrDotNet.Connection
{
    public class SolrHttpClient
    {
        private HttpClient _client;

        public SolrHttpClient(HttpClient client)
        {
            _client = client;
        }
    }
}
