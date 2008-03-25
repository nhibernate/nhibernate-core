using System;
using System.Collections;

namespace NHibernate.Test.MultipleCollectionFetchTest
{
	public class Person
	{
		private int id;
		private ICollection children;
		private ICollection friends;
		private Person parent;

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual ICollection Children
		{
			get { return children; }
			set { children = value; }
		}

		public virtual ICollection Friends
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
