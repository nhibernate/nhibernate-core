using System;

namespace NHibernate.Test.NHSpecificTest.NH1882
{
	public class Book
	{
		public Book()
		{
		}

		public Book(String title, Author author)
		{
			Title = title;
			Author = author;
		}

		public virtual int Id { get; set; }

		public virtual string Title { get; set; }

		public virtual Author Author { get; set; }
	}
}