using System;

namespace NHibernate.Test.ConnectionTest
{
	[Serializable]
	public class Other
	{
		private long id;
		private string name;

		public Other()
		{
		}

		public Other(string name)
		{
			this.name = name;
		}

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
	}
}