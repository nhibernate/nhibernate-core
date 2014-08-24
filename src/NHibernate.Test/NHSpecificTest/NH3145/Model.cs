
namespace NHibernate.Test.NHSpecificTest.NH3145
{
	public class Root
	{
		public virtual int Id { get; set; }
		public virtual Base Base { get; set; }
	}

	public class Base
	{
		public virtual int Id { get; set; }
		public virtual string LongContent { get; set; }
	}

	public class Derived : Base
	{
	}
}
