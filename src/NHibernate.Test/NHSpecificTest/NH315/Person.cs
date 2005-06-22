using System;
using System.Collections;

namespace NHibernate.Test.NHSpecificTest.NH315
{
	public class Person
	{
		int _id;

		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}
	}
}
