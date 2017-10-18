using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NHibernate.Async
{
    public static class TaskExtensions
    {
		//https://github.com/dotnet/corefx/issues/2704
		/// <summary>
		/// Returns a Task that completes if the cancellationToken is cancelled
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public static Task WhenCanceled(this CancellationToken cancellationToken)
	    {
		    var tcs = new TaskCompletionSource<bool>();
		    cancellationToken.Register(s => ((TaskCompletionSource<bool>) s).SetResult(true), tcs);
		    return tcs.Task;
	    }
	}
}
