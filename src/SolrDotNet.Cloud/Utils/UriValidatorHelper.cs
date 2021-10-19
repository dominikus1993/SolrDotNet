using System;
using System.Runtime.CompilerServices;

namespace SolrDotNet.Cloud.Utils
{
    internal class UriValidatorHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int UriLength(UriBuilder u) =>
            u.Scheme.Length +
            3 +
            u.Host.Length +
            (u.Port <= 0 ? 0 : (u.Port.ToString().Length + 1)) +
            u.UserName.Length +
            (u.UserName.Length > 0 ? 1 : 0) +
            u.Password.Length +
            (u.Password.Length > 0 ? 1 : 0) +
            u.Path.Length +
            u.Query.Length +
            u.Fragment.Length;
    }
}
