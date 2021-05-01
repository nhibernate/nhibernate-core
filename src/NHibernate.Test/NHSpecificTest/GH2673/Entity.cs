using System;

namespace NHibernate.Test.NHSpecificTest.GH2673
{
	[Serializable]
	public class Resource
	{
		public virtual int Id { get; set; }
		public virtual Resource Manager { get; set; }
		public virtual string Name { get; set; }
		public virtual Role ResourceRole { get; set; }
	}

	[Serializable]
	public class Role
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
	}
}
