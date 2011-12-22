using System;

namespace NHibernate.Test.NHSpecificTest.NH2722
{
	public class Entity
	{
		public virtual Guid Id { get; set; }
		public virtual int? Property { get; set; }
	}
}
