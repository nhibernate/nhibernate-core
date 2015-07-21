using System;
using System.Collections.Generic;

namespace NHibernate.Test.MultipleCollectionFetchTest
{
	public class Person
	{
		private int id;
		private ICollection<Person> children;
		private ICollection<Person> friends;
		private Person parent;

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual ICollection<Person> Children
		{
			get { return children; }
			set { children = value; }
		}

		public virtual ICollection<Person> Friends
		{
			get { return friends; }
			set { friends = value; }
		}

		public virtual Person Parent
		{
			get { return parent; }
			set { parent = value; }
		}
	}
}
