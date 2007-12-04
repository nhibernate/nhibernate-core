using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Lucene.Net.Analysis;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using NUnit.Framework;

namespace NHibernate.Search.Tests.Queries
{
	[TestFixture]
	public class LuceneQueryTest : SearchTestCase
	{
		[Test]
		public void UsingCriteriaApi()
		{
			IFullTextSession s = Search.CreateFullTextSession(OpenSession());
			ITransaction tx = s.BeginTransaction();
			Clock clock = new Clock(1, "Seiko");
			s.Save(clock);
			tx.Commit();

			IList list = s.CreateCriteria(typeof(Clock))
				.Add(Search.Query("Brand:seiko"))
				.List();
			Assert.AreEqual(1, list.Count, "should get result back from query");

			s.Delete(clock);
			s.Flush();
			s.Close();
		}

		[Test]
		public void List()
		{
			IFullTextSession s = Search.CreateFullTextSession(OpenSession());
			ITransaction tx = s.BeginTransaction();
			Clock clock = new Clock(1, "Seiko");
			s.Save(clock);
			clock = new Clock(2, "Festina");
			s.Save(clock);
			Book book = new Book(1, "La chute de la petite reine a travers les yeux de Festina", "La chute de la petite reine a travers les yeux de Festina, blahblah");
			s.Save(book);
			book = new Book(2, "La gloire de mon père", "Les deboires de mon père en vélo");
			s.Save(book);
			tx.Commit();
			s.Clear();
			tx = s.BeginTransaction();
			QueryParser parser = new QueryParser("title", new StopAnalyzer());

			Query query = parser.Parse("Summary:noword");
			IQuery hibQuery = s.CreateFullTextQuery(query, typeof(Clock), typeof(Book));
			IList result = hibQuery.List();
			Assert.AreEqual(0, result.Count);

			query = parser.Parse("Summary:Festina Or Brand:Seiko");
			hibQuery = s.CreateFullTextQuery(query, typeof(Clock), typeof(Book));
			result = hibQuery.List();
			Assert.AreEqual(2, result.Count, "Query with explicit class filter");


			query = parser.Parse("Summary:Festina Or Brand:Seiko");
			hibQuery = s.CreateFullTextQuery(query);
			result = hibQuery.List();
			Assert.AreEqual(2, result.Count, "Query with no class filter");
			foreach (Object element in result)
			{
				Assert.IsTrue(NHibernateUtil.IsInitialized(element));
				s.Delete(element);
			}
			s.Flush();
			tx.Commit();

			tx = s.BeginTransaction();
			query = parser.Parse("Summary:Festina Or Brand:Seiko");
			hibQuery = s.CreateFullTextQuery(query);
			result = hibQuery.List();
			Assert.AreEqual(0, result.Count, "Query with delete objects");

			s.Delete("from System.Object");
			tx.Commit();
			s.Close();
		}

		[Test]
		public void FirstMax()
		{
			IFullTextSession s = Search.CreateFullTextSession(OpenSession());
			ITransaction tx = s.BeginTransaction();
			Clock clock = new Clock(1, "Seiko");
			s.Save(clock);
			clock = new Clock(2, "Festina");
			s.Save(clock);
			Book book = new Book(1, "La chute de la petite reine a travers les yeux de Festina", "La chute de la petite reine a travers les yeux de Festina, blahblah");
			s.Save(book);
			book = new Book(2, "La gloire de mon père", "Les deboires de mon père en vélo");
			s.Save(book);
			tx.Commit();
			s.Clear();
			tx = s.BeginTransaction();
			QueryParser parser = new QueryParser("title", new StopAnalyzer());

			Query query = parser.Parse("Summary:Festina Or Brand:Seiko");
			IQuery hibQuery = s.CreateFullTextQuery(query, typeof(Clock), typeof(Book));
			hibQuery.SetFirstResult(1);
			IList result = hibQuery.List();
			Assert.AreEqual(1, result.Count, "first result no max result");

			hibQuery.SetFirstResult(0);
			hibQuery.SetMaxResults(1);
			result = hibQuery.List();
			Assert.AreEqual(1, result.Count, "max result set");

			hibQuery.SetFirstResult(0);
			hibQuery.SetMaxResults(3);
			result = hibQuery.List();
			Assert.AreEqual(2, result.Count, "max result out of limit");

			hibQuery.SetFirstResult(2);
			hibQuery.SetMaxResults(3);
			result = hibQuery.List();
			Assert.AreEqual(0, result.Count, "first result out of limit");

			s.Delete("from System.Object");
			tx.Commit();
			s.Close();
		}

