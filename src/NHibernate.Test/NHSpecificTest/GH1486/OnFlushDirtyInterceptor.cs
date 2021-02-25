using NHibernate.Type;

namespace NHibernate.Test.NHSpecificTest.GH1486
{
	public class OnFlushDirtyInterceptor : EmptyInterceptor
	{
		public int CallCount = 0;

		public override bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, IType[] types)
		{
			CallCount++;
			return false;
		}

		public void Reset()
		{
			CallCount = 0;
		}
	}
}
