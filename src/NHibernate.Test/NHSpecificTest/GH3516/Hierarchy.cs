using System;

namespace NHibernate.Test.NHSpecificTest.GH3516
{
	public class BaseClass
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
	}

	public class Subclass1 : BaseClass
	{
	}

	public class Subclass2 : BaseClass
	{
	}
}
