using System;

namespace NHibernate.Test.NHSpecificTest.GH2820
{
	class Entity
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual DateTimeOffset Timestamp { get; set; }
	}
}
