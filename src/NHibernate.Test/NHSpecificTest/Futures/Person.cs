using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.Futures
{
	public class Person
	{
		private IList<Person> children = new List<Person>();
		private IList<Person> friends = new List<Person>();
		private int id;
		private Person parent;

        public virtual string Name { get; set; }

		public virtual Person Parent
		{
			get { return parent; }
			set { parent = value; }
		}

		public virtual IList<Person> Friends
		{
			get { return friends; }
			set { friends = value; }
		}

		public virtual IList<Person> Children
		{
			get { return children; }
			set { children = value; }
		}

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}
	}
}