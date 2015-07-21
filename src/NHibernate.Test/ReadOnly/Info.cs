using System;

namespace NHibernate.Test.ReadOnly
{
	public class Info
	{
		private long id;
		private string details;
		
		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}
		
		public virtual string Details
		{
			get { return details; }
			set { details = value; }
		}
		
		public Info()
		{
		}
		
		public Info(string details)
		{
			this.details = details;
		}
	}
}
