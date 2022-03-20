using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("SolrDotNet.Cloud.AspNetCore")]

namespace SolrDotNet.Cloud.Extensions;

internal static class AsyncHelper
{
    private static readonly TaskFactory TaskFactory = new(CancellationToken.None,
        TaskCreationOptions.None,
        TaskContinuationOptions.None,
        TaskScheduler.Default);

    public static TResult RunSync<TResult>(Func<Task<TResult>> func)
        => TaskFactory
            .StartNew(func)
            .Unwrap()
            .GetAwaiter()
            .GetResult();

    public static void RunSync(Func<Task> func)
        => TaskFactory
            .StartNew(func)
            .Unwrap()
            .GetAwaiter()
            .GetResult();
}

internal static class TaskExt
{
    internal static async Task<(T1, T2)> WhenAll<T1, T2>(Task<T1> task1, Task<T2> task2,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await Task.WhenAll(task1, task2).ConfigureAwait(false);
        return (task1.Result, task2.Result);
    }
}