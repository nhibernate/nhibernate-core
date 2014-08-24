namespace NHibernate.Test.Unionsubclass2
{
	public class Person
	{
		private long id;
		private string name;
		private char sex;
		private Address address = new Address();

		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		public virtual char Sex
		{
			get { return sex; }
			set { sex = value; }
		}

		public virtual Address Address
		{
			get { return address; }
		}

	}
}
