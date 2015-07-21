namespace NHibernate.Test.NHSpecificTest.NH2488
{
	public class Base1
	{
		public virtual int Id { get; set; }

		public virtual string ShortContent { get; set; }
	}

	public class Base2
	{
		public virtual int Id { get; set; }

		public virtual string ShortContent { get; set; }
	}

	public class Derived1 : Base1
	{
		public virtual string LongContent { get; set; }
	}

	public class Derived2 : Base2
	{
		public virtual string LongContent { get; set; }
	}

	public class Base3
	{
		public virtual int Id { get; set; }

		public virtual string ShortContent { get; set; }
	}
	public class Derived3 : Base3
	{
		public virtual string LongContent { get; set; }
	}
}