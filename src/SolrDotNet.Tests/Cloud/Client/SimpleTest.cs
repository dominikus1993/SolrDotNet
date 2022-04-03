using System;

using SolrDotNet.Tests.Cloud.Fixture;

using Xunit;

namespace SolrDotNet.Tests.Cloud.Client;

public class SimpleTest : IClassFixture<SolrCloudFixture>
{
    private SolrCloudFixture _solrCloudFixture;

    public SimpleTest(SolrCloudFixture solrCloudFixture)
    {
        _solrCloudFixture = solrCloudFixture;
    }
    
}