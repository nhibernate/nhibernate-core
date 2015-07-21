using System;
using System.Collections.Generic;

namespace NHibernate.Test.FilterTest
{
	public class Category
	{
		private long id;
		private String name;
		private DateTime effectiveStartDate;
		private DateTime effectiveEndDate;
		private ISet<Product> products;

		public Category()
		{
		}

		public Category(String name)
		{
			this.name = name;
		}

		public Category(String name, DateTime effectiveStartDate, DateTime effectiveEndDate)
		{
			this.name = name;
			this.effectiveStartDate = effectiveStartDate;
			this.effectiveEndDate = effectiveEndDate;
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

		public virtual ISet<Product> Products
		{
			get { return products; }
			set { products = value; }
		}

		public override bool Equals(Object o)
		{
			if (this == o)
				return true;
			if (!(o is Category))
				return false;

			Category category = (Category) o;

			if (!name.Equals(category.name))
			{
				return false;
			}

			if (!effectiveEndDate.Equals(category.effectiveEndDate))
			{
				return false;
			}

			if (!effectiveStartDate.Equals(category.effectiveStartDate))
			{
				return false;
			}

			return true;
		}

		public override int GetHashCode()
		{
			int result;
			result = name.GetHashCode();
			result = 29 * result + effectiveStartDate.GetHashCode();
			result = 29 * result + effectiveEndDate.GetHashCode();
			return result;
		}
	}
}