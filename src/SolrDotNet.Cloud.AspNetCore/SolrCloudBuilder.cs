using System.Net;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SolrDotNet.Cloud.Exceptions;
using SolrDotNet.Cloud.Extensions;
using SolrDotNet.Cloud.Operations;
using SolrDotNet.Cloud.Zookeeper;
using SolrDotNet.Impl;

using SolrNet;

namespace SolrDotNet.Cloud.AspNetCore;

    public sealed class SolrCloudBuilder
    {

        /// <summary>
        /// Collection list
        /// </summary>
        private static readonly HashSet<string> Collections;

        /// <summary>
        /// Cloud state providers
        /// </summary>
        private static readonly IDictionary<string, ISolrCloudStateProvider> Providers;


        /// <summary>
        /// Constructor
        /// </summary>
        static SolrCloudBuilder()
        {
            Collections = new HashSet<string>();
            Providers = new Dictionary<string, ISolrCloudStateProvider>(StringComparer.OrdinalIgnoreCase);
        }

        private readonly SolrCloudConfig _configuration;
        private readonly IServiceCollection _services;
        private readonly SolrCloudClient _aliasSolrCloudStateProvider;

        public SolrCloudBuilder(SolrCloudConfig configuration, IServiceCollection services, SolrCloudClient aliasSolrCloudStateProvider)
        {
            _configuration = configuration;
            _services = services;
            _aliasSolrCloudStateProvider = aliasSolrCloudStateProvider;
        }

        public void Init<T>(bool isPostConnection = false) => AsyncHelper.RunSync(() => InitAsync<T>(isPostConnection));

        public void Init<T>(string collectionName, bool isPostConnection = false) => AsyncHelper.RunSync(() => InitAsync<T>(collectionName, isPostConnection));

        /// <summary>
        /// Startup initializing 
        /// </summary>
        private async Task InitAsync<T>(bool isPostConnection = false)
        {
            if (_aliasSolrCloudStateProvider == null)
                throw new ArgumentNullException("cloudStateProvider");
            await EnsureRegistrationAsync(_services, _aliasSolrCloudStateProvider);

            if (!Collections.Add(string.Empty))
                return;

            _services.AddTransient<ISolrBasicOperations<T>>(x =>
                new SolrCloudBasicOperations<T>(
                    x.GetService<ISolrCloudStateProvider>(),
                    x.GetService<ISolrOperationsProvider>(),
                    isPostConnection));

            _services.AddTransient<ISolrBasicReadOnlyOperations<T>>(x =>
                new SolrCloudBasicOperations<T>(
                    x.GetService<ISolrCloudStateProvider>(),
                    x.GetService<ISolrOperationsProvider>(),
                    isPostConnection));

            _services.AddTransient<ISolrOperations<T>>(x =>
                new SolrCloudOperations<T>(
                    x.GetService<ISolrCloudStateProvider>(),
                    x.GetService<ISolrOperationsProvider>(),
                    isPostConnection));

            _services.AddTransient<ISolrReadOnlyOperations<T>>(x =>
                new SolrCloudOperations<T>(
                    x.GetService<ISolrCloudStateProvider>(),
                    x.GetService<ISolrOperationsProvider>(),
                    isPostConnection));
        }

        /// <summary>
        /// Startup initializing 
        /// </summary>
        private async Task InitAsync<T>(string collectionName, bool isPostConnection = false)
        {
            if (_aliasSolrCloudStateProvider == null)
                throw new ArgumentNullException(nameof(_aliasSolrCloudStateProvider));
            if (string.IsNullOrEmpty(collectionName))
                throw new ArgumentNullException(nameof(collectionName));

            await EnsureRegistrationAsync(_services, _aliasSolrCloudStateProvider);

            if (!Collections.Add(collectionName))
                return;

            _services.AddTransient<ISolrBasicOperations<T>>(x =>
                new SolrCloudBasicOperations<T>(
                    x.GetService<ISolrCloudStateProvider>(),
                    x.GetService<ISolrOperationsProvider>(),
                    collectionName,
                    isPostConnection));

            _services.AddTransient<ISolrBasicReadOnlyOperations<T>>(x =>
                new SolrCloudBasicOperations<T>(
                    x.GetService<ISolrCloudStateProvider>(),
                    x.GetService<ISolrOperationsProvider>(),
                    collectionName,
                    isPostConnection));

            _services.AddTransient<ISolrOperations<T>>(x =>
                new SolrCloudOperations<T>(
                    x.GetService<ISolrCloudStateProvider>(),
                    x.GetService<ISolrOperationsProvider>(),
                    collectionName,
                    isPostConnection));

            _services.AddTransient<ISolrReadOnlyOperations<T>>(x =>
                new SolrCloudOperations<T>(
                    x.GetService<ISolrCloudStateProvider>(),
                    x.GetService<ISolrOperationsProvider>(),
                    collectionName,
                    isPostConnection));
        }

        /// <summary>
        /// Ensures registrations and initializing
        /// </summary>
        private static async Task EnsureRegistrationAsync(IServiceCollection services, ISolrCloudStateProvider cloudStateProvider)
        {
            if (Providers.Count == 0)
                services.AddTransient<ISolrOperationsProvider, HttpClientOperationsProvider>();
            services.AddHttpClient(HttpClientSolrConnection.HttpClientSolrConnectionClient)
                        .ConfigurePrimaryHttpMessageHandler(() =>
                        {
                            return new HttpClientHandler
                            {
                                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
                            };
                        });
            if (Providers.ContainsKey(cloudStateProvider.Key))
                return;
            await cloudStateProvider.InitAsync();
            Providers.Add(cloudStateProvider.Key, cloudStateProvider);
            services.AddTransient<ISolrCloudStateProvider>(_ => cloudStateProvider);
        }

        private class OperationsProvider : ISolrOperationsProvider
        {
            public ISolrBasicOperations<T> GetBasicOperations<T>(string url, bool isPostConnection = false)
            {
                return SolrNet.GetBasicServer<T>(url, isPostConnection);
            }

            public ISolrOperations<T> GetOperations<T>(string url, bool isPostConnection = false)
            {
                return SolrNet.GetServer<T>(url, isPostConnection);
            }
        }

        private class HttpClientOperationsProvider : ISolrOperationsProvider
        {
            private IHttpClientFactory clientFactory;

            public HttpClientOperationsProvider(IHttpClientFactory clientFactory)
            {
                this.clientFactory = clientFactory;
            }

            public ISolrBasicOperations<T> GetBasicOperations<T>(string url, bool _ = false)
            {
                return HttpClientSolrNet.GetBasicServer<T>(url, clientFactory);
            }

            public ISolrOperations<T> GetOperations<T>(string url, bool _ = false)
            {
                return HttpClientSolrNet.GetServer<T>(url, clientFactory);
            }
        }
    }
    public static class SolrCloud
    {

        public static void AddSolrCloud(this IServiceCollection servcies, IConfiguration config, Action<SolrCloudBuilder> setupAction)
        {
            var cfg = config.GetSection("SolrCloud").Get<SolrCloudConfig>();
            var auth = cfg.Auth?.ToAuth();
            if (cfg is { Connection: { Length: > 0 } })
            {
                var provider = new SolrCloudClient(cfg.Connection, cfg.ZkRoot, auth);
                var builder = new SolrCloudBuilder(cfg, servcies, provider);
                setupAction(builder);
            }
        }

        public static void AddSolrCloud(this IServiceCollection servcies, SolrCloudConfig config, Action<SolrCloudBuilder> setupAction)
        {
            var auth = config.Auth?.ToAuth();
            if (config is { Connection: { Length: > 0 } })
            {
                var provider = new SolrCloudClient(config.Connection, config.ZkRoot, auth);
                var builder = new SolrCloudBuilder(config, servcies, provider);
                setupAction(builder);
            }
        }
    }