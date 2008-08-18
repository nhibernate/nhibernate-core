using System.Collections;
using Iesi.Collections;

namespace NHibernate.Test.Extralazy
{
	public class User
	{
		private string name;
		private string password;
		private IDictionary session = new Hashtable();
		private ISet documents = new HashedSet();
		protected User() {}
		public User(string name, string password)
		{
			this.name = name;
			this.password = password;
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		public virtual string Password
		{
			get { return password; }
			set { password = value; }
		}

		public virtual IDictionary Session
		{
			get { return session; }
			set { session = value; }
		}

		public virtual ISet Documents
		{
			get { return documents; }
			set { documents = value; }
		}
	}
}