using FluentAssertions;
using SolrDotNet.Cloud.Solr.Nodes;
using SolrDotNet.Cloud.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SolrDotNet.Cloud.Tests.Utils
{
    public class SolrLiveNodesParserTests
    {
        [Fact]
        public void ParseLiveNodesTest()
        {
            var nodes = new List<string>() { "172.18.0.3:8983_solr" };
            var subject = SolrLiveNodesParser.Parse(nodes).ToList();
            subject.Should().NotBeNullOrEmpty();
            subject.Should().HaveCount(1);
            var node = subject[0];
            node.Should().Be(new SolrLiveNode("172.18.0.3:8983_solr", "http://172.18.0.3:8983/solr"));
            node.Url.Should().Be("http://172.18.0.3:8983/solr");
        }

        [Fact]
        public void ParseLiveNodesTestWhenListIsEmpty()
        {
            var nodes = new List<string>() { };
            Assert.Throws<Exception>(() => SolrLiveNodesParser.Parse(nodes).ToList());
        }

        [Fact]
        public void GetAliasUrlFromNodeTests()
        {
            var nodes = new List<string>() { "172.18.0.3:8983_solr" };
            const string alias = "xd";
            var subject = SolrLiveNodesParser.Parse(nodes).ToList();
            subject.Should().NotBeNullOrEmpty();
            subject.Should().HaveCount(1);
            var node = subject[0];
            var aliasUrl = node.GetAliasUrl(alias);
            aliasUrl.Should().Be("http://172.18.0.3:8983/solr/xd");
        }
    }
}
