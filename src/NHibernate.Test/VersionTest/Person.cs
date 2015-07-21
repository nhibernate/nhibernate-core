using System;
using System.Collections.Generic;

namespace NHibernate.Test.VersionTest
{
	public class Person
	{
		private string name;
		private IList<Thing> things;
		private IList<Task> tasks;
		private int version;

		protected Person()
		{
		}

		public Person(string name)
		{
			this.name = name;
			this.things = new List<Thing>();
			this.tasks = new List<Task>();
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		public virtual IList<Thing> Things
		{
			get { return things; }
			set { things = value; }
		}

		public virtual IList<Task> Tasks
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