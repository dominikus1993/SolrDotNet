using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolrDotNet.Cloud.Auth
{
    public interface ISolrCloudAuthorization
    {
        string Name { get; }
        ReadOnlyMemory<byte> GetAuthData();
    }
}
