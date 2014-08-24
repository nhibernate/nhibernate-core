using System;

namespace NHibernate.Test.Operations
{
	public class TimestampedEntity
	{
		public virtual string Id { get; set; }
		public virtual string Name { get; set; }
		public virtual DateTime Timestamp { get; set; }
	}
}