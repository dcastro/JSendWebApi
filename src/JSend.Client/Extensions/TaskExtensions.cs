using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Runtime.CompilerServices;

namespace JSend.Client.Extensions
{
    internal static class TaskExtensions
    {
        internal static ConfiguredTaskAwaitable<T> IgnoreContext<T>(this Task<T> task)
        {
            return task.ConfigureAwait(false);
        }
    }
}
