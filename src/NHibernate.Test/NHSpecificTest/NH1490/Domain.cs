namespace NHibernate.Test.NHSpecificTest.NH1490
{
	public class Customer
	{
		private int id;
		private string name;
		private Category category;
		private bool isActive;
		public Customer() {}
		public Customer(string name)
		{
			this.name = name;
		}

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		public virtual Category Category
		{
			get { return category; }
			set { category = value; }
		}

		public virtual bool IsActive
		{
			get { return isActive; }
			set { isActive = value; }
		}
	}

	public class Category
	{
		private int id;
		private string name;
		private bool isActive;
		public Category() {}
		public Category(string name)
		{
			this.name = name;
		}

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		public virtual bool IsActive
		{
			get { return isActive; }
			set { isActive = value; }
		}
	}
}