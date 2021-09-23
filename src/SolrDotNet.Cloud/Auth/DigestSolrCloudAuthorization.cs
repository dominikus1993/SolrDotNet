using System;
using System.Text;

namespace SolrDotNet.Cloud.Auth
{
    public sealed record DigestSolrCloudAuthorization(string Username, string Password) : ISolrCloudAuthorization
    {
        public string Name => "digest";

        public ReadOnlyMemory<byte> GetAuthData()
        {
            return Encoding.UTF8.GetBytes($"{Username}:{Password}");
        }
    }
}
