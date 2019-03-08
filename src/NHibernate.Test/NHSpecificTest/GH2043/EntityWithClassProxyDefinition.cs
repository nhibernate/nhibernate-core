using System;

namespace NHibernate.Test.NHSpecificTest.GH2043
{
	public class EntityWithClassProxyDefinition
	{
		public virtual Guid Id { get; set; }

		public virtual string Name { get; set; }
	}
}
