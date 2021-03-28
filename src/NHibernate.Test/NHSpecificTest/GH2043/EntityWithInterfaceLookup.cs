using System;

namespace NHibernate.Test.NHSpecificTest.GH2043
{
	public class EntityWithInterfaceLookup
	{
		public virtual Guid Id { get; set; }

		public virtual string Name { get; set; }

		public virtual IEntityProxy EntityLookup { get; set; }
	}
}
