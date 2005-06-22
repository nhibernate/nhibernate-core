using System;
using System.Collections;

namespace NHibernate.Test.NHSpecificTest.NH315
{
	public class ClientPersons
	{
		private int _personId;
		private Person _person;

		public int PersonId
		{
			get { return _personId; }
			set { _personId = value; }
		}

		public Person Person
		{
			get { return _person; }
			set { _person = value; }
		}
	}
}
