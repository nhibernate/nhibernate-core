using System;
using NHibernate.Type;

namespace NHibernate.Test.NHSpecificTest.NH1159
{
	public class HibernateInterceptor : EmptyInterceptor
	{
		public static int CallCount = 0;
		public override bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, IType[] types)
		{
			CallCount++;
			return false;
		}
	}

}
