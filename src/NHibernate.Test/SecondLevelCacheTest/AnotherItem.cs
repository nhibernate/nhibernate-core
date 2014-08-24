namespace NHibernate.Test.SecondLevelCacheTests
{
	public class AnotherItem
	{
		private int id;
		private string name;
		private string description= string.Empty;
		
		public AnotherItem()
		{

		}

		public AnotherItem(string name)
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

		public virtual string Description
		{
			get { return description; }
			set { description = value; }
		}
	}
}