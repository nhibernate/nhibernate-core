using System;

namespace NHibernate.Test.NHSpecificTest.NH276
{
	public class Building
	{
		private int _id;
		private string _number;

		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public string Number
		{
			get { return _number; }
			set { _number = value; }
		}
	}
}