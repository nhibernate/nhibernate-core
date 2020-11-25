using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.GH2614
{
	public abstract class BaseClass
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
	}

	public class ConcreteClass1 : BaseClass
	{
	}

	public class ConcreteClass2 : BaseClass
	{
	}
}
