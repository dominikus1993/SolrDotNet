using LanguageExt;

using SolrDotNet.Cloud.Solr;
using SolrDotNet.Cloud.Zookeeper;

using SolrNet;

namespace SolrDotNet.Cloud.Operations;

    public abstract class SolrCloudOperationsBase<T> {
        /// <summary>
        /// Is post connection
        /// </summary>
        private readonly bool isPostConnection;

        /// <summary>
        /// Collection name
        /// </summary>
        private readonly string? collectionName;

        /// <summary>
        /// Cloud state provider
        /// </summary>
        private readonly ISolrCloudStateProvider cloudStateProvider;

        /// <summary>
        /// Operations provider
        /// </summary>
        private readonly ISolrOperationsProvider operationsProvider;
        
        /// <summary>
        /// Constructor
        /// </summary>
        protected SolrCloudOperationsBase(ISolrCloudStateProvider cloudStateProvider, ISolrOperationsProvider operationsProvider, bool isPostConnection) {
            this.cloudStateProvider = cloudStateProvider;
            this.operationsProvider = operationsProvider;
            this.isPostConnection = isPostConnection;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        protected SolrCloudOperationsBase(ISolrCloudStateProvider cloudStateProvider, ISolrOperationsProvider operationsProvider, bool isPostConnection, string? collectionName = null)
            : this(cloudStateProvider, operationsProvider, isPostConnection)
        {
            this.collectionName = collectionName;
        }
        
        /// <summary>
        /// Performs basic operation
        /// </summary>
        protected ISolrBasicOperations<T> GetBasicOperations(bool leader = false)
        {
            var collection = GetCollection(collectionName);
            var url = collection.GetUrl(leader);
            if (url is null)
            {
                throw new ValueIsNullException("Url is null");
            }
            var operations = operationsProvider.GetBasicOperations<T>(
                url.AbsoluteUri,
                isPostConnection);
            if (operations is null)
                throw new ApplicationException("Operations provider returned null.");
            return operations;
        }

        /// <summary>
        /// Perform operation
        /// </summary>
        protected ISolrOperations<T> GetOperations(bool leader = false) {
            var collection = GetCollection(collectionName);
            var url = collection.GetUrl(leader);
            if (url is null)
            {
                throw new ValueIsNullException("Url is null");
            }
            var operations = operationsProvider.GetOperations<T>(
                url.AbsoluteUri,
                isPostConnection);
            if (operations == null)
                throw new ApplicationException("Operations provider returned null.");
            return operations;
        }
        

        /// <summary>
        /// Returns collection of replicas
        /// </summary>
        private ISolrCloudCollection GetCollection(string? collection) {
            var state = cloudStateProvider.GetCloudState();
            if (state == null)
            {
                throw new ApplicationException("Didn't get state from zookeeper.");
            }

            return state.GetCollection(collection);
        }
    }