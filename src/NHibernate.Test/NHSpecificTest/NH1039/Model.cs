using System;
using System.Collections;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH1039
{
	public class Person
	{
		public Person() { }
		public Person(string id) { this._ID = id; }

		private string _ID;
		public virtual string ID
		{
			get { return _ID; }
			set { _ID = value; }
		}

		private string _Name;
		public virtual string Name
		{
			get { return _Name; }
			set { _Name = value; }
		}

		private IDictionary _Properties = new Hashtable();
		public virtual IDictionary Properties
		{
			get { return _Properties; }
			set { _Properties = value; }
		}
	}
}
