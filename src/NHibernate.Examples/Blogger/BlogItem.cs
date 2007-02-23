using System;

namespace NHibernate.Examples.Blogger
{
	public class BlogItem
	{
		private long id;
		private DateTime dateTime;
		private string text;
		private string title;
		private Blog blog;

		public Blog ParentBlog
		{
			get { return blog; }
			set { blog = value; }
		}

		public DateTime ItemDate
		{
			get { return dateTime; }
			set { dateTime = value; }
		}

		public long Id
		{
			get { return id; }
			set { id = value; }
		}

		public string Text
		{
			get { return text; }
			set { text = value; }
		}

		public string Title
		{
			get { return title; }
			set { title = value; }
		}
	}
}