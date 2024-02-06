using System;

namespace NHibernate.Test.NHSpecificTest.GH3403OneToOne
{
	public class Entity1
	{
		public virtual Guid Id { get; set; }

		public virtual Entity2 Child { get; set; }
	}
	public class Entity2
	{
		public virtual Guid Id { get; set; }

		public virtual Entity1 Parent { get; set; }
	}
}
