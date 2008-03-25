using System;

namespace NHibernate.DomainModel
{
	/// <summary>
	/// Summary description for Immutable.
	/// </summary>
	public class Immutable
	{
		private string _foo;
		private string _bar;
		private string _id;

		public string Foo
		{
			get { return _foo; }
			set { _foo = value; }
		}

		public string Bar
		{
			get { return _bar; }
			set { _bar = value; }
		}

		public string Id
		{
			get { return _id; }
			set { _id = value; }
		}
	}
}