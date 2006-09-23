using System;
using Iesi.Collections;

namespace NHibernate.Test.FilterTest
{
	public class Department
	{
		private long id;
		private string name;
		private ISet salespersons = new HashedSet();

		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		public virtual ISet Salespersons
		{
			get { return salespersons; }
			set { salespersons = value; }
		}
	}
}
