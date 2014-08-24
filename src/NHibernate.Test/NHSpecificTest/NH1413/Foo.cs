using System;

namespace NHibernate.Test.NHSpecificTest.NH1413
{
	public class Foo
	{
		private DateTime birthDate;
		private string name;
		private long oid;
		private int version;

		public Foo() {}

		public Foo(string name, DateTime birthDate)
		{
			this.name = name;
			this.birthDate = birthDate;
		}

		public virtual long Oid
		{
			get { return oid; }
			set { oid = value; }
		}

		public virtual int Version
		{
			get { return version; }
			set { version = value; }
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		public virtual DateTime BirthDate
		{
			get { return birthDate; }
			set { birthDate = value; }
		}
	}
}