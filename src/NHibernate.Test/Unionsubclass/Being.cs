using System.Collections;

namespace NHibernate.Test.Unionsubclass
{
	public abstract class Being
	{
		private long id;
		private string identity;
		private Location location;
		private IList things = new ArrayList();
		private IList info = new ArrayList();

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

		public virtual IList Things
		{
			get { return things; }
			set { things = value; }
		}

		public virtual IList Info
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