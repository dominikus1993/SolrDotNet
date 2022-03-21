namespace SolrDotNet.Cloud.Exceptions;

public class NoAppropriateNodeWasSelectedException : Exception
{
    public NoAppropriateNodeWasSelectedException() : base("No appropriate node was selected to perform the operation.")
    {
        
    }
}