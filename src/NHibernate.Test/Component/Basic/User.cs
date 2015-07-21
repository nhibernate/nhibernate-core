using System;

namespace NHibernate.Test.Component.Basic
{
	public class User
	{
		private string userName;
		private string password;
		private Person person;
		private DateTime lastModified;
		
		public virtual string UserName 
		{
			get { return userName; }
			set { userName = value; }
		}
		
		public virtual string Password 
		{
			get { return password; }
			set { password = value; }
		}
		
		public virtual Person Person 
		{
			get { return person; }
			set { person = value; }
		}
		
		public virtual DateTime LastModified 
		{
			get { return lastModified; }
			set { lastModified = value; }
		}
		
		public User()
		{
		}
		
		public User(string id, string pw, Person person) 
		{
			this.userName = id;
			this.password = pw;
			this.person = person;
		}		
	}
}