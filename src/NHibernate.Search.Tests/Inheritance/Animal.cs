using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Search.Attributes;

namespace NHibernate.Search.Tests.Inheritance
{
	[Indexed]
	public class Animal
	{
		private int id;
		private string name;

		[DocumentId]
		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		[Field(Index.Tokenized, Store = Store.No)]
		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}
	}
}
