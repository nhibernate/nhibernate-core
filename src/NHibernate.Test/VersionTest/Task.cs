using System;

namespace NHibernate.Test.VersionTest
{
	public class Task
	{
		private string description;
		private Person person;
		private int version;

		protected Task()
		{
		}

		public Task(string description, Person person)
		{
			this.description = description;
			this.person = person;
			person.Tasks.Add(this);
		}

		public virtual string Description
		{
			get { return description; }
			set { description = value; }
		}

		public virtual Person Person
		{
			get { return person; }
			set { person = value; }
		}

		public virtual int Version
		{
			get { return version; }
			set { version = value; }
		}
	}
}