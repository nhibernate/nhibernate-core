using System;

namespace NHibernate.Test.NHSpecificTest.NH3961
{
	public class Entity
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual DateTime NonNullableDateTime { get; set; }
		public virtual DateTime? NullableDateTime { get; set; }
	}
}
