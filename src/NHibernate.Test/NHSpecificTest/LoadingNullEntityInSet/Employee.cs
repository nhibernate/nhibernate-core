using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.LoadingNullEntityInSet
{
	using Iesi.Collections.Generic;

	public class Employee
	{
		private int id;
		private ISet<PrimaryProfession> primaries = new HashedSet<PrimaryProfession>();
		private ISet<SecondaryProfession> secondaries = new HashedSet<SecondaryProfession>();

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
