using SolrDotNet.Cloud.Connection;
using SolrDotNet.Cloud.SolrRouter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolrDotNet.Cloud.ConnectionManager
{
    public interface ISolrCloudConnectionManager : IDisposable, IAsyncDisposable
    {
        ISolrCloudConnection GetCloudConnection();

        Task ConnectAsync();
    }
}
