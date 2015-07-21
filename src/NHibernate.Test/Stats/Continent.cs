using System.Collections.Generic;

namespace NHibernate.Test.Stats
{
	public class Continent
	{
		private int id;
		private string name;
		private ISet<Country> countries;

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		public virtual ISet<Country> Countries
		{
			get { return countries; }
			set { countries = value; }
		}
	}
}
