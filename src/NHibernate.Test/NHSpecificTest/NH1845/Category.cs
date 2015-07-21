using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH1845
{
	public class Category
	{
		private readonly IList<Category> subcategories = new List<Category>();

		public Category() : this("") {}

		public Category(string name)
		{
			Name = name;
		}

		public virtual int Id { get; set; }

		public virtual string Name { get; set; }

		public virtual Category Parent { get; set; }

		public virtual IList<Category> Subcategories
		{
			get { return subcategories; }
		}

		public virtual void AddSubcategory(Category subcategory)
		{
			subcategories.Add(subcategory);
			subcategory.Parent = this;
		}

		public override string ToString()
		{
			return Name;
		}

		public override bool Equals(object obj)
		{
			var other = obj as Category;
			if (other == null)
			{
				return false;
			}
			return other.Name == Name;
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}
	}
}