using System;

namespace NHibernate.Test.TypesTest
{
	public class TimeAsTimeSpanClass
	{
		public int Id { get; set; }
		public TimeSpan TimeSpanValue { get; set; }
		public TimeSpan TimeSpanWithScale { get; set; }
	}
}
