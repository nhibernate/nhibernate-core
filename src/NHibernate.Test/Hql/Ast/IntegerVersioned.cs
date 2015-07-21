namespace NHibernate.Test.Hql.Ast
{
	public class IntegerVersioned
	{
		private long id;
		private int version;
		private string name;

		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual int Version
		{
			get { return version; }
			set { version = value; }
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		public virtual string Data { get; set; }
	}
}