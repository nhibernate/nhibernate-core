using System.Collections.Generic;

namespace NHibernate.Test.Extralazy
{
	public class Group
	{
		private string _name;
		private IDictionary<string, User> _users = new Dictionary<string, User>();

		protected Group() {}
		public Group(string name)
		{
			this._name = name;
		}

		public virtual string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public virtual IDictionary<string, User> Users
		{
			get { return _users; }
			set { _users = value; }
		}
	}
}