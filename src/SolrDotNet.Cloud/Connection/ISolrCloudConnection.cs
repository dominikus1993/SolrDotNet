using SolrDotNet.Cloud.SolrRouter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolrDotNet.Cloud.Connection
{
    public interface ISolrCloudConnection
    {
        ISolrCloudRouter GetRouter();
    }
}
