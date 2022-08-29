using System;

namespace NHibernate.Test.NHSpecificTest.NH3247
{
	class Entity
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual Char Initial { get; set; }
	}
}
