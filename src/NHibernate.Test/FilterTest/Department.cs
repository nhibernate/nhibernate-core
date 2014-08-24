using System;
using System.Collections.Generic;

namespace NHibernate.Test.FilterTest
{
	public class Department
	{
		private long id;
		private string name;
		private ISet<Salesperson> salespersons = new HashSet<Salesperson>();

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

		public virtual ISet<Salesperson> Salespersons
		{
			get { return salespersons; }
			set { salespersons = value; }
		}
	}
}