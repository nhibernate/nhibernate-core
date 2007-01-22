using System;
using System.Collections;

namespace NHibernate.Test.VersionTest
{
	public class Person
	{
		private string name;
		private IList things;
		private IList tasks;
		private int version;

		protected Person()
		{
		}

		public Person(string name)
		{
			this.name = name;
			this.things = new ArrayList();
			this.tasks = new ArrayList();
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		public virtual IList Things
		{
			get { return things; }
			set { things = value; }
		}

		public virtual IList Tasks
		{
			get { return tasks; }
			set { tasks = value; }
		}

		public virtual int Version
		{
			get { return version; }
			set { version = value; }
		}
	}
}
