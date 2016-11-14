
using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH1756
{

	public class Book
	{
		private DateTime _version;

		public virtual DateTime Version
		{
			get { return _version; }
			set { _version = value; }
		}

		public virtual DateTime PreviousVersion { get; set; }


		public virtual int				Id		{ get; set; }
		public virtual string			Name	{ get; set; }
		public virtual IList<Page>		Pages	{ get; set; }

		public Book()
		{
			PreviousVersion = new DateTime(1900, 1, 1);
		}
	}

	public class Page
	{
		public virtual int				Id		{ get; set; }
	}

	public class BookNotGenerated
	{
		private DateTime _version;

		public virtual DateTime Version
		{
			get { return _version; }
			set { _version = value; }
		}

		public virtual int				Id		{ get; set; }
		public virtual string			Name	{ get; set; }
		public virtual IList<Page>		Pages	{ get; set; }
	}

}
