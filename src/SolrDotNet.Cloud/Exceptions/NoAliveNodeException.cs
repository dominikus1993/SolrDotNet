namespace SolrDotNet.Cloud.Exceptions;

public class NoAliveNodeException : Exception
{
    public NoAliveNodeException() : base("No Alive Node")
    {
        
    }

    public NoAliveNodeException(Exception innerException) : base("No Alive Node", innerException)
    {
        
    }
}