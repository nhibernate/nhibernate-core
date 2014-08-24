using System;
using System.Collections.Generic;

namespace NHibernate.Test.FilterTest
{
	public class Product
	{
		private long id;
		private string name;
		private int stockNumber; // int for ease of hashCode() impl
		private DateTime effectiveStartDate;
		private DateTime effectiveEndDate;
		private ISet<LineItem> orderLineItems;
		private ISet<Category> categories;

		public virtual void AddCategory(Category category)
		{
			if (category == null)
			{
				return;
			}

			if (categories == null)
			{
				categories = new HashSet<Category>();
			}

			categories.Add(category);
			if (category.Products == null)
			{
				category.Products = new HashSet<Product>();
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

		public virtual ISet<LineItem> OrderLineItems
		{
			get { return orderLineItems; }
			set { orderLineItems = value; }
		}

		public virtual ISet<Category> Categories
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