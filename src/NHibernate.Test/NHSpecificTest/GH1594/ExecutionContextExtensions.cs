using System.Collections;
using System.Threading;

namespace NHibernate.Test.NHSpecificTest.GH1594
{
	public static class ExecutionContextExtensions
	{
		public static int? LocalValuesCount(this ExecutionContext c)
		{
#if NETFX
			const string localValuesFieldName = "_localValues";
#else			
			const string localValuesFieldName = "m_localValues";
#endif
			var f = typeof(ExecutionContext).GetField(localValuesFieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			// The property value may not implement IDictionary, especially when there is less than 4 values, but not only.
			// So we may not be able to know anything about this count.
			var d = f.GetValue(c) as IDictionary;
			return d?.Count;
		}
	}
}
