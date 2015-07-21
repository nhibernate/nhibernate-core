using System;

namespace NHibernate.Test.SubclassFilterTest
{
	public class Person
	{
		private long id;
		private string name;
		private string company;
		private string region;

		public Person()
		{
		}

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

		public virtual string Company
		{
			get { return company; }
			set { company = value; }
		}

		public virtual string Region
		{
			get { return region; }
			set { region = value; }
		}
	}
}