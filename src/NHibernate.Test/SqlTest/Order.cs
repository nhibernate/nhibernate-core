using System;

namespace NHibernate.Test.SqlTest
{
	public class Order
	{
		[Serializable]
		public class OrderIdType
		{
			private string orgid;
			private string ordernumber;

			public string Orgid
			{
				get { return orgid; }
				set { orgid = value; }
			}

			public string Ordernumber
			{
				get { return ordernumber; }
				set { ordernumber = value; }
			}

			public override bool Equals(object obj)
			{
				OrderIdType that = obj as OrderIdType;
				return that != null && that.orgid == orgid && that.ordernumber == ordernumber;
			}

			public override int GetHashCode()
			{
				return orgid.GetHashCode();
			}
		}

		private OrderIdType orderId;
		private Product product;
		private Person person;

		public virtual OrderIdType OrderId
		{
			get { return orderId; }
			set { orderId = value; }
		}

		public virtual Product Product
		{
			get { return product; }
			set { product = value; }
		}

		public virtual Person Person
		{
			get { return person; }
			set { person = value; }
		}
	}
}