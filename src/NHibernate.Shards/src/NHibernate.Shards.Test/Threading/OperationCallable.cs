using System.Threading;
using NHibernate.Shards.Threading;

namespace NHibernate.Shards.Test.Threading
{
	public class OperationCallable<T> : ICallable<T>
	{
		protected int millis;
		protected T result;

		public OperationCallable(int millis, T result)
		{
			this.result = result;
			this.millis = millis;
		}

		#region ICallable<T> Members

		public virtual T Call()
		{
			Thread.Sleep(millis);
			return result;
		}

		#endregion
	}
}