		[Test]
		public void Iterator()
		{
			IFullTextSession s = Search.CreateFullTextSession(OpenSession());
			ITransaction tx = s.BeginTransaction();
			Clock clock = new Clock(1, "Seiko");
			s.Save(clock);
			clock = new Clock(2, "Festina");
			s.Save(clock);
			Book book = new Book(1, "La chute de la petite reine a travers les yeux de Festina", "La chute de la petite reine a travers les yeux de Festina, blahblah");
			s.Save(book);
			book = new Book(2, "La gloire de mon père", "Les deboires de mon père en vélo");
			s.Save(book);
			tx.Commit(); //post Commit events for lucene
			s.Clear();
			tx = s.BeginTransaction();
			QueryParser parser = new QueryParser("title", new StopAnalyzer());

			Query query = parser.Parse("Summary:noword");
			IQuery hibQuery = s.CreateFullTextQuery(query, typeof(Clock), typeof(Book));
			IEnumerator result = hibQuery.Enumerable().GetEnumerator();
			Assert.IsFalse(result.MoveNext());

			query = parser.Parse("Summary:Festina Or Brand:Seiko");
			hibQuery = s.CreateFullTextQuery(query, typeof(Clock), typeof(Book));
			result = hibQuery.Enumerable().GetEnumerator();
			int index = 0;
			while (result.MoveNext())
			{
				index++;
				s.Delete(result.Current);
			}
			Assert.AreEqual(2, index);

			tx.Commit();

			tx = s.BeginTransaction();
			query = parser.Parse("Summary:Festina Or Brand:Seiko");
			hibQuery = s.CreateFullTextQuery(query, typeof(Clock), typeof(Book));
			result = hibQuery.Enumerable().GetEnumerator();

			Assert.IsFalse(result.MoveNext());
			s.Delete("from System.Object");
			tx.Commit();
			s.Close();
		}

		[Test]
		public void MultipleEntityPerIndex()
		{
			IFullTextSession s = Search.CreateFullTextSession(OpenSession());
			ITransaction tx = s.BeginTransaction();
			Clock clock = new Clock(1, "Seiko");
			s.Save(clock);
			Book book = new Book(1, "La chute de la petite reine a travers les yeux de Festina", "La chute de la petite reine a travers les yeux de Festina, blahblah");
			s.Save(book);
			AlternateBook alternateBook = new AlternateBook(1, "La chute de la petite reine a travers les yeux de Festina");
			s.Save(alternateBook);
			tx.Commit();
			s.Clear();
			tx = s.BeginTransaction();
			QueryParser parser = new QueryParser("Title", new StopAnalyzer());

			Query query = parser.Parse("Summary:Festina");
			IQuery hibQuery = s.CreateFullTextQuery(query, typeof(Clock), typeof(Book));
			IList result = hibQuery.List();

			Assert.AreEqual(1, result.Count, "Query with explicit class filter");

			query = parser.Parse("Summary:Festina");
			hibQuery = s.CreateFullTextQuery(query, typeof(Clock), typeof(Book));
			IEnumerator it = hibQuery.Enumerable().GetEnumerator();
			Assert.IsTrue(it.MoveNext());
			Assert.IsNotNull(it.Current);
			Assert.IsFalse(it.MoveNext());

			query = parser.Parse("Summary:Festina OR Brand:seiko");
			hibQuery = s.CreateFullTextQuery(query, typeof(Clock), typeof(Book));
			hibQuery.SetMaxResults(2);
			result = hibQuery.List();
			Assert.AreEqual(2, result.Count, "Query with explicit class filter and limit");

			query = parser.Parse("Summary:Festina");
			hibQuery = s.CreateFullTextQuery(query);
			result = hibQuery.List();
			Assert.AreEqual(2, result.Count, "Query with no class filter");
			foreach (Object element in result)
			{
				Assert.IsTrue(NHibernateUtil.IsInitialized(element));
				s.Delete(element);
			}
			s.Delete("from System.Object");
			tx.Commit();
			s.Close();
		}


		protected override IList Mappings
		{
			get
			{
				return new string[]
					{
						"Query.Book.hbm.xml",
						"Query.AlternateBook.hbm.xml",
						"Query.Clock.hbm.xml"
					};
			}
		}
	}
}