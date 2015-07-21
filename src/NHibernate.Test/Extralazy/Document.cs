namespace NHibernate.Test.Extralazy
{
	public class Document
	{
		private string title;
		private string content;
		private User owner;
		protected Document() {}
		public Document(string title, string content, User owner)
		{
			this.title = title;
			this.content = content;
			this.owner = owner;
		}

		public virtual string Title
		{
			get { return title; }
			set { title = value; }
		}

		public virtual string Content
		{
			get { return content; }
			set { content = value; }
		}

		public virtual User Owner
		{
			get { return owner; }
			set { owner = value; }
		}
	}
}