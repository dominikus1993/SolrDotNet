using System.Threading.Tasks;

using DotNet.Testcontainers.Containers.Builders;
using DotNet.Testcontainers.Containers.Configurations.Databases;
using DotNet.Testcontainers.Containers.Modules;
using DotNet.Testcontainers.Containers.Modules.Databases;
using DotNet.Testcontainers.Containers.WaitStrategies;
using DotNet.Testcontainers.Networks;
using DotNet.Testcontainers.Networks.Builders;
using DotNet.Testcontainers.Networks.Configurations;

using Xunit;

namespace SolrDotNet.Tests.Cloud.Fixture;

public class SolrCloudFixture : IAsyncLifetime
{
    public TestcontainersContainer Zookeeper { get; private set; }
    public  TestcontainersContainer Solr { get; private set; }
    
    public SolrCloudFixture()
    {
        Zookeeper = new TestcontainersBuilder<TestcontainersContainer>()
            .WithImage("zookeeper:3.8")
            .WithName("zoo")
            .WithPortBinding(2181, 2181)
            .WithEnvironment("ZOO_MY_ID", "1")
            .WithEnvironment("ZOO_SERVERS", "server.1=zoo:2888:3888;9983")
            .WithEnvironment("ZOO_4LW_COMMANDS_WHITELIST", "mntr, conf, ruok")
            .WithEnvironment("ZOO_CFG_EXTRA",
                "metricsProvider.className=org.apache.zookeeper.metrics.prometheus.PrometheusMetricsProvider metricsProvider.httpPort=7000 metricsProvider.exportJvmInfo=true")
            .Build();

        Solr = new TestcontainersBuilder<TestcontainersContainer>()
            .WithImage("solr:8.11-slim")
            .WithName("solr")
            .WithPortBinding(8983, 8983)
            .WithEnvironment("SOLR_OPTS", "-Dsolr.jetty.http.idleTimeout=60000 -DsocketTimeout=60000 -DconnTimeout=60000")
            .WithEnvironment("ZK_HOST", $"{Zookeeper.Hostname}:9983")
            .Build();
    }
    
    public async Task InitializeAsync()
    {
        await Zookeeper.StartAsync();
        await Solr.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await Solr.StopAsync();
        await Zookeeper.StopAsync();
    }
}