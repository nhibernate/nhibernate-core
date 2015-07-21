using System;
using System.Collections;

namespace NHibernate.Test.SubselectFetchTest
{
	public class Child
	{
		private string name;
		private IList friends;

		protected Child()
		{
		}

		public Child(string name)
		{
			this.name = name;
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		public virtual IList Friends
		{
			get { return friends; }
			set { friends = value; }
		}
	}
}