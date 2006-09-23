using System;
using Iesi.Collections;

namespace NHibernate.Test.FilterTest
{
	public class Product
	{
		private long id;
		private string name;
		private int stockNumber;  // int for ease of hashCode() impl
		private DateTime effectiveStartDate;
		private DateTime effectiveEndDate;
		private ISet orderLineItems;
		private ISet categories;

		public virtual void AddCategory(Category category)
		{
			if (category == null)
			{
				return;
			}

			if (categories == null)
			{
				categories = new HashedSet();
			}

			categories.Add(category);
			if (category.Products == null)
			{
				category.Products = new HashedSet();
			}
			category.Products.Add(this);
		}

		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		public virtual int StockNumber
		{
			get { return stockNumber; }
			set { stockNumber = value; }
		}

		public virtual DateTime EffectiveStartDate
		{
			get { return effectiveStartDate; }
			set { effectiveStartDate = value; }
		}

		public virtual DateTime EffectiveEndDate
		{
			get { return effectiveEndDate; }
			set { effectiveEndDate = value; }
		}

		public virtual ISet OrderLineItems
		{
			get { return orderLineItems; }
			set { orderLineItems = value; }
		}

		public virtual ISet Categories
		{
			get { return categories; }
			set { categories = value; }
		}

		public override int GetHashCode()
		{
			return stockNumber;
		}

		public override bool Equals(object obj)
		{
			return obj is Product && (((Product) obj).stockNumber == this.stockNumber);
		}
	}
}
