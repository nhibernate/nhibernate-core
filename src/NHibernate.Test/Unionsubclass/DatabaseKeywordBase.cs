namespace NHibernate.Test.Unionsubclass
{
	public abstract class DatabaseKeywordBase
	{
		private long id;

		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}
	}
}
