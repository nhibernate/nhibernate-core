using System;
using NHibernate.Type;
namespace NHibernate.Test.Interceptor
{
	public class PropertyInterceptor : EmptyInterceptor
	{
		public override bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, IType[] types)
		{
			currentState[1] = DateTime.Now;
			return true;
		}

		public override bool OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types)
		{
			state[2] = DateTime.Now;
			return true;
		}
	}
}