using System;

namespace NHibernate.Test.NHSpecificTest.NH276.JoinedSubclass
{
	public class Organization
	{
		private int _organizationId;
		private string _name;

		public int OrganizationId
		{
			get { return _organizationId; }
			set { _organizationId = value; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}
	}
}
