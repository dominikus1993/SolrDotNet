using SolrDotNet.Cloud.Zookeeper;

using SolrNet;
using SolrNet.Commands.Parameters;
using SolrNet.Impl;
using SolrNet.Schema;

namespace SolrDotNet.Cloud.Operations;

public class SolrCloudBasicOperations<T> : SolrCloudOperationsBase<T>, ISolrBasicOperations<T>
    {
        public SolrCloudBasicOperations(ISolrCloudStateProvider cloudStateProvider, ISolrOperationsProvider operationsProvider)
            : base(cloudStateProvider, operationsProvider, isPostConnection: false) { }
        public SolrCloudBasicOperations(ISolrCloudStateProvider cloudStateProvider, ISolrOperationsProvider operationsProvider, bool isPostConnection = false)
            : base(cloudStateProvider, operationsProvider, isPostConnection) { }

        public SolrCloudBasicOperations(ISolrCloudStateProvider cloudStateProvider, ISolrOperationsProvider operationsProvider, string collectionName)
            : base(cloudStateProvider, operationsProvider, isPostConnection: false, collectionName: collectionName) { }
        public SolrCloudBasicOperations(ISolrCloudStateProvider cloudStateProvider, ISolrOperationsProvider operationsProvider, string collectionName, bool isPostConnection = false)
            : base(cloudStateProvider, operationsProvider, isPostConnection, collectionName) { }

        public SolrQueryResults<T> Query(ISolrQuery query, QueryOptions options)
        {
            var operations = base.GetBasicOperations();
            return operations.Query(query, options);
        }

        public SolrMoreLikeThisHandlerResults<T> MoreLikeThis(SolrMLTQuery query, MoreLikeThisHandlerQueryOptions options)
        {
            var operations = base.GetBasicOperations();
            return  operations.MoreLikeThis(query, options);
        }

        public ResponseHeader Ping()
        {
            var operations = base.GetBasicOperations();
            return operations.Ping();
        }

        public SolrSchema GetSchema(string schemaFileName)
        {
            var operations = base.GetBasicOperations();
            return operations.GetSchema(schemaFileName);
        }

        public SolrDIHStatus GetDIHStatus(KeyValuePair<string, string> options)
        {
            var operations = base.GetBasicOperations();
            return operations.GetDIHStatus(options);
        }

        public ResponseHeader Commit(CommitOptions options)
        {
            var operations = base.GetBasicOperations();
            return operations.Commit(options);
        }

        public ResponseHeader Optimize(CommitOptions options)
        {
            var operations = base.GetBasicOperations();
            return operations.Optimize(options);
        }

        public ResponseHeader Rollback()
        {
            var operations = base.GetBasicOperations();
            return operations.Rollback();
        }

        public ResponseHeader AddWithBoost(IEnumerable<KeyValuePair<T, double?>> docs, AddParameters parameters)
        {
            var operations = base.GetBasicOperations();
            return operations.AddWithBoost(docs, parameters);
        }

        public ExtractResponse Extract(ExtractParameters parameters)
        {
            var operations = base.GetBasicOperations();
            return operations.Extract(parameters);
        }

        public ResponseHeader Delete(IEnumerable<string> ids, ISolrQuery q, DeleteParameters parameters)
        {
            var operations = base.GetBasicOperations();
            return operations.Delete(ids, q, parameters);
        }

        public string Send(ISolrCommand cmd)
        {
            var operations = base.GetBasicOperations();
            return operations.Send(cmd);
        }

        public ResponseHeader SendAndParseHeader(ISolrCommand cmd)
        {
            var operations = base.GetBasicOperations();
            return operations.SendAndParseHeader(cmd);
        }

        public ExtractResponse SendAndParseExtract(ISolrCommand cmd)
        {
            var operations = base.GetBasicOperations();
            return operations.SendAndParseExtract(cmd);
        }

        public ResponseHeader AtomicUpdate(string uniqueKey, string id, IEnumerable<AtomicUpdateSpec> updateSpecs, AtomicUpdateParameters parameters)
        {
            var operations = base.GetBasicOperations();
            return operations.AtomicUpdate(uniqueKey, id, updateSpecs, parameters);
        }

        public Task<ResponseHeader> AtomicUpdateAsync(string uniqueKey, string id, IEnumerable<AtomicUpdateSpec> updateSpecs, AtomicUpdateParameters parameters)
        {
            var operations = base.GetBasicOperations();
            return operations.AtomicUpdateAsync(uniqueKey, id, updateSpecs, parameters);
        }

        public Task<ResponseHeader> CommitAsync(CommitOptions options)
        {
            var operations = base.GetBasicOperations();
            return operations.CommitAsync(options);
        }

        public Task<ResponseHeader> OptimizeAsync(CommitOptions options)
        {
            var operations = base.GetBasicOperations();
            return  operations.OptimizeAsync(options);
        }

        public Task<ResponseHeader> RollbackAsync()
        {
            var operations = base.GetBasicOperations();
            return operations.RollbackAsync();
        }

        public Task<ResponseHeader> AddWithBoostAsync(IEnumerable<KeyValuePair<T, double?>> docs, AddParameters parameters)
        {
            var operations = base.GetBasicOperations();
            return operations.AddWithBoostAsync(docs, parameters);
        }

        public Task<ExtractResponse> ExtractAsync(ExtractParameters parameters)
        {
            var operations = base.GetBasicOperations();
            return operations.ExtractAsync(parameters);
        }

        public Task<ResponseHeader> DeleteAsync(IEnumerable<string> ids, ISolrQuery q, DeleteParameters parameters)
        {
            var operations = base.GetBasicOperations();
            return operations.DeleteAsync(ids, q, parameters);
        }

        public Task<string> SendAsync(ISolrCommand cmd)
        {
            var operations = base.GetBasicOperations();
            return operations.SendAsync(cmd);
        }

        public Task<ResponseHeader> SendAndParseHeaderAsync(ISolrCommand cmd)
        {
            var operations = base.GetBasicOperations();
            return operations.SendAndParseHeaderAsync(cmd);
        }

        public Task<ExtractResponse> SendAndParseExtractAsync(ISolrCommand cmd)
        {
            var operations = base.GetBasicOperations();
            return operations.SendAndParseExtractAsync(cmd);
        }

        public Task<SolrQueryResults<T>> QueryAsync(ISolrQuery query, QueryOptions options, CancellationToken cancellationToken = default(CancellationToken))
        {
            var operations = base.GetBasicOperations();
            return operations.QueryAsync(query, options, cancellationToken);
        }

        public Task<SolrMoreLikeThisHandlerResults<T>> MoreLikeThisAsync(SolrMLTQuery query, MoreLikeThisHandlerQueryOptions options, CancellationToken cancellationToken = default(CancellationToken))
        {
            var operations = base.GetBasicOperations();
            return operations.MoreLikeThisAsync(query, options, cancellationToken);
        }

        public Task<ResponseHeader> PingAsync()
        {
            var operations = base.GetBasicOperations();
            return operations.PingAsync();
        }

        public Task<SolrSchema> GetSchemaAsync(string schemaFileName)
        {
            var operations = base.GetBasicOperations();
            return operations.GetSchemaAsync(schemaFileName);
        }

        public Task<SolrDIHStatus> GetDIHStatusAsync(KeyValuePair<string, string> options)
        {
            var operations = base.GetBasicOperations();
            return operations.GetDIHStatusAsync(options);
        }
    }