using System;
using System.Collections.Generic;

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
		private ISet<Jay> jays = new HashSet<Jay>();

		public Eye()
		{
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

		public virtual Jay Jay
		{
			get { return jay; }
			set { jay = value; }
		}

		public virtual ISet<Jay> Jays
		{
			get { return jays; }
			set { jays = value; }
		}
	}
}