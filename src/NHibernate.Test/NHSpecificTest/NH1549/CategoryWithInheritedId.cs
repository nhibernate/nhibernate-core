using System;

namespace NHibernate.Test.NHSpecificTest.NH1549
{
	public class CategoryWithInheritedId : EntityInt32
	{
		public virtual string Name { get; set; }
	}

	public class CategoryWithId
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
	}
}