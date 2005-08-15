using System;
using System.Collections;

namespace NHibernate.Test.NHSpecificTest.NH345
{
	public class Client
	{
		private int _id;
		private string _name;

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
