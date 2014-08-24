using System;

namespace NHibernate.Test.SqlTest
{
	public class Person
	{
		private long id;
		private string name;

		public Person(String name)
		{
			this.name = name;
		}

		public Person()
		{
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
	}
}