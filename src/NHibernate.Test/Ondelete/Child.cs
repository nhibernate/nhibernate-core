namespace NHibernate.Test.Ondelete
{
	public class Child
	{
		private string name;
		private readonly Parent parent;

		protected Child()
		{
		}

		public Child(Parent parent, string name)
		{
			this.parent = parent;
			this.name = name;
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		public virtual Parent Parent
		{
			get { return parent; }
		}
	}
}