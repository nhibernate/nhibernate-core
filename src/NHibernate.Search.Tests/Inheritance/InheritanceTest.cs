using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Lucene.Net.Analysis;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using NUnit.Framework;

namespace NHibernate.Search.Tests.Inheritance
{
	[TestFixture]
	public class InheritanceTest : SearchTestCase
	{
		[Test]
		public void Inheritance()
		{
			IFullTextSession s = Search.CreateFullTextSession(OpenSession());
			ITransaction tx = s.BeginTransaction();
			Animal a = new Animal();
			a.Name = ("Shark Jr");
			s.Save(a);
			Mammal m = new Mammal();
			m.NumberOfLegs = (4);
			m.Name = ("Elephant Jr");
			s.Save(m);
			tx.Commit(); //post commit events for lucene
			s.Clear();
			tx = s.BeginTransaction();
			QueryParser parser = new QueryParser("Name", new StopAnalyzer());

			Query query;
			IQuery hibQuery;

			query = parser.Parse("Elephant");
			hibQuery = s.CreateFullTextQuery(query, typeof (Mammal));
			IList result = hibQuery.List();
			Assert.IsNotEmpty(result);
			Assert.AreEqual(1, result.Count, "Query subclass by superclass attribute");

			query = parser.Parse("NumberOfLegs:[4 TO 4]");
			hibQuery = s.CreateFullTextQuery(query, typeof (Animal), typeof (Mammal));
			result = hibQuery.List();
			Assert.IsNotEmpty(result);
			Assert.AreEqual(1, result.Count, "Query subclass by subclass attribute");

			query = parser.Parse("Jr");
			hibQuery = s.CreateFullTextQuery(query, typeof (Animal));
			result = hibQuery.List();
			Assert.IsNotEmpty(result);
			Assert.AreEqual(2, result.Count, "Query filtering on superclass return mapped subclasses");
			foreach (Object managedEntity in result)
			{
				s.Delete(managedEntity);
			}
			tx.Commit();
			s.Close();
		}

		protected override IList Mappings
		{
			get
			{
				return new string[]
					{
						"Inheritance.Animal.hbm.xml",
						"Inheritance.Mammal.hbm.xml",
					};
			}
		}
	}
}