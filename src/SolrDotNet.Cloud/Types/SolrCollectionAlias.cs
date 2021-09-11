using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolrDotNet.Cloud.Types
{
    internal class SolrCollectionAlias : ISolrCollection
    {
        public string Name => throw new NotImplementedException();

        public string GetUrl()
        {
            throw new NotImplementedException();
        }
    }
}
