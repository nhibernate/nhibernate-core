using System;
using System.Collections;

namespace NHibernate.Cfg
{
	public interface ISecondPass
	{
		void DoSecondPass(IDictionary persistentClasses);
	}
}
