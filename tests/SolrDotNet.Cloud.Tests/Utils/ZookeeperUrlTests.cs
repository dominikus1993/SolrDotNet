using FluentAssertions;
using SolrDotNet.Cloud.Utils;
using Xunit;

namespace SolrDotNet.Cloud.Tests.Utils
{
    public class ZookeeperUrlTests
    {
        [Theory]
        [InlineData(null, "/clusterstate.json")]
        [InlineData("solr", "/solr/clusterstate.json")]
        public void ZookeeperClusterStateUrlTests(string? root, string expected)
        {
            var subject = ZookeeperUrl.GetClusterStateUrlPath(root);
            subject.Should().Be(expected);
        }

        [Theory]
        [InlineData(null, "/aliases.json")]
        [InlineData("solr", "/solr/aliases.json")]
        public void ZookeeperAliasesUrlTests(string? root, string expected)
        {
            var subject = ZookeeperUrl.GetAliasesUrlPath(root);
            subject.Should().Be(expected);
        }

        [Theory]
        [InlineData(null, "/collections")]
        [InlineData("solr", "/solr/collections")]
        public void ZookeeperCollectionsUrlTests(string? root, string expected)
        {
            var subject = ZookeeperUrl.GetCollectionsUrlPath(root);
            subject.Should().Be(expected);
        }


        [Theory]
        [InlineData(null, "/live_nodes")]
        [InlineData("solr", "/solr/live_nodes")]
        public void ZookeeperLiveNodesUrlTests(string? root, string expected)
        {
            var subject = ZookeeperUrl.GetLiveNodesUrlPath(root);
            subject.Should().Be(expected);
        }
    }
}
