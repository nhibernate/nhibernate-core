namespace NHibernate.Test.Naturalid.Mutable
{
	public class User
	{
		// Used by reflection
#pragma warning disable CS0169 // The field is never used
		private long id;
#pragma warning restore CS0169 // The field is never used
		private readonly string name;
		private readonly string org;
		private string password;

		public User() {}
		public User(string name, string org, string password)
		{
			this.name = name;
			this.org = org;
			this.password = password;
		}

		public virtual string Name
		{
			get { return name; }
		}

		public virtual string Org
		{
			get { return org; }
		}

		public virtual string Password
		{
			get { return password; }
			set { password = value; }
		}
	}
}