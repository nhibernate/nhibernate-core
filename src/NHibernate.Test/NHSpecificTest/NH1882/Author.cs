using System;
using Iesi.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH1882
{
	public class Author
	{
		public Author()
		{
			Books = new HashedSet<Book>();
		}

		public Author(String name)
		{
			Books = new HashedSet<Book>();
			Name = name;
		}

		public virtual int Id { get; set; }

		public virtual string Name { get; set; }

		public virtual Publisher Publisher { get; set; }

		public virtual ISet<Book> Books { get; set; }
	}
}
