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
		private ISet<Widget> widgets = new HashSet<Widget>();
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

		public virtual IEnumerable<Widget> Widgets
		{
			get { return widgets; }
		}

		public virtual void AddWidget(string title, string description)
		{
			widgets.Add(new Widget(title, description, this));
		}

		public virtual void RemoveWidget(Widget widget)
		{
			widgets.Remove(widget);
		}
	}
}
