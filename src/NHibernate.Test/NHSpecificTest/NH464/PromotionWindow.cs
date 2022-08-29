using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH464
{
	public class PromotionWindow
	{
		private IList<DateRange> dates;

		public PromotionWindow()
		{
			this.dates = new List<DateRange>();
		}

		public IList<DateRange> Dates
		{
			get { return dates; }
			set { dates = value; }
		}

		public bool IsActive()
		{
			bool isActive = false;
			DateTime today = DateTime.Today;
			foreach (DateRange dateRange in dates)
			{
				isActive = dateRange.Contains(today);
				if (isActive)
					break;
			}

			return isActive;
		}
	}
}
