using System;

namespace NHibernate.Test.NHSpecificTest.IlogsProxyTest
{
	public class EntityWithInterfaceLookup
	{
		public virtual Guid Id { get; set; }

		public virtual string Name { get; set; }

		public virtual IEntityProxy EntityLookup { get; set; }
	}
}
