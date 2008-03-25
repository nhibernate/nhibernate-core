using System;

namespace NHibernate.Test.NHSpecificTest.NH315
{
	public class Person
	{
		private int _id;

		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}
	}
}