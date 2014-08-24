namespace NHibernate.Test.Hql.Ast
{
	public class Joiner
	{
		private long id;
		private string name;
		private string joinedName;

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

		public virtual string JoinedName
		{
			get { return joinedName; }
			set { joinedName = value; }
		}
	}
}