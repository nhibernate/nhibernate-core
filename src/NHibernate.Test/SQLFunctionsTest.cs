using System;
using System.Collections;

using NHibernate.DomainModel;

using NUnit.Framework;

namespace NHibernate.Test
{
	/// <summary>
	/// Summary description for SQLFunctionsTest.
	/// </summary>
	[TestFixture]
	public class SQLFunctionsTest : TestCase
	{
		[SetUp]
		public void SetUp() 
		{
			ExportSchema( new string[] { "Simple.hbm.xml",
										"Blobber.hbm.xml"
									   } );
		}

		[Test]
		public void SetProperties() 
		{
			ISession s = sessions.OpenSession();
			ITransaction t = s.BeginTransaction();
			Simple simple = new Simple();
			simple.Name = "Simple 1";
			s.Save(simple, (long)10);
			IQuery q = s.CreateQuery("from s in class Simple where s.Name=:Name and s.Count=:Count");
			q.SetProperties(simple);
			Assert.AreEqual( simple, q.List()[0]);
			s.Delete(simple);
			t.Commit();
			s.Close();
		}

		[Test]
		public void NothingToUpdate() 
		{
			ISession s = sessions.OpenSession();
			ITransaction t = s.BeginTransaction();
			Simple simple = new Simple();
			simple.Name = "Simple 1";
			s.Save( simple, (long)10 );
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			s.Update( simple, (long)10 );
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			s.Update( simple, (long)10 );
			s.Delete(simple);
			t.Commit();
			s.Close();

		}

