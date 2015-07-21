using System;
using System.Collections.Generic;

namespace NHibernate.Test.SqlTest
{
	public class Product
	{
		[Serializable]
		public class ProductIdType
		{
			private string orgid;
			private string productnumber;

			public string Orgid
			{
				get { return orgid; }
				set { orgid = value; }
			}

			public string Productnumber
			{
				get { return productnumber; }
				set { productnumber = value; }
			}

			public override bool Equals(object obj)
			{
				ProductIdType that = obj as ProductIdType;
				return that != null && that.orgid == orgid && that.productnumber == productnumber;
			}

			public override int GetHashCode()
			{
				return orgid.GetHashCode();
			}
		}

		private ProductIdType productId;
		private string name;
		private Person person;
		private ISet<Order> orders = new HashSet<Order>();

		public virtual ProductIdType ProductId
		{
			get { return productId; }
			set { productId = value; }
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		public virtual Person Person
		{
			get { return person; }
			set { person = value; }
		}

		public virtual ISet<Order> Orders
		{
			get { return orders; }
			set { orders = value; }
		}
	}
}