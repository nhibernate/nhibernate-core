using System;
using Iesi.Collections;

namespace NHibernate.DomainModel
{
	/// <summary>
	/// Summary description for Eye.
	/// </summary>
	public class Eye
	{
		private long id;
		private string name;
		private Jay jay;
		private ISet jays = new HashedSet();

		public Eye()
		{
		}

		public long Id
		{
			get { return id; }
			set { id = value; }
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public Jay Jay
		{
			get { return jay; }
			set { jay = value; }
		}

		public ISet Jays
		{
			get { return jays; }
			set { jays = value; }
		}
	}
}
