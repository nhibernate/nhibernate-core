using System;

namespace NHibernate.Test.NHSpecificTest.GH2673
{
	[Serializable]
	public class Role
	{
		public virtual string Name { get; set; }
		public virtual int Key { get; set; }
	}
}
