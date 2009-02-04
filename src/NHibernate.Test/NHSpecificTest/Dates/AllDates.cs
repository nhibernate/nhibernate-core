using System;

namespace NHibernate.Test.NHSpecificTest.Dates
{
	public class AllDates
	{
		public int Id { get; set; }

		public DateTime Sql_datetime { get; set; }

		public DateTime Sql_datetime2 { get; set; }

		public DateTimeOffset Sql_datetimeoffset { get; set; }
	}
}