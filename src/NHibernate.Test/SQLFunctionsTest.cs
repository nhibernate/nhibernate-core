using System;
using System.Collections;

using NHibernate.Dialect;
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
		private static readonly log4net.ILog log =
			log4net.LogManager.GetLogger( typeof( SQLFunctionsTest ) );
		protected override IList Mappings
		{
			get
			{
				return new string[] { "Simple.hbm.xml", "Blobber.hbm.xml", "Broken.hbm.xml" };
			}
		}

		[Test]
		public void DialectSQLFunctions()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			IEnumerator iter = s.Enumerable( "select max(s.Count) from s in class Simple" )
				.GetEnumerator();

			if( dialect is MySQLDialect
				// Added two dialects below for NH
				|| dialect is MsSql2000Dialect
				|| dialect is PostgreSQLDialect )
			{
				Assert.IsTrue( iter.MoveNext() );
				Assert.IsNull( iter.Current );
			}

			Simple simple = new Simple();
			simple.Name = "Simple Dialect Function Test";
			simple.Address = "Simple Address";
			simple.Pay = 45.8f;
			simple.Count = 2;
			s.Save( simple, 10L );

			// Test to make sure allocating an specified object operates correctly.
			Assert.AreEqual( 1,
				s.Find("select new S(s.Count, s.Address) from s in class Simple")
					.Count );

			// Quick check the base dialect functions operate correctly
			Assert.AreEqual( 1, 
				s.Find( "select max(s.Count) from s in class Simple" ).Count );
			Assert.AreEqual( 1,
				s.Find( "select count(*) from s in class Simple" ).Count );

			if ( dialect is OracleDialect) 
			{
				// Check Oracle Dialect mix of dialect functions - no args (no parenthesis and single arg functions
				IList rset = s.Find( "select s.Name, sysdate, trunc(s.Pay), round(s.Pay) from s in class Simple" );
				object[] row = (object[]) rset[0];
				Assert.IsNotNull( row[0], "Name string should have been returned" );
				Assert.IsNotNull( row[1], "Todays Date should have been returned" );
				Assert.AreEqual( 45f, row[2], "trunc(45.8) result was incorrect" );
				Assert.AreEqual( 46f, row[3], "round(45.8) result was incorrect" );

				simple.Pay = -45.8f;
				s.Update(simple);

				// Test type conversions while using nested functions (Float to Int).
				rset = s.Find( "select abs(round(s.Pay)) from s in class Simple" );
				Assert.AreEqual( 46f, rset[0], "abs(round(-45.8)) result was incorrect" );

				// Test a larger depth 3 function example - Not a useful combo other than for testing
				Assert.AreEqual( 1,
					s.Find( "select trunc(round(sysdate)) from s in class Simple" ).Count );

				// Test the oracle standard NVL funtion as a test of multi-param functions...
				// NOTE: commented out for NH, since Pay is a value type and will never be null
				//simple.Pay = null;
				//s.Update( simple );
				//Assert.AreEqual( 0,
				//	s.Find("select MOD( NVL(s.Pay, 5000), 2 ) from Simple as s where s.id = 10")[0] );
			}

			// NOTE: Commented out for NHibernate, no HSQL dialect.
			//if ( (getDialect() is HSQLDialect) ) 
			//{
			//	// Test the hsql standard MOD funtion as a test of multi-param functions...
			//	Integer value = (Integer) s.find("select MOD(s.count, 2) from Simple as s where s.id = 10" ).get(0);
			//	assertTrue( 0 == value.intValue() );
			//}

			s.Delete(simple);
			t.Commit();
			s.Close();
		}

		[Test]
		public void SetProperties() 
		{
			ISession s = OpenSession();
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
		public void Broken()
		{
			if( dialect is Oracle9Dialect ) return;

			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			
			Broken b = new Fixed();
			b.Id = 123;
			b.OtherId = "foobar";
			s.Save( b );
			s.Flush();
			b.Timestamp = DateTime.Now;
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			s.Update( b );
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			b = (Broken) s.Load( typeof( Broken ), b );
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			s.Delete( b );
			t.Commit();
			s.Close();
		}

		[Test]
		public void NothingToUpdate() 
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Simple simple = new Simple();
			simple.Name = "Simple 1";
			s.Save( simple, (long)10 );
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			s.Update( simple, (long)10 );
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			s.Update( simple, (long)10 );
			s.Delete(simple);
			t.Commit();
			s.Close();

		}


		[Test]
		[Ignore("Requires IQuery.SetCacheable")]
		public void CachedQuery()
		{
			/*
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Simple simple = new Simple();
			simple.Name = "Simple 1";
			s.Save( simple, 10L );
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			IQuery q = s.CreateQuery( "from Simple s where s.name=?" );
			q.SetCacheable( true );
			q.SetString( 0, "Simple 1" );

			Assert.AreEqual( 1, q.List().Count );
			Assert.AreEqual( 1, q.List().Count );
			Assert.AreEqual( 1, q.List().Count );

			q = s.CreateQuery( "from Simple s where s.name=:name" );
			q.SetCacheable( true );
			q.SetString( "name", "Simple 1" );
			Assert.AreEqual( 1, q.List().Count );

			simple = (Simple) q.List()[0];

			q.SetString( "name", "Simple 2" );
			Assert.AreEqual( 0, q.List().Count );
			Assert.AreEqual( 0, q.List().Count );
			simple.Name = "Simple 2";
			Assert.AreEqual( 1, q.List().Count );
			Assert.AreEqual( 1, q.List().Count );
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			q = s.CreateQuery("from Simple s where s.name=:name");
			q.SetString( "name", "Simple 2" );
			q.SetCacheable( true );
			Assert.AreEqual( 1, q.List().Count );
			Assert.AreEqual( 1, q.List().Count );
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			s.Update( simple, 10L );
			s.Delete( simple );
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			q = s.CreateQuery("from Simple s where s.name=?");
			q.SetCacheable( true );
			q.SetString( 0, "Simple 1" );
			Assert.AreEqual( 0, q.List().Count );
			Assert.AreEqual( 0, q.List().Count );
			t.Commit();
			s.Close();
			*/
		}

		private string LocateAppropriateDialectFunctionNameForAliasTest()
		{
			foreach( DictionaryEntry de in dialect.Functions )
			{
				ISQLFunction function = (ISQLFunction) de.Value;
				if( !function.HasArguments && !function.HasParenthesesIfNoArguments )
				{
					return (string) de.Key;
				}
			}

			return null;
		}

		[Test]
		public void SQLFunctionAsAlias()
		{
			string functionName = LocateAppropriateDialectFunctionNameForAliasTest();
			if( functionName == null )
			{
				log.Info( "Dialect does not list any no-arg functions" );
				return;
			}

			log.Info( "Using function named [" + functionName + "] for 'function as alias' test" );
			string query = "select " + functionName + " from Simple as " + functionName + " where " + functionName + ".id = 10";

			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Simple simple = new Simple();
			simple.Name = "Simple 1";
			s.Save( simple, 10L );
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			IList result = s.Find(query);
			Assert.IsTrue( result[0] is Simple,
				"Unexpected result type [" + result[0].GetType().Name + "]" );
			s.Delete( result[0] );
			t.Commit();
			s.Close();
		}

		[Test]
		[Ignore("Requires IQuery.SetCacheable")]
		public void CachedQueryOnInsert()
		{
			/*
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Simple simple = new Simple();
			simple.Name = "Simple 1";
			s.Save( simple, 10L );
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			IQuery q = s.CreateQuery( "from Simple s" );
			IList list = q.SetCacheable( true ).List();
			Assert.AreEqual( 1, list.Count );
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			q = s.CreateQuery( "from Simple s" );
			list = q.SetCacheable( true ).List();
			Assert.AreEqual( 1, list.Count );
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			Simple simple2 = new Simple();
			simple2.Count = 133;
			s.Save( simple2, 12L );
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			q = s.CreateQuery( "from Simple s" );
			list = q.SetCacheable( true ).List();
			Assert.AreEqual( 2, list.Count );
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			q = s.CreateQuery( "from Simple s" );
			list = q.SetCacheable( true ).List();
			Assert.AreEqual( 2, list.Count );
			foreach( object obj in list )
			{
				s.Delete( obj );
			}
			t.Commit();
			s.Close();
			*/
		}

		[Test]
		[Ignore("Requires IQuery.SetCacheable and IQuery.SetCacheRegion")]
		public void CachedQueryRegion()
		{
			/*
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Simple simple = new Simple();
			simple.Name = "Simple 1";
			s.Save( simple, 10L );
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			IQuery q = s.CreateQuery( "from Simple s where s.Name=?" );
			q.SetCacheRegion( "foo" );
			q.SetCacheable( true );
			q.SetString( 0, "Simple 1" );
			Assert.AreEqual( 1, q.List().Count );
			Assert.AreEqual( 1, q.List().Count );
			Assert.AreEqual( 1, q.List().Count );
			
			q = s.CreateQuery( "from Simple s where s.Name=:name" );
			q.SetCacheRegion( "foo" );
			q.SetCacheable( true );
			q.SetString( "name", "Simple 1" );
			Assert.AreEqual( 1, q.List().Count );
			simple = (Simple) q.List()[0];

			q.SetString( "name", "Simple 2" );
			Assert.AreEqual( 0, q.List().Count );
			Assert.AreEqual( 0, q.List().Count );
			simple.Name = "Simple 2";
			Assert.AreEqual( 1, q.List().Count );
			Assert.AreEqual( 1, q.List().Count );
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			s.Update( simple, 10L );
			s.Delete( simple );
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			q = s.CreateQuery( "from Simple s where s.Name=?" );
			q.SetCacheRegion( "foo" );
			q.SetCacheable( true );
			q.SetString( 0, "Simple 1" );
			Assert.AreEqual( 0, q.List().Count );
			Assert.AreEqual( 0, q.List().Count );
			t.Commit();
			s.Close();
			*/
		}

		[Test]
		public void SQLFunctions() 
		{
			using( ISession s = OpenSession() )
			{
				ITransaction t = s.BeginTransaction();
				Simple simple = new Simple();
				simple.Name = "Simple 1";
				s.Save( simple, (long)10 );

				if( dialect is Dialect.DB2Dialect ) 
				{
					s.Find("from s in class Simple where repeat('foo', 3) = 'foofoofoo'");
					s.Find("from s in class Simple where repeat(s.Name, 3) = 'foofoofoo'");
					s.Find("from s in class Simple where repeat( lower(s.Name), 3 + (1-1) / 2) = 'foofoofoo'");
				}

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

				if( dialect.SupportsSubSelects )
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

				if( dialect.SupportsSubSelects ) 
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
			}
		}

		
	}
}
