using System.Collections.Generic;

namespace NHibernate.Test.Unionsubclass
{
	public abstract class Being
	{
		private long id;
		private string identity;
		private Location location;
		private IList<Thing> things = new List<Thing>();
		private IList<string> info = new List<string>();

		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual string Identity
		{
			get { return identity; }
			set { identity = value; }
		}

		public virtual Location Location
		{
			get { return location; }
			set { location = value; }
		}

		public virtual IList<Thing> Things
		{
			get { return things; }
			set { things = value; }
		}

		public virtual IList<string> Info
		{
			get { return info; }
			set { info = value; }
		}

		public virtual string Species
		{
			get { return null; }
			set { throw new System.NotSupportedException(); }
		}

	}
}