namespace SolrDotNet.Cloud.Exceptions;

public class CollectionOrAliasNotFoundException : Exception
{
    public string CollectionName { get; }

    public CollectionOrAliasNotFoundException(string collectionName) : base()
    {
        CollectionName = collectionName;
    }

    public CollectionOrAliasNotFoundException(string? message, string collectionName) : base(message)
    {
        CollectionName = collectionName;
    }

    public CollectionOrAliasNotFoundException(string? message, Exception? innerException, string collectionName) :
        base(message, innerException)
    {
        CollectionName = collectionName;
    }

    public override string ToString()
    {
        return $"Collection or alias not found: {CollectionName}, {base.ToString()}";
    }
}