using System;

namespace NHibernate.Test.NHSpecificTest.NH464
{
	public class DateRange
	{
		private DateTime start, end;

		public DateRange()
		{
		}

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
	}
}
