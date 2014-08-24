namespace NHibernate.Test.NHSpecificTest.NH1403
{
	public class Person
	{
		private int id;
		private string name;

		public Person() {}

		public Person(string name) : this()
		{
			this.name = name;
		}

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}
	}
}