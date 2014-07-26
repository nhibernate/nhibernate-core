using System;

namespace NHibernate.Test.Hql.Ast
{
	public class TimestampVersioned
	{
		private long id;
		private DateTime version;
		private string name;

		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual DateTime Version
		{
			get { return version; }
			set { version = value; }
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		public virtual string Data { get; set; }
	}
}