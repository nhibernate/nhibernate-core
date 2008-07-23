using Iesi.Collections;

namespace NHibernate.Test.Any
{
	public class Address
	{
		private long id;
		private ISet lines = new HashedSet();

		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual ISet Lines
		{
			get { return lines; }
			set { lines = value; }
		}
	}
}