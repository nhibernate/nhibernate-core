using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Search.Attributes;

namespace NHibernate.Search.Tests.FieldAccess
{
	[Indexed(Index = "DocumentField")]
	public class Document
	{
		[DocumentId]
		private int id;

		[Field(Index.Tokenized, Store= Attributes.Store.Yes)]
		[Boost(2)]
		private string title;

		[Field(Index.Tokenized, Name = "Abstract", Store= Attributes.Store.No)]
		private string summary;
		
		[Field(Index.Tokenized, Store= Attributes.Store.No)]
		private string text;

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual string Title
		{
			get { return title; }
			set { title = value; }
		}

		public virtual string Summary
		{
			get { return summary; }
			set { summary = value; }
		}


		public virtual string Text
		{
			get { return text; }
			set { text = value; }
		}

		public Document()
		{
		}


		public Document(string title, string summary, string text)
		{
			this.title = title;
			this.summary = summary;
			this.text = text;
		}
	}
}
