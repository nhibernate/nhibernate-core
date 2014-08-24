using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH1990
{
	public class NewsFeed
	{
		public NewsFeed()
		{
			Items = new List<NewsItem>();
		}

		public virtual Guid Id { get; set; }
		public virtual string Title { get; set; }
		public virtual string Url { get; set; }
		public virtual int Status { get; set; }
		public virtual IList<NewsItem> Items { get; set; }
	}

	public class NewsItem
	{
		public virtual Guid Id { get; set; }
		public virtual string Title { get; set; }
		public virtual int Status { get; set; }
		public virtual NewsFeed Feed { get; set; }
	}
}
