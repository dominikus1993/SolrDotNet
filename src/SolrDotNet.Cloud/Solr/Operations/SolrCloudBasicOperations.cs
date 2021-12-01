using SolrNet;
using SolrNet.Commands.Parameters;
using SolrNet.Impl;
using SolrNet.Schema;

namespace SolrDotNet.Cloud.Solr.Operations;

internal class SolrCloudBasicOperations<T> : ISolrBasicOperations<T>
{
    public ResponseHeader AddWithBoost(IEnumerable<KeyValuePair<T, double?>> docs, AddParameters parameters) => throw new NotImplementedException();
    public Task<ResponseHeader> AddWithBoostAsync(IEnumerable<KeyValuePair<T, double?>> docs, AddParameters parameters) => throw new NotImplementedException();
    public ResponseHeader AtomicUpdate(string uniqueKey, string id, IEnumerable<AtomicUpdateSpec> updateSpecs, AtomicUpdateParameters parameters) => throw new NotImplementedException();
    public Task<ResponseHeader> AtomicUpdateAsync(string uniqueKey, string id, IEnumerable<AtomicUpdateSpec> updateSpecs, AtomicUpdateParameters parameters) => throw new NotImplementedException();
    public ResponseHeader Commit(CommitOptions options) => throw new NotImplementedException();
    public Task<ResponseHeader> CommitAsync(CommitOptions options) => throw new NotImplementedException();
    public ResponseHeader Delete(IEnumerable<string> ids, ISolrQuery q, DeleteParameters parameters) => throw new NotImplementedException();
    public Task<ResponseHeader> DeleteAsync(IEnumerable<string> ids, ISolrQuery q, DeleteParameters parameters) => throw new NotImplementedException();
    public ExtractResponse Extract(ExtractParameters parameters) => throw new NotImplementedException();
    public Task<ExtractResponse> ExtractAsync(ExtractParameters parameters) => throw new NotImplementedException();
    public SolrDIHStatus GetDIHStatus(KeyValuePair<string, string> options) => throw new NotImplementedException();
    public Task<SolrDIHStatus> GetDIHStatusAsync(KeyValuePair<string, string> options) => throw new NotImplementedException();
    public SolrSchema GetSchema(string schemaFileName) => throw new NotImplementedException();
    public Task<SolrSchema> GetSchemaAsync(string schemaFileName) => throw new NotImplementedException();
    public SolrMoreLikeThisHandlerResults<T> MoreLikeThis(SolrMLTQuery query, MoreLikeThisHandlerQueryOptions options) => throw new NotImplementedException();
    public Task<SolrMoreLikeThisHandlerResults<T>> MoreLikeThisAsync(SolrMLTQuery query, MoreLikeThisHandlerQueryOptions options, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public ResponseHeader Optimize(CommitOptions options) => throw new NotImplementedException();
    public Task<ResponseHeader> OptimizeAsync(CommitOptions options) => throw new NotImplementedException();
    public ResponseHeader Ping() => throw new NotImplementedException();
    public Task<ResponseHeader> PingAsync() => throw new NotImplementedException();
    public SolrQueryResults<T> Query(ISolrQuery query, QueryOptions options) => throw new NotImplementedException();
    public Task<SolrQueryResults<T>> QueryAsync(ISolrQuery query, QueryOptions options, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public ResponseHeader Rollback() => throw new NotImplementedException();
    public Task<ResponseHeader> RollbackAsync() => throw new NotImplementedException();
    public string Send(ISolrCommand cmd) => throw new NotImplementedException();
    public ExtractResponse SendAndParseExtract(ISolrCommand cmd) => throw new NotImplementedException();
    public Task<ExtractResponse> SendAndParseExtractAsync(ISolrCommand cmd) => throw new NotImplementedException();
    public ResponseHeader SendAndParseHeader(ISolrCommand cmd) => throw new NotImplementedException();
    public Task<ResponseHeader> SendAndParseHeaderAsync(ISolrCommand cmd) => throw new NotImplementedException();
    public Task<string> SendAsync(ISolrCommand cmd) => throw new NotImplementedException();
}
