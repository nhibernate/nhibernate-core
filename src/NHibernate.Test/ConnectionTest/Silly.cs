using System;

namespace NHibernate.Test.ConnectionTest
{
	[Serializable]
	public class Silly
	{
		private long id;
		private string name;
		private Other other;

		public Silly()
		{
		}

		public Silly(string name)
		{
			this.name = name;
		}

		public Silly(string name, Other other)
		{
			this.name = name;
			this.other = other;
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

		public virtual Other Other
		{
			get { return other; }
			set { other = value; }
		}
	}
}