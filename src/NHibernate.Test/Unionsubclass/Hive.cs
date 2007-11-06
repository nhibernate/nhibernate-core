using System.Collections;

namespace NHibernate.Test.Unionsubclass
{
	public class Hive
	{
		private long id;
		private Location location;
		private IList members = new ArrayList();

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

		public virtual IList Members
		{
			get { return members; }
			set { members = value; }
		}
	}
}