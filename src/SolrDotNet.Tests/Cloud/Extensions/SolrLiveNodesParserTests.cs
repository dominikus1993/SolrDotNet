using System;
using System.Collections.Generic;
using System.Linq;

using Shouldly;

using SolrDotNet.Cloud.Extensions;

using Xunit;

namespace SolrDotNet.Tests.Cloud.Extensions;

public class SolrLiveNodesParserTests
{
    [Fact]
    public void ParseLiveNodesTest()
    {
        var nodes = new List<string>() { "172.18.0.3:8983_solr" };
        var alias = "xd";
        var subject = SolrLiveNodesParser.ParseAlias(nodes, alias).ToList();
        subject.ShouldNotBeNull();
        subject.ShouldNotBeEmpty();
        subject.Count.ShouldBe(1);
        var node = subject[0];
        node.ShouldBe(new SolrLiveNode(new Uri("http://172.18.0.3:8983/solr/xd")));
        node.Url.ShouldBe(new Uri("http://172.18.0.3:8983/solr/xd"));
    }

    [Fact]
    public void ParseLiveNodesTestWhenListIsEmpty()
    {
        var nodes = new List<string>() {};
        Assert.Throws<Exception>(() => SolrLiveNodesParser.ParseAlias(nodes, "xd").ToList());
    }
}