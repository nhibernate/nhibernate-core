using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;
using NHibernate.Hql.Ast.ANTLR;
using NUnit.Framework;
using System.Collections;
using NHibernate.DomainModel;

namespace NHibernate.Test.CollectionFilterTest
{
	[TestFixture]
	public class CollectionFilterQueriesTest : TestCase
	{
		protected override IList Mappings
		{
			get
			{
				return new string[] { "One.hbm.xml", "Many.hbm.xml" };
			}
		}

		private One one;

		protected override void OnSetUp()
		{
			base.OnSetUp();

			// create the objects to search on		
			one = new One();
			one.X = 20;
			one.Manies = new HashSet<Many>();

			Many many1 = new Many();
			many1.X = 10;
			many1.One = one;
			one.Manies.Add( many1 );

			Many many2 = new Many();
			many2.X = 20;
			many2.One = one;
			one.Manies.Add( many2 );

			using( ISession s = OpenSession() )
			using( ITransaction t = s.BeginTransaction() )
			{
				s.Save( one );
				s.Save( many1 );
				s.Save( many2 );
				t.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using( ISession session = OpenSession() )
			using( ITransaction tx = session.BeginTransaction() )
			{
				session.Delete( "from Many" );
				session.Delete( "from One" );
				tx.Commit();
			}
			base.OnTearDown();
		}

		[Test]
		public void UpdateShouldBeDisallowed()
		{
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				One one2 = (One)s.CreateQuery("from One").UniqueResult();

				Assert.Throws<QuerySyntaxException>(() =>
				{
					s.CreateFilter(one2.Manies, "update Many set X = 1")
						.ExecuteUpdate();
					// Collection filtering disallows DML queries
				});

				t.Rollback();
			}
		}

		[Test]
		public void DeleteShouldBeDisallowed()
		{
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				One one2 = (One)s.CreateQuery("from One").UniqueResult();

				Assert.Throws<QuerySyntaxException>(() =>
				{
					s.CreateFilter(one2.Manies, "delete from Many")
						.ExecuteUpdate();
					// Collection filtering disallows DML queries
				});

				t.Rollback();
			}
		}

		[Test]
		public void InsertShouldBeDisallowed()
		{
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				One one2 = (One)s.CreateQuery("from One").UniqueResult();

				Assert.Throws<QuerySyntaxException>(() =>
				{
					s.CreateFilter(one2.Manies, "insert into Many (X) select t0.X from Many t0")
						.ExecuteUpdate();
					// Collection filtering disallows DML queries
				});

				t.Rollback();
			}
		}

		[Test]
		public void InnerSubqueryShouldNotBeFiltered()
		{
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				One one2 = (One)s.CreateQuery("from One").UniqueResult();

				s.CreateFilter(one2.Manies, "where this.X in (select t0.X from Many t0)")
					.List();
				// Filter should only affect outer query, not inner

				t.Rollback();
			}
		}

		[Test]
		public void InnerSubqueryMustHaveFromClause()
		{
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				One one2 = (One)s.CreateQuery("from One").UniqueResult();

				Assert.Throws<QuerySyntaxException>(() =>
				{
					s.CreateFilter(one2.Manies, "where this.X in (select X)")
						.List();
					// Inner query for filter query should have FROM clause 
				});

				t.Rollback();
			}
		}
	}
}
