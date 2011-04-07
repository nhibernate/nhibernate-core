using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH2632
{
	public class Customer
	{
		public virtual Int64 Id
		{
			get;
			private set;
		}
		public virtual String Name
		{
			get;
			set;
		}
		public virtual String Address
		{
			get;
			set;
		}
		public virtual IEnumerable<Order> Orders
		{
			get;
			private set;
		}
	} 

	public class Order
	{
		public virtual Int32 Id
		{
			get;
			private set;
		}

		public virtual DateTime Date
		{
			get;
			set;
		}

		public virtual Customer Customer
		{
			get;
			set;
		}
	}
}