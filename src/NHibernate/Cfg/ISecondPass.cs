using System.Collections.Generic;

using NHibernate.Mapping;

namespace NHibernate.Cfg
{
	public interface ISecondPass
	{
		void DoSecondPass(IDictionary<System.Type, PersistentClass> persistentClasses);
	}
}