using System;

namespace NHibernate.Test.NHSpecificTest.NH2167
{
	class Entity
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
	}
}