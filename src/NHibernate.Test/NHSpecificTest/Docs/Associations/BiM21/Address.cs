using System;
using Iesi.Collections;

namespace NHibernate.Test.NHSpecificTest.Docs.Associations.BiM21
{
	public class Address
	{
		private ISet _people;
		private int _id;

		public Address()
		{
		}

		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public ISet People
		{
			get { return _people; }
			set { _people = value; }
		}
	}
}