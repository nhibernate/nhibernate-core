using System.Collections.Generic;

namespace NHibernate.Test.Unionsubclass
{
	public class Hive
	{
		private long id;
		private Location location;
		private IList<Alien> members = new List<Alien>();

		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual Location Location
		{
			get { return location; }
			set { location = value; }
		}

		public virtual IList<Alien> Members
		{
			get { return members; }
			set { members = value; }
		}
	}
}