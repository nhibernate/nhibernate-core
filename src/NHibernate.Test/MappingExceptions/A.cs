using System;
using System.Collections.Generic;

namespace NHibernate.Test.MappingExceptions
{
	/// <summary>
	/// Summary description for A.
	/// </summary>
	public class A
	{
		private int _id;
		private string _name;
		private ISet<object> _children = new HashSet<object>();

		public ISet<object> Children
		{
			get { return _children; }
			set { _children = value; }
		}

		private A()
		{
		}

		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}
	}
}