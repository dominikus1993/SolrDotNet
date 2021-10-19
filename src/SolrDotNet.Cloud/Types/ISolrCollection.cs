﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolrDotNet.Cloud.Types
{
    public interface ISolrCollection
    {
        string Name { get; }

        string GetUrl();
    }
}