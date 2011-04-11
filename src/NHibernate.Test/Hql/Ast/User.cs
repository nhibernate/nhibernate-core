using System.Collections;

namespace NHibernate.Test.HQL.Ast
{
	public class User
	{
		private long id;
		private string userName;
		private Human human;
		private IList permissions;

		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual string UserName
		{
			get { return userName; }
			set { userName = value; }
		}

		public virtual Human Human
		{
			get { return human; }
			set { human = value; }
		}

		public virtual IList Permissions
		{
			get { return permissions; }
			set { permissions = value; }
		}
	}
}