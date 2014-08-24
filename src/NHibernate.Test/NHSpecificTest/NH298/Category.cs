using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH298 {

	public class Category {
		private int id;
		private string name;
		private IList<Category> subCategories;
		private Category parentCategory;

		public int Id {
			get { return id; }
			set { id = value; }
		}

		public string Name {
			get { return name; }
			set { name = value; }
		}

		public IList<Category> SubCategories {
			get { return subCategories; }
			private set { subCategories = value; }
		}

		public Category ParentCategory {
			get { return parentCategory; }
			set { parentCategory = value; }
		}

		public Category( int id, string name, Category parent ) {
			this.id = id;
			this.name = name;
			subCategories = new List<Category>();
			parentCategory = parent;
		}

		protected Category() : this( 0, "Unknown Category", null ) { }

		public override string ToString() { return name; }
	}
}
