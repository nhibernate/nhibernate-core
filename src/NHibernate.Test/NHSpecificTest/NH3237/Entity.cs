using System;

namespace NHibernate.Test.NHSpecificTest.NH3237
{
	class Entity
	{
		public virtual Guid Id { get; set; }
		public virtual DateTimeOffset DateTimeOffset { get; set; }
		public virtual TestEnum TestEnum { get; set; }
	}

	public enum TestEnum
	{
		Zero = 0,
		One = 1,
		Two = 2
	}
}