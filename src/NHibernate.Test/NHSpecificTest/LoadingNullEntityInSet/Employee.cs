using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.LoadingNullEntityInSet
{
	
	public class Employee
	{
		private int id;
		private ISet<PrimaryProfession> primaries = new HashSet<PrimaryProfession>();
		private ISet<SecondaryProfession> secondaries = new HashSet<SecondaryProfession>();

		public ISet<PrimaryProfession> Primaries
		{
			get { return primaries; }
			set { primaries = value; }
		}

		public ISet<SecondaryProfession> Secondaries
		{
			get { return secondaries; }
			set { secondaries = value; }
		}

		public int Id
		{
			get { return id; }
			set { id = value; }
		}
	}
}
