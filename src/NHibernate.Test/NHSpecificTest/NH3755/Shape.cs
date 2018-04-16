using System;

namespace NHibernate.Test.NHSpecificTest.NH3755
{
	public abstract class Shape : IShape
	{
		public virtual Guid Id { get; set; }
		public string Property1 { get; set; }
		public abstract string Property2 { get; set; }
	}
}
