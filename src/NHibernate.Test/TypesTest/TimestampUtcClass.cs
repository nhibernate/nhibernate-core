using System;

namespace NHibernate.Test.TypesTest
{
	public class TimestampUtcClass
	{
		public int Id { get; set; }
		public DateTime Value { get; set; }
		public DateTime Revision { get; protected set; }
	}
}
