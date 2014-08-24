using System.Collections.Generic;

namespace NHibernate.Test.Deletetransient
{
	public class Person
	{
		private long id;
		private string name;
		private ISet<Address> addresses = new HashSet<Address>();
		private IList<Person> friends = new List<Person>();
		public Person() {}
		public Person(string name)
		{
			this.name = name;
		}

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

		public virtual ISet<Address> Addresses
		{
			get { return addresses; }
			set { addresses = value; }
		}

		public virtual IList<Person> Friends
		{
			get { return friends; }
			set { friends = value; }
		}
	}
}