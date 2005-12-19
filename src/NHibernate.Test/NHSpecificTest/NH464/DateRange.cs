using System;

namespace NHibernate.Test.NHSpecificTest.NH464
{
	public struct DateRange
	{
		public static readonly DateRange Infinite = new DateRange(new DateTime(2001, 1, 1), new DateTime(2050, 12, 31));

		private DateTime start;
		private DateTime end;

		public DateRange(DateTime start, DateTime end)
		{
			this.start = start;
			this.end = end;
		}

		public DateTime Start
		{
			get { return start; }
			set { start = value; }
		}

		public DateTime End
		{
			get { return end; }
			set { end = value; }
		}

		public bool Contains(DateTime date)
		{
			return ((date.CompareTo(start) >= 0) && (date.CompareTo(end) <= 0));
		}
	}
}