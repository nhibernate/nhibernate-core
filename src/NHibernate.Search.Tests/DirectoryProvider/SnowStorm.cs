using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Search.Attributes;
using NHibernate.Test;

namespace NHibernate.Search.Tests.DirectoryProvider
{
	[Indexed]
	public class SnowStorm
	{
		private long id;
		private DateTime dateTime;
		private string location;
		
		[DocumentId]
		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}

		[Field(Index.UnTokenized)]
		[DateBridge(Resolution.Day)]
		public virtual DateTime DateTime
		{
			get { return dateTime; }
			set { dateTime = value; }
		}

		[Field(Index.Tokenized)]
		public virtual string Location
		{
			get { return location; }
			set { location = value; }
		}
	}
}