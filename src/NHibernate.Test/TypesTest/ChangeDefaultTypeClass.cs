using System;

namespace NHibernate.Test.TypesTest
{
	public class ChangeDefaultTypeClass
	{
		public int Id { get; set; }

		public DateTime NormalDateTimeValue { get; set; } = DateTime.Today;
	}
}
