using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH1119
{
	public class TestClass
	{
		private int _ID;
		public virtual int ID
		{
			get { return _ID; }
			set { _ID = value; }
		}

		private DateTime _DateTimeProperty;
		public virtual DateTime DateTimeProperty
		{
			get { return _DateTimeProperty; }
			set { _DateTimeProperty = value; }
		}
	}
}
