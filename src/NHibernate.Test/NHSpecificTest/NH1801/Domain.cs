namespace NHibernate.Test.NHSpecificTest.NH1801
{
	public class A
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
	}

	public class B
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }

		public virtual A A { get; set; }
	}

	public class C
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }

		public virtual A A { get; set; }
	}
}