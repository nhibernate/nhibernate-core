namespace NHibernate.Test.NHSpecificTest.NH662
{
	public class Base
	{
		public virtual int Id { get; set; }
		public virtual string Description { get; set; }
		protected virtual void Foo()
		{
			// Some logic. 
		}
	}

	public class Derived : Base
	{
		protected override void Foo()
		{
			// Some other logic 
		}
	}
}