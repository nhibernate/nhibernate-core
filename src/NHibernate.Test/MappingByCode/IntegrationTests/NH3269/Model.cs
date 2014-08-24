using System;

namespace NHibernate.Test.MappingByCode.IntegrationTests.NH3269
{
	public abstract class Base
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
	}

	public class Inherited1 : Base
	{
		public virtual string F1 { get; set; }
	}

	public class Inherited2 : Base
	{
		public virtual string F2 { get; set; }
	}
}