		[Test]
		public void SQLFunctions() 
		{
			ISession s = sessions.OpenSession();
			ITransaction t = s.BeginTransaction();
			Simple simple = new Simple();
			simple.Name = "Simple 1";
			s.Save( simple, (long)10 );

			/*
			 * TODO: once DB2Dialect is implemented uncomment this 
			if( dialect is Dialect.DB2Dialect ) 
			{
				s.Find("from s in class Simple where repeat('foo', 3) = 'foofoofoo'");
				s.Find("from s in class Simple where repeat(s.Name, 3) = 'foofoofoo'");
				s.Find("from s in class Simple where repeat( lower(s.Name), 3 + (1-1) / 2) = 'foofoofoo'");
			}
			*/

			Assert.AreEqual( 1, s.Find("from s in class Simple where upper(s.Name) = 'SIMPLE 1'").Count );
			Assert.AreEqual( 1, s.Find("from s in class Simple where not( upper(s.Name)='yada' or 1=2 or 'foo'='bar' or not('foo'='foo') or 'foo' like 'bar')").Count );

			if( !(dialect is Dialect.MySQLDialect) && !(dialect is Dialect.SybaseDialect) && !(dialect is Dialect.MsSql2000Dialect) ) 
			{
				// Dialect.MckoiDialect and Dialect.InterbaseDialect also included
				// My Sql has a funny concatenation operator
				Assert.AreEqual( 1, s.Find("from s in class Simple where lower(s.Name || ' foo')='simple 1 foo'").Count );
			}

			if( (dialect is Dialect.SybaseDialect) ) 
			{
				Assert.AreEqual( 1, s.Find("from s in class Simple where lower( concat(s.Name, ' foo') ) = 'simple 1 foo'").Count );
			}

			if( (dialect is Dialect.MsSql2000Dialect) ) 
			{
				Assert.AreEqual( 1, s.Find("from s in class Simple where lower( s.Name + ' foo' ) = 'simple 1 foo'").Count );
			}

			/*
			 * TODO: uncomment if MckoiDialect is ever implemented 
			if( (dialect is Dialect.MckoiDialect) ) 
			{
				Assert.AreEqual( 1, s.Find("from s in class Simple where lower( concat(s.Name, ' foo') ) = 'simple 1 foo'").Count );
			}
			*/

			Simple other = new Simple();
			other.Name = "Simple 2";
			other.Count = 12;
			simple.Other = other;
			s.Save( other, (long)20 );
			Assert.AreEqual( 1, s.Find("from s in class Simple where upper( s.Other.Name )='SIMPLE 2'").Count );
			Assert.AreEqual( 0, s.Find("from s in class Simple where not (upper(s.Other.Name)='SIMPLE 2')").Count );
			Assert.AreEqual( 1, s.Find("select distinct s from s in class Simple where ( ( s.Other.Count + 3) = (15*2)/2 and s.Count = 69) or ( (s.Other.Count + 2) / 7 ) = 2").Count );
			Assert.AreEqual( 1, s.Find("select s from s in class Simple where ( ( s.Other.Count + 3) = (15*2)/2 and s.Count = 69) or ( (s.Other.Count + 2) / 7 ) = 2 order by s.Other.Count").Count );

			Simple min = new Simple();
			min.Count = -1;

			s.Save( min, (long)30 );

			// && !(dialect is Dialect.HSQLDialect)
			// MySql has no subqueries
			if( !(dialect is Dialect.MySQLDialect) ) 
			{
				Assert.AreEqual( 2, s.Find("from s in class Simple where s.Count > ( select min(sim.Count) from sim in class NHibernate.DomainModel.Simple )").Count );
				t.Commit();
				t = s.BeginTransaction();
				Assert.AreEqual( 2, s.Find("from s in class Simple where s = some( select sim from sim in class NHibernate.DomainModel.Simple where sim.Count>=0) and s.Count >= 0").Count );
				Assert.AreEqual( 1, s.Find("from s in class Simple where s = some( select sim from sim in class NHibernate.DomainModel.Simple where sim.Other.Count=s.Other.Count ) and s.Other.Count > 0").Count );
			}

			IEnumerator enumer = s.Enumerable("select sum(s.Count) from s in class Simple group by s.Count having sum(s.Count) > 10 ").GetEnumerator();
			Assert.IsTrue( enumer.MoveNext() );
			Assert.AreEqual(12, (Int32)enumer.Current );
			Assert.IsFalse( enumer.MoveNext() );

			if( !(dialect is Dialect.MySQLDialect) ) 
			{
				enumer = s.Enumerable("select s.Count from s in class Simple group by s.Count having s.Count = 12").GetEnumerator();
				Assert.IsTrue( enumer.MoveNext() );
			}

			enumer = s.Enumerable("select s.id, s.Count, count(t), max(t.Date) from s in class Simple, t in class Simple where s.Count = t.Count group by s.id, s.Count order by s.Count").GetEnumerator();

			IQuery q = s.CreateQuery("from s in class Simple");
			q.SetMaxResults(10);
			Assert.AreEqual( 3, q.List().Count );
			
			q = s.CreateQuery("from s in class Simple");
			q.SetMaxResults(1);
			Assert.AreEqual( 1, q.List().Count );

			q = s.CreateQuery("from s in class Simple");
			Assert.AreEqual( 3, q.List().Count );

			q = s.CreateQuery("from s in class Simple where s.Name = ?");
			q.SetString(0, "Simple 1");
			Assert.AreEqual( 1, q.List().Count );

			q = s.CreateQuery("from s in class Simple where s.Name = ? and upper(s.Name) = ?");
			q.SetString( 1, "SIMPLE 1" );
			q.SetString( 0, "Simple 1" );
			q.SetFirstResult(0);
			Assert.IsTrue( q.Enumerable().GetEnumerator().MoveNext() );

			q = s.CreateQuery("from s in class Simple where s.Name = :foo and upper(s.Name) = :bar or s.Count=:count or s.Count=:count + 1");
			q.SetParameter("bar", "SIMPLE 1");
			q.SetString("foo", "Simple 1");
			q.SetInt32("count", 69);
			q.SetFirstResult(0);
			Assert.IsTrue( q.Enumerable().GetEnumerator().MoveNext() );

			q = s.CreateQuery("select s.id from s in class Simple");
			q.SetFirstResult(1);
			q.SetMaxResults(2);
			IEnumerable enumerable = q.Enumerable();
			int i=0;
			foreach( object obj in enumerable ) 
			{
				Assert.IsTrue( obj is Int64 );
				i++;
			}

			Assert.AreEqual( 2, i );

			q = s.CreateQuery("select all s, s.Other from s in class Simple where s = :s");
			q.SetParameter("s", simple);
			Assert.AreEqual( 1, q.List().Count );

			q = s.CreateQuery("from s in class Simple where s.Name in (:name_list) and s.Count > :count");
			IList list = new ArrayList(2);
			list.Add("Simple 1");
			list.Add("foo");
			q.SetParameterList( "name_list", list );
			q.SetParameter( "count", (int)-1 );
			Assert.AreEqual( 1, q.List().Count );
			
			s.Delete(other);
			s.Delete(simple);
			s.Delete(min);
			t.Commit();
			s.Close();

		}

		[Test]
		[Ignore("BLOB/CLOB not implmented like h2.0.3 - http://jira.nhibernate.org:8080/browse/NH-19")]
		public void BlobClob() 
		{
		}

		
	}
}
