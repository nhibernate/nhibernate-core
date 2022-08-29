using System;

namespace NHibernate.Test.NHSpecificTest.NH266
{
	public class A
	{
		private int _id;
		private string _name;

		public A()
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
