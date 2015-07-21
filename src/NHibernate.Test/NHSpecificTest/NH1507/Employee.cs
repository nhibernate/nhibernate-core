using System;
using System.Collections;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH1507
{
	[Serializable]
	public class Employee
	{
		private int _id;
		private IList<Order> nativeOrders;

		public virtual string LastName { get; set; }

		public virtual string FirstName { get; set; }

		public virtual string Title { get; set; }

		public virtual string TitleOfCourtesy { get; set; }

		public virtual DateTime? BirthDate { get; set; }

		public virtual DateTime? HireDate { get; set; }

		public virtual string Address { get; set; }

		public virtual string City { get; set; }

		public virtual string Region { get; set; }

		public virtual string PostalCode { get; set; }

		public virtual string Country { get; set; }

		public virtual string HomePhone { get; set; }

		public virtual string Extension { get; set; }

		public virtual byte[] Photo { get; set; }

		public virtual string Notes { get; set; }

		public virtual int? ReportsTo { get; set; }

		public virtual string PhotoPath { get; set; }

		protected virtual IList<Order> orders
		{
			get
			{
				if (nativeOrders == null)
				{
					nativeOrders = new List<Order>();
				}

				return nativeOrders;
			}
			set
			{
				if (value != nativeOrders)
				{
					nativeOrders = value;
				}
			}
		}
	}
}