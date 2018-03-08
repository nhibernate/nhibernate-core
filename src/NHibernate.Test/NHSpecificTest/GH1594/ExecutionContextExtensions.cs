using System.Collections;
using System.Threading;

namespace NHibernate.Test.NHSpecificTest.GH1594
{
	public static class ExecutionContextExtensions
	{
		public static int LocalValuesCount(this ExecutionContext c)
		{
#if NETFX
			const string localValuesFieldName = "_localValues";
#else			
			const string localValuesFieldName = "m_localValues";
#endif
			var f = typeof(ExecutionContext).GetField(localValuesFieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			var d = (IDictionary) f.GetValue(c);
			return d?.Count ?? 0;
		}
	}
}
