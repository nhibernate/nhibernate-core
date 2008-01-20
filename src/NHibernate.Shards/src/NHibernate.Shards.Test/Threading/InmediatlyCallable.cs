using NHibernate.Shards.Threading;

namespace NHibernate.Shards.Test.Threading
{
	public class InmediatlyCallable :ICallable<int>
	{
		public InmediatlyCallable(int value)
		{
			this.value = value;
		}

		private int value;

		public int Call()
		{
			return this.value;
		}
	}
}