using SolrDotNet.Cloud.Zookeeper;

using SolrNet;
using SolrNet.Commands.Parameters;
using SolrNet.Impl;
using SolrNet.Mapping.Validation;
using SolrNet.Schema;

namespace SolrDotNet.Cloud.Operations;

    public class SolrCloudOperations<T> : SolrCloudOperationsBase<T>, ISolrOperations<T>
    {
        public SolrCloudOperations(ISolrCloudStateProvider cloudStateProvider, ISolrOperationsProvider operationsProvider)
            : base(cloudStateProvider, operationsProvider, isPostConnection: false) { }
        public SolrCloudOperations(ISolrCloudStateProvider cloudStateProvider, ISolrOperationsProvider operationsProvider, bool isPostConnection = false)
            : base(cloudStateProvider, operationsProvider, isPostConnection) { }

        public SolrCloudOperations(ISolrCloudStateProvider cloudStateProvider, ISolrOperationsProvider operationsProvider, string collectionName)
            : base(cloudStateProvider, operationsProvider, isPostConnection: false, collectionName: collectionName) { }
        public SolrCloudOperations(ISolrCloudStateProvider cloudStateProvider, ISolrOperationsProvider operationsProvider, string collectionName, bool isPostConnection = false)
            : base(cloudStateProvider, operationsProvider, isPostConnection, collectionName) { }

        public SolrQueryResults<T> Query(ISolrQuery query, QueryOptions options)
        {
            var operations = base.GetOperations();
            return operations.Query(query, options);
        }

        /// <inheritdoc />
        public SolrMoreLikeThisHandlerResults<T> MoreLikeThis(SolrMLTQuery query, MoreLikeThisHandlerQueryOptions options)
        {
            var operations = base.GetOperations();
            return operations.MoreLikeThis(query, options);
        }

        /// <inheritdoc />
        public ResponseHeader Ping()
        {
            var operations = base.GetOperations();
            return  operations.Ping();
        }

        /// <inheritdoc />
        public SolrSchema GetSchema(string schemaFileName)
        {
            var operations = base.GetOperations();
            return operations.GetSchema(schemaFileName);
        }

        /// <inheritdoc />
        public SolrDIHStatus GetDIHStatus(KeyValuePair<string, string> options)
        {
            var operations = base.GetOperations();
            return operations.GetDIHStatus(options);
        }

        /// <inheritdoc />
        public SolrQueryResults<T> Query(string q)
        {
            var operations = base.GetOperations();
            return operations.Query(q);
        }

        /// <inheritdoc />
        public SolrQueryResults<T> Query(string q, ICollection<SortOrder> orders)
        {
            var operations = base.GetOperations();
            return operations.Query(q, orders);
        }

        /// <inheritdoc />
        public SolrQueryResults<T> Query(string q, QueryOptions options)
        {
            var operations = base.GetOperations();
            return operations.Query(q, options);
        }

        /// <inheritdoc />
        public SolrQueryResults<T> Query(ISolrQuery q)
        {
            var operations = base.GetOperations();
            return operations.Query(q);
        }

        /// <inheritdoc />
        public SolrQueryResults<T> Query(ISolrQuery query, ICollection<SortOrder> orders)
        {
            var operations = base.GetOperations();
            return operations.Query(query, orders);
        }

        /// <inheritdoc />
        public ICollection<KeyValuePair<string, int>> FacetFieldQuery(SolrFacetFieldQuery facets)
        {
            var operations = base.GetOperations();
            return operations.FacetFieldQuery(facets);
        }

        /// <inheritdoc />
        public ResponseHeader Commit()
        {
            var operations = base.GetOperations(true);
            return operations.Commit();
        }

        /// <inheritdoc />
        public ResponseHeader Rollback()
        {
            var operations = base.GetOperations(true);
            return operations.Commit();
        }

        /// <inheritdoc />
        public ResponseHeader Optimize()
        {
            var operations = base.GetOperations(true);
            return operations.Commit();
        }

        /// <inheritdoc />
        public ResponseHeader Add(T doc)
        {
            var operations = base.GetOperations(true);
            return operations.Add(doc);
        }

        /// <inheritdoc />
        public ResponseHeader Add(T doc, AddParameters parameters)
        {
            var operations = base.GetOperations(true);
            return operations.Add(doc, parameters);
        }

        /// <inheritdoc />
        public ResponseHeader AddWithBoost(T doc, double boost)
        {
            var operations = base.GetOperations(true);
            return operations.AddWithBoost(doc, boost);
        }

        /// <inheritdoc />
        public ResponseHeader AddWithBoost(T doc, double boost, AddParameters parameters)
        {
            var operations = base.GetOperations(true);
            return operations.AddWithBoost(doc, boost, parameters);
        }

        /// <inheritdoc />
        public ExtractResponse Extract(ExtractParameters parameters)
        {
            var operations = base.GetOperations(true);
            return operations.Extract(parameters);
        }

        /// <inheritdoc />
        public ResponseHeader Add(IEnumerable<T> docs)
        {
            var operations = base.GetOperations(true);
            return operations.AddRange(docs);
        }

        /// <inheritdoc />
        public ResponseHeader AddRange(IEnumerable<T> docs)
        {
            var operations = base.GetOperations(true);
            return operations.AddRange(docs);
        }

        /// <inheritdoc />
        public ResponseHeader AddRange(IEnumerable<T> docs, AddParameters parameters)
        {
            var operations = base.GetOperations(true);
            return operations.AddRange(docs, parameters);
        }

        /// <inheritdoc />
        [Obsolete("Obsolete")]
        public ResponseHeader AddWithBoost(IEnumerable<KeyValuePair<T, double?>> docs)
        {
            var operations = base.GetOperations(true);
            return operations.AddWithBoost(docs);
        }

        /// <inheritdoc />
        public ResponseHeader AddRangeWithBoost(IEnumerable<KeyValuePair<T, double?>> docs)
        {
            var operations = base.GetOperations(true);
            return operations.AddRangeWithBoost(docs);
        }

        /// <inheritdoc />
        [Obsolete("Obsolete")]
        public ResponseHeader AddWithBoost(IEnumerable<KeyValuePair<T, double?>> docs, AddParameters parameters)
        {
            var operations = base.GetOperations(true);
            return operations.AddWithBoost(docs, parameters);
        }

        /// <inheritdoc />
        public ResponseHeader AddRangeWithBoost(IEnumerable<KeyValuePair<T, double?>> docs, AddParameters parameters)
        {
            var operations = base.GetOperations(true);
            return operations.AddRangeWithBoost(docs, parameters);
        }

        /// <inheritdoc />
        public ResponseHeader Delete(T doc)
        {
            var operations = base.GetOperations(true);
            return operations.Delete(doc);
        }

        /// <inheritdoc />
        public ResponseHeader Delete(T doc, DeleteParameters parameters)
        {
            var operations = base.GetOperations(true);
            return operations.Delete(doc, parameters);
        }

        /// <inheritdoc />
        public ResponseHeader Delete(IEnumerable<T> docs)
        {
            var operations = base.GetOperations(true);
            return operations.Delete(docs);
        }

        /// <inheritdoc />
        public ResponseHeader Delete(IEnumerable<T> docs, DeleteParameters parameters)
        {
            var operations = base.GetOperations(true);
            return operations.Delete(docs, parameters);
        }

        /// <inheritdoc />
        public ResponseHeader Delete(ISolrQuery q)
        {
            var operations = base.GetOperations(true);
            return operations.Delete(q);
        }

        /// <inheritdoc />
        public ResponseHeader Delete(ISolrQuery q, DeleteParameters parameters)
        {
            var operations = base.GetOperations(true);
            return operations.Delete(q, parameters);
        }

        /// <inheritdoc />
        public ResponseHeader Delete(string id)
        {
            var operations = base.GetOperations(true);
            return operations.Delete(id);
        }

        /// <inheritdoc />
        public ResponseHeader Delete(string id, DeleteParameters parameters)
        {
            var operations = base.GetOperations(true);
            return operations.Delete(id, parameters);
        }

        /// <inheritdoc />
        public ResponseHeader Delete(IEnumerable<string> ids)
        {
            var operations = base.GetOperations(true);
            return operations.Delete(ids);
        }

        /// <inheritdoc />
        public ResponseHeader Delete(IEnumerable<string> ids, DeleteParameters parameters)
        {
            var operations = base.GetOperations(true);
            return operations.Delete(ids, parameters);
        }

        /// <inheritdoc />
        public ResponseHeader Delete(IEnumerable<string> ids, ISolrQuery q)
        {
            var operations = base.GetOperations(true);
            return operations.Delete(ids, q);
        }

        /// <inheritdoc />
        public ResponseHeader Delete(IEnumerable<string> ids, ISolrQuery q, DeleteParameters parameters)
        {
            var operations = base.GetOperations(true);
            return operations.Delete(ids, q, parameters);
        }

        /// <inheritdoc />
        public ResponseHeader BuildSpellCheckDictionary()
        {
            var operations = base.GetOperations(true);
            return operations.BuildSpellCheckDictionary();
        }

        /// <inheritdoc />
        public IEnumerable<ValidationResult> EnumerateValidationResults()
        {
            var operations = base.GetOperations(true);
            return  operations.EnumerateValidationResults();
        }

        /// <inheritdoc />
        public ResponseHeader AtomicUpdate(T doc, IEnumerable<AtomicUpdateSpec> updateSpecs)
        {
            var operations = base.GetOperations(true);
            return operations.AtomicUpdate(doc, updateSpecs);
        }

        /// <inheritdoc />
        public ResponseHeader AtomicUpdate(string id, IEnumerable<AtomicUpdateSpec> updateSpecs)
        {
            var operations = base.GetOperations(true);
            return operations.AtomicUpdate(id, updateSpecs);
        }

        /// <inheritdoc />
        public ResponseHeader AtomicUpdate(T doc, IEnumerable<AtomicUpdateSpec> updateSpecs, AtomicUpdateParameters parameters)
        {
            var operations = base.GetOperations(true);
            return operations.AtomicUpdate(doc, updateSpecs, parameters);
        }

        /// <inheritdoc />
        public ResponseHeader AtomicUpdate(string id, IEnumerable<AtomicUpdateSpec> updateSpecs, AtomicUpdateParameters parameters)
        {
            var operations = base.GetOperations(true);
            return operations.AtomicUpdate(id, updateSpecs, parameters);
        }

        /// <inheritdoc />
        public Task<ResponseHeader> CommitAsync()
        {
            var operations = base.GetOperations();
            return operations.CommitAsync();
        }

        /// <inheritdoc />
        public Task<ResponseHeader> RollbackAsync()
        {
            var operations = base.GetOperations();
            return operations.RollbackAsync();
        }

        /// <inheritdoc />
        public Task<ResponseHeader> OptimizeAsync()
        {
            var operations = base.GetOperations();
            return operations.OptimizeAsync();
        }


        /// <inheritdoc />
        public Task<ResponseHeader> AddAsync(T doc)
        {
            var operations = base.GetOperations();
            return operations.AddAsync(doc);
        }

        /// <inheritdoc />
        public Task<ResponseHeader> AddAsync(T doc, AddParameters parameters)
        {
            var operations = base.GetOperations();
            return operations.AddAsync(doc, parameters);
        }


        /// <inheritdoc />
        public Task<ResponseHeader> AddWithBoostAsync(T doc, double boost)
        {
            var operations = base.GetOperations();
            return operations.AddWithBoostAsync(doc, boost);
        }

        /// <inheritdoc />
        public Task<ResponseHeader> AddWithBoostAsync(T doc, double boost, AddParameters parameters)
        {
            var operations = base.GetOperations();
            return operations.AddWithBoostAsync(doc, boost, parameters);
        }

        /// <inheritdoc />
        public Task<ExtractResponse> ExtractAsync(ExtractParameters parameters)
        {
            var operations = base.GetOperations();
            return operations.ExtractAsync(parameters);
        }

        /// <inheritdoc />
        public Task<ResponseHeader> AddRangeAsync(IEnumerable<T> docs)
        {
            var operations = base.GetOperations();
            return operations.AddRangeAsync(docs);
        }

        /// <inheritdoc />
        public Task<ResponseHeader> AddRangeAsync(IEnumerable<T> docs, AddParameters parameters)
        {
            var operations = base.GetOperations();
            return operations.AddRangeAsync(docs, parameters);
        }

        /// <inheritdoc />
        public Task<ResponseHeader> AddRangeWithBoostAsync(IEnumerable<KeyValuePair<T, double?>> docs)
        {
            var operations = base.GetOperations();
            return operations.AddRangeWithBoostAsync(docs);
        }

        /// <inheritdoc />
        public Task<ResponseHeader> AddRangeWithBoostAsync(IEnumerable<KeyValuePair<T, double?>> docs, AddParameters parameters)
        {
            var operations = base.GetOperations();
            return operations.AddRangeWithBoostAsync(docs, parameters);
        }

        /// <inheritdoc />
        public Task<ResponseHeader> DeleteAsync(T doc)
        {
            var operations = base.GetOperations();
            return operations.DeleteAsync(doc);
        }

        /// <inheritdoc />
        public Task<ResponseHeader> DeleteAsync(T doc, DeleteParameters parameters)
        {
            var operations = base.GetOperations();
            return operations.DeleteAsync(doc, parameters);
        }

        /// <inheritdoc />
        public Task<ResponseHeader> DeleteAsync(IEnumerable<T> docs)
        {
            var operations = base.GetOperations();
            return operations.DeleteAsync(docs);
        }

        /// <inheritdoc />
        public Task<ResponseHeader> DeleteAsync(IEnumerable<T> docs, DeleteParameters parameters)
        {
            var operations = base.GetOperations();
            return operations.DeleteAsync(docs, parameters);
        }

        /// <inheritdoc />
        public Task<ResponseHeader> DeleteAsync(ISolrQuery q)
        {
            var operations = base.GetOperations();
            return operations.DeleteAsync(q);
        }

        /// <inheritdoc />
        public Task<ResponseHeader> DeleteAsync(ISolrQuery q, DeleteParameters parameters)
        {
            var operations = base.GetOperations();
            return operations.DeleteAsync(q, parameters);
        }

        /// <inheritdoc />
        public Task<ResponseHeader> DeleteAsync(string id)
        {
            var operations = base.GetOperations();
            return operations.DeleteAsync(id);
        }

        /// <inheritdoc />
        public Task<ResponseHeader> DeleteAsync(string id, DeleteParameters parameters)
        {
            var operations = base.GetOperations();
            return operations.DeleteAsync(id, parameters);
        }

        /// <inheritdoc />
        public Task<ResponseHeader> DeleteAsync(IEnumerable<string> ids)
        {
            var operations = base.GetOperations();
            return operations.DeleteAsync(ids);
        }

        /// <inheritdoc />
        public Task<ResponseHeader> DeleteAsync(IEnumerable<string> ids, DeleteParameters parameters)
        {
            var operations = base.GetOperations();
            return operations.DeleteAsync(ids, parameters);
        }

        /// <inheritdoc />
        public Task<ResponseHeader> DeleteAsync(IEnumerable<string> ids, ISolrQuery q)
        {
            var operations = base.GetOperations();
            return operations.DeleteAsync(ids, q);
        }

        /// <inheritdoc />
        public Task<ResponseHeader> DeleteAsync(IEnumerable<string> ids, ISolrQuery q, DeleteParameters parameters)
        {
            var operations = base.GetOperations();
            return operations.DeleteAsync(ids, q, parameters);
        }

        /// <inheritdoc />
        public Task<ResponseHeader> BuildSpellCheckDictionaryAsync()
        {
            var operations = base.GetOperations();
            return operations.BuildSpellCheckDictionaryAsync();
        }

        /// <inheritdoc />
        public Task<IEnumerable<ValidationResult>> EnumerateValidationResultsAsync()
        {
            var operations = base.GetOperations();
            return  operations.EnumerateValidationResultsAsync();
        }

        /// <inheritdoc />
        public Task<SolrQueryResults<T>> QueryAsync(string q, CancellationToken cancellationToken = default)
        {
            var operations = base.GetOperations();
            return operations.QueryAsync(q, cancellationToken);
        }

        /// <inheritdoc />
        public Task<SolrQueryResults<T>> QueryAsync(string q, ICollection<SortOrder> orders, CancellationToken cancellationToken = default)
        {
            var operations = base.GetOperations();
            return operations.QueryAsync(q, orders, cancellationToken);
        }

        /// <inheritdoc />
        public Task<SolrQueryResults<T>> QueryAsync(string q, QueryOptions options, CancellationToken cancellationToken = default)
        {
            var operations = base.GetOperations();
            return operations.QueryAsync(q, options, cancellationToken);
        }

        /// <inheritdoc />
        public Task<SolrQueryResults<T>> QueryAsync(ISolrQuery q, CancellationToken cancellationToken = default)
        {
            var operations = base.GetOperations();
            return operations.QueryAsync(q, cancellationToken);
        }

        /// <inheritdoc />
        public Task<SolrQueryResults<T>> QueryAsync(ISolrQuery query, ICollection<SortOrder> orders, CancellationToken cancellationToken = default)
        {
            var operations = base.GetOperations();
            return operations.QueryAsync(query, orders, cancellationToken);
        }

        /// <inheritdoc />
        public Task<ICollection<KeyValuePair<string, int>>> FacetFieldQueryAsync(SolrFacetFieldQuery facets)
        {
            var operations = base.GetOperations();
            return operations.FacetFieldQueryAsync(facets);
        }

        /// <inheritdoc />
        public Task<SolrQueryResults<T>> QueryAsync(ISolrQuery query, QueryOptions options, CancellationToken cancellationToken = default)
        {
            var operations = base.GetOperations();
            return operations.QueryAsync(query, options, cancellationToken);
        }

        /// <inheritdoc />
        public Task<SolrMoreLikeThisHandlerResults<T>> MoreLikeThisAsync(SolrMLTQuery query, MoreLikeThisHandlerQueryOptions options, CancellationToken cancellationToken = default)
        {
            var operations = base.GetOperations();
            return operations.MoreLikeThisAsync(query, options);
        }

        /// <inheritdoc />
        public Task<ResponseHeader> PingAsync()
        {
            var operations = base.GetOperations();
            return operations.PingAsync();
        }

        /// <inheritdoc />
        public Task<SolrSchema> GetSchemaAsync(string schemaFileName)
        {
            var operations = base.GetOperations();
            return operations.GetSchemaAsync(schemaFileName);
        }

        /// <inheritdoc />
        public Task<SolrDIHStatus> GetDIHStatusAsync(KeyValuePair<string, string> options)
        {
            var operations = base.GetOperations();
            return operations.GetDIHStatusAsync(options);
        }

        /// <inheritdoc />
        public Task<ResponseHeader> AtomicUpdateAsync(T doc, IEnumerable<AtomicUpdateSpec> updateSpecs)
        {
            var operations = base.GetOperations();
            return operations.AtomicUpdateAsync(doc, updateSpecs);
        }

        /// <inheritdoc />
        public Task<ResponseHeader> AtomicUpdateAsync(string id, IEnumerable<AtomicUpdateSpec> updateSpecs)
        {
            var operations = base.GetOperations();
            return operations.AtomicUpdateAsync(id, updateSpecs);
        }

        /// <inheritdoc />
        public Task<ResponseHeader> AtomicUpdateAsync(T doc, IEnumerable<AtomicUpdateSpec> updateSpecs, AtomicUpdateParameters parameters)
        {
            var operations = base.GetOperations();
            return operations.AtomicUpdateAsync(doc, updateSpecs, parameters);
        }

        /// <inheritdoc />
        public Task<ResponseHeader> AtomicUpdateAsync(string id, IEnumerable<AtomicUpdateSpec> updateSpecs, AtomicUpdateParameters parameters)
        {
            var operations = base.GetOperations();
            return operations.AtomicUpdateAsync(id, updateSpecs, parameters);
        }

        /// <inheritdoc />
        public IEnumerable<ValidationResult> EnumerateValidationResults(string schemaFileName)
        {
            var operations = base.GetOperations();
            return operations.EnumerateValidationResults(schemaFileName);
        }

        /// <inheritdoc />
        public Task<IEnumerable<ValidationResult>> EnumerateValidationResultsAsync(string schemaFileName)
        {
            var operations = base.GetOperations();
            return operations.EnumerateValidationResultsAsync(schemaFileName);
        }
    }