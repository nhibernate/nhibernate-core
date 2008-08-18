using System.Collections;

namespace NHibernate.Test.Extralazy
{
	public class Group
	{
		private string name;
		private IDictionary users = new Hashtable();
		protected Group() {}
		public Group(string name)
		{
			this.name = name;
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		public virtual IDictionary Users
		{
			get { return users; }
			set { users = value; }
		}
	}
}