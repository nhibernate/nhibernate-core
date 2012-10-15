using System.Collections;
using System.Collections.Generic;
using Iesi.Collections.Generic;

namespace NHibernate.Test.Extralazy
{
	public class User
	{
		private string name;
		private string password;
		private IDictionary session = new Hashtable();
		private ISet<Document> documents = new HashedSet<Document>();
		private ISet<Photo> photos = new HashedSet<Photo>();
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

		public virtual ISet<Document> Documents
		{
			get { return documents; }
			set { documents = value; }
		}


		public virtual ISet<Photo> Photos
		{
			get { return photos; }
			set { photos = value; }
		}
	}
}