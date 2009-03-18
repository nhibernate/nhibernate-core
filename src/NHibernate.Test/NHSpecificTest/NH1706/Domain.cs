using Iesi.Collections;

namespace NHibernate.Test.NHSpecificTest.NH1706
{
	public class A
	{
		public A()
		{
			Items = new HashedSet();
		}
		public int Id { get; set; }

		public string ExtraIdA { get; set; }

		public string Name { get; set; }

		public virtual ISet Items { get; set; }
	}

	public class B
	{
		public virtual int Id { get; set; }

		public virtual string Name { get; set; }

		public virtual string ExtraIdB { get; set; }
	}
}