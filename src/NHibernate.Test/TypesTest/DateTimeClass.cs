using System;

namespace NHibernate.Test.TypesTest
{
	public class DateTimeClass
	{
		public int Id { get; set; }
		public DateTime Value { get; set; }
		public DateTime Revision { get; protected set; }
		public DateTime? NullableValue { get; set; }
	}
}
