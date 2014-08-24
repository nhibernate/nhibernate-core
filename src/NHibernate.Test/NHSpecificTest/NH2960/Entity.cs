using System;

namespace NHibernate.Test.NHSpecificTest.NH2960
{
	class Entity
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
	}
}