using System;

namespace NHibernate.Test.NHSpecificTest.NH345
{
	public class Project
	{
		private int _id;
		private string _name;
		private Client _client;

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

		public Client Client
		{
			get { return _client; }
			set { _client = value; }
		}
	}
}