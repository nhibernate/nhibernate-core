using System;

namespace NHibernate.Test.NHSpecificTest.GH2673
{
	[Serializable]
	public class Resource
	{
		public virtual int Key { get; set; }
		public virtual Resource Manager { get; set; }
		public virtual string Name { get; set; }
		public virtual Role ResourceRole { get; set; }
	}
}
