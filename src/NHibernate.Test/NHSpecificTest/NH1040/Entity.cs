namespace NHibernate.Test.NHSpecificTest.NH1040
{
	class Parent
	{
		public virtual int Id { get; set; }
		public virtual int UK { get; set; }
	}

	class Child : Parent
	{
		public virtual string Name { get; set; }
	}

	class Consumer
	{
		public virtual int Id { get; set; }
		public virtual Child Child { get; set; }
	}
}
