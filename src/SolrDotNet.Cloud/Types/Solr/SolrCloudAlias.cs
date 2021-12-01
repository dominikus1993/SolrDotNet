using Newtonsoft.Json;

namespace SolrDotNet.Cloud.Types.Solr;

internal class Aliases
{
    [JsonProperty("collection")]
    public Dictionary<string, string> Collection { get; set; } = new Dictionary<string, string>();

    public bool IsEmpty()
    {
        return Collection.Count == 0;
    }
}
