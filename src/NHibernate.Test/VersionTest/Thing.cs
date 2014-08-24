using System;

namespace NHibernate.Test.VersionTest
{
	public class Thing
	{
		private string description;
		private Person person;
		private int version;
		private string longDescription;

		protected Thing()
		{
		}

		public Thing(String description, Person person)
		{
			this.description = description;
			this.person = person;
			person.Things.Add(this);
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

		public virtual string LongDescription
		{
			get { return longDescription; }
			set { longDescription = value; }
		}
	}
}