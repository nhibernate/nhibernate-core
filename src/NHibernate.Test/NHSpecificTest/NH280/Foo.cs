using System;

namespace NHibernate.Test.NHSpecificTest.NH280
{
	public class Foo
	{
		public Foo()
		{
		}

		public Foo(string description) : base()
		{
			_description = description;
		}

		private int _id;

		public virtual int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		private string _description;

		public virtual string Description
		{
			get { return _description; }
			set { _description = value; }
		}

	}
}