using System;

namespace NHibernate.Test.DialectTest.SchemaTests
{
	public class Entity
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual string Name1 { get; set; }
		public virtual string Name2 { get; set; }
		public virtual Entity Parent { get; set; }
	}
}
