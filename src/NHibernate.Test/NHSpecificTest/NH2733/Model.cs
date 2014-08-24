using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH2733
{
	public class Item
	{
		public virtual int Id { get; set; }

		public class ItemDetails
		{
			public virtual DateTime? StartTime { get; set; }
		}

		public virtual ItemDetails Details { get; set; }

		public Item()
		{
			Details = new ItemDetails();
		}
	}
}