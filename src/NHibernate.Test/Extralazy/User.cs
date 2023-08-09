using System.Collections.Generic;

namespace NHibernate.Test.Extralazy
{
	public class User
	{
		private string name;
		private string password;
		private IDictionary<string, SessionAttribute> session = new Dictionary<string, SessionAttribute>();
		private ISet<Document> documents = new HashSet<Document>();
		private ISet<Photo> photos = new HashSet<Photo>();
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

		public virtual IDictionary<string, SessionAttribute> Session
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

		public virtual IDictionary<string, UserSetting> Settings { get; set; } = new Dictionary<string, UserSetting>();

		public virtual ISet<UserPermission> Permissions { get; set; } = new HashSet<UserPermission>();

		public virtual ISet<UserFollower> Followers { get; set; } = new HashSet<UserFollower>();

		public virtual IList<Company> Companies { get; set; } = new List<Company>();

		public virtual IList<CreditCard> CreditCards { get; set; } = new List<CreditCard>();

		public virtual void UpdateCompaniesIndexes()
		{
			for (var i = 0; i < Companies.Count; i++)
			{
				Companies[i].ListIndex = i;
			}
		}
	}
}
