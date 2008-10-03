using System;
using System.Collections;
using System.Collections.Generic;

using NHibernate.Criterion;

using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1508
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			Person john = new Person();
			john.Name = "John";

			Document doc1 = new Document();
			doc1.Person = john;
			doc1.Title = "John's Doc";

			Document doc2 = new Document();
			doc2.Title = "Spec";
			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				session.Save(john);
				session.Save(doc1);
				session.Save(doc2);

				tx.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				session.Delete("from Person");
				session.Delete("from Document");
				tx.Commit();
			}
		}

		[Test]
		public void DoesntThrowExceptionWhenHqlQueryIsGiven()
		{
			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				IQuery sqlQuery = session.CreateQuery("from Document");
				IMultiQuery q = session
					.CreateMultiQuery()
					.Add(sqlQuery);
				q.List();
			}
		}

		[Test]
		[ExpectedException(typeof(NotSupportedException))]
		public void ThrowsExceptionWhenSqlQueryIsGiven()
		{
			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				ISQLQuery sqlQuery = session.CreateSQLQuery("select * from Document");
				IMultiQuery q = session
					.CreateMultiQuery()
					.Add(sqlQuery);
				q.List();
			}
		}

		[Test]
		[ExpectedException(typeof(NotSupportedException))]
		public void ThrowsExceptionWhenNamedSqlQueryIsGiven()
		{
			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{

				IMultiQuery q = session
					.CreateMultiQuery()
					.AddNamedQuery("SampleSqlQuery");
				q.List();
			}
		}

	}
}
