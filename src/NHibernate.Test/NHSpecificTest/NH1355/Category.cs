namespace NHibernate.Test.NHSpecificTest.NH1355
{
	/// <summary>
	/// Category object for NHibernate mapped table 'Categories'.
	/// </summary>
	public class Category
	{
		private int id;
		private string categoryName;
		private byte[] rowVersion;

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual string CategoryName
		{
			get { return categoryName; }
			set { categoryName = value; }
		}

		public virtual byte[] RowVersion
		{
			get { return rowVersion; }
			set { rowVersion = value; }
		}
	}
}