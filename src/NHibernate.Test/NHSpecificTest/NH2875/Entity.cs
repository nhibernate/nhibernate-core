using System;

namespace NHibernate.Test.NHSpecificTest.NH2875
{
	class Entity
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
	}
}