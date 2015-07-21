using System;

namespace NHibernate.Test.NHSpecificTest.NH1549
{
	public class ProductWithInheritedId : EntityInt32
	{
		public CategoryWithInheritedId CategoryWithInheritedId { get; set; }
	}

	public class ProductWithId
	{
		public virtual int Id { get; set; }
		public CategoryWithId CategoryWithId { get; set; }
	}
}