using System;
using System.Collections.Generic;

using Shouldly;

using SolrDotNet.Cloud.Exceptions;
using SolrDotNet.Cloud.Solr;

using Xunit;

namespace SolrDotNet.Tests.Cloud.Solr;

public class SolrCollectionTests
{
    [Fact]
    public void GetAliasUrlWhenEmpty()
    {
        // Arrange
        var alias = new SolrCloudAlias("test", new List<string>());
        
        // Act

        var subject = alias.GetUrl(false);
        
        // Test
        
        subject.ShouldBeNull();

    }

    [Fact]
    public void TestGetAliasWhenHasLiveNodes_ShouldReturnRandomUri()
    {
        // Arrange 
        var nodes = new[] { "172.18.0.3:8983_solr", "172.18.0.5:8983_solr" };
        var alias = new SolrCloudAlias("xd", nodes);
        
        // Act

        var subject = alias.GetUrl(false);
        
        // Test
        subject.ShouldNotBeNull();
        subject.ShouldBeOneOf(new Uri("http://172.18.0.3:8983/solr/xd"), new Uri("http://172.18.0.5:8983/solr/xd"));
    }
    
    [Fact]
    public void GetCollectionUrlWhenEmpty()
    {
        // Arrange
        var alias = new SolrCloudCollection("test", new SolrCloudRouter("test"),
            new Dictionary<string, SolrCloudShard>());

        // Test

        Assert.Throws<NoAppropriateNodeWasSelectedException>(() => alias.GetUrl(false));

    }

    [Fact]
    public void TestGetCollectionWhenHasLiveNodes_ShouldReturnRandomUri()
    {
        // Arrange 
        var nodes = new[] { "172.18.0.3:8983_solr", "172.18.0.5:8983_solr" };
        var alias = new SolrCloudAlias("xd", nodes);
        
        // Act

        var subject = alias.GetUrl(false);
        
        // Test
        subject.ShouldNotBeNull();
        subject.ShouldBeOneOf(new Uri("http://172.18.0.3:8983/solr/xd"), new Uri("http://172.18.0.5:8983/solr/xd"));
    }
}