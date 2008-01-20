using System.Threading;

namespace NHibernate.Shards.Test.Threading
{
	public class FooCallable1 : OperationCallable<string>
	{
		public FooCallable1(): base(1000, "FooCallable1" )
		{
		}
	}
}