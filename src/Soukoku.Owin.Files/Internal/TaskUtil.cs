using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soukoku.Owin.Files.Internal
{
    static class TaskUtil
    {
        public static Task<TResult> FromResult<TResult>(TResult value)
        {
#if FX4
            var tcs = new TaskCompletionSource<TResult>();
            tcs.SetResult(value);
            return tcs.Task;
#else
            return Task.FromResult(value);
#endif
        }
    }
}
