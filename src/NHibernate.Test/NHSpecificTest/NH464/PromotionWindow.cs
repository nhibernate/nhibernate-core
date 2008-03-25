using System;
using System.Collections;

namespace NHibernate.Test.NHSpecificTest.NH464
{
	public class PromotionWindow
	{
		private IList dates;

		public PromotionWindow()
		{
			this.dates = new ArrayList();
		}

		public IList Dates
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