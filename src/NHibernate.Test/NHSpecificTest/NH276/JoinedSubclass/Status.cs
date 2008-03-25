using System;

namespace NHibernate.Test.NHSpecificTest.NH276.JoinedSubclass
{
	public class Status
	{
		private int _statusId;
		private string _name;

		public int StatusId
		{
			get { return _statusId; }
			set { _statusId = value; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}
	}
}