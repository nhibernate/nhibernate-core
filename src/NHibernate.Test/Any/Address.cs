using System.Collections.Generic;

namespace NHibernate.Test.Any
{
	public class Address
	{
		private long id;
		private ISet<string> lines = new HashSet<string>();

		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual ISet<string> Lines
		{
			get { return lines; }
			set { lines = value; }
		}
	}
}