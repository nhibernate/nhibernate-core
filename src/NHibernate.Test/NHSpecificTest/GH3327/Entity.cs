using System;

namespace NHibernate.Test.NHSpecificTest.GH3327
{
	public class Entity
	{
		public virtual int Id { get; set; }
 		public virtual string Name { get; set; }
	}

	public class ChildEntity
	{
		public virtual int Id { get; set; }
		public virtual Entity Parent { get; set; }
		public virtual string Name { get; set; }
	}
}
