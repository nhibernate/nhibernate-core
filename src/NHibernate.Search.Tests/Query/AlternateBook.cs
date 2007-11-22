using System;
using NHibernate.Search.Attributes;

namespace NHibernate.Search.Tests.Queries
{
	[Indexed(Index = "Book")]
	public class AlternateBook
	{
		private int id;
		private String summary;

		[DocumentId]
		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		[Field(Index.Tokenized, Store=Store.Yes)]
		public virtual string Summary
		{
			get { return summary; }
			set { summary = value; }
		}

		public AlternateBook()
		{

		}

		public AlternateBook(int id, string summary)
		{
			this.id = id;
			this.summary = summary;
		}
	}
}