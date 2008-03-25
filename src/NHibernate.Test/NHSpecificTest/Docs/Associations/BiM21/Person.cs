using System;

namespace NHibernate.Test.NHSpecificTest.Docs.Associations.BiM21
{
	public class Person
	{
		private int _id;
		private Address _address;

		public Person()
		{
		}

		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public Address Address
		{
			get { return _address; }
			set { _address = value; }
		}
	}
}