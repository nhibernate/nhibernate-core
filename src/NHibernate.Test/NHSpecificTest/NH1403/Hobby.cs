namespace NHibernate.Test.NHSpecificTest.NH1403
{
	public class Hobby
	{
		private int id;
		private string name;
		private Person person;

		public Hobby() {}

		public Hobby(string name) : this()
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

		public virtual Person Person
		{
			get { return person; }
			set { person = value; }
		}
	}
}