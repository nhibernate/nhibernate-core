using System;

namespace NHibernate.Test.NHSpecificTest.NH3984
{
	public class LogEntry
	{
		public virtual int Id { get; set; }
		public virtual string Text { get; set; }
		public virtual DateTime CreatedAt { get; set; }
	}
}
