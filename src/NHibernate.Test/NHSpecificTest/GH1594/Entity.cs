using System;

namespace NHibernate.Test.NHSpecificTest.GH1594
{
	class Entity
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
	}
}
