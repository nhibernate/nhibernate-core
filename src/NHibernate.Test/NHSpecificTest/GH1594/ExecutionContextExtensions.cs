using System.Collections;
using System.Threading;

namespace NHibernate.Test.NHSpecificTest.GH1594
{
	public static class ExecutionContextExtensions
	{
		public static int LocalValuesCount(this ExecutionContext c)
		{
			var f = typeof(ExecutionContext).GetField("_localValues", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			var d = (IDictionary) f.GetValue(c);
			return d?.Count ?? 0;
		}
	}
}
