using System;
using System.Collections;

namespace NHibernate.Test.NHSpecificTest.NH315
{
	public class Client
	{
		int _id;
		ClientPersons _contacts;

		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public ClientPersons Contacts
		{
			get { return _contacts; }
			set { _contacts = value; }
		}
	}
}
