namespace NHibernate.Test.Unionsubclass
{
	public class DatabaseKeyword : DatabaseKeywordBase
	{
		private string table;
		private string create;
		private string view;
		private string user;

		public virtual string Table
		{
			get { return table; }
			set { table = value; }
		}

		public virtual string Create
		{
			get { return create; }
			set { create = value; }
		}

		public virtual string View
		{
			get { return view; }
			set { view = value; }
		}

		public virtual string User
		{
			get { return user; }
			set { user = value; }
		}
	}
}
