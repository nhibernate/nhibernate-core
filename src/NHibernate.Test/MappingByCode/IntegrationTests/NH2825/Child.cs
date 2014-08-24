using System;

namespace NHibernate.Test.MappingByCode.IntegrationTests.NH2825
{
	public class Child
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual Parent Parent { get; set; }
	}
}