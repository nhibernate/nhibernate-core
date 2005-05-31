using System;
using System.Collections;

using NHibernate.DomainModel;

using NUnit.Framework;

namespace NHibernate.Test
{
	/// <summary>
	/// Summary description for SQLLoaderTest.
	/// </summary>
	[TestFixture]
	public class SQLLoaderTest : TestCase
	{
		protected override IList Mappings
		{
			get
			{
				return new string[]
					{
						"ABC.hbm.xml",
						"Category.hbm.xml",
						"Simple.hbm.xml",
						"Fo.hbm.xml",
						"SingleSeveral.hbm.xml",
						"Componentizable.hbm.xml"
					};
			}
		}

		static int nextInt = 1;
		static long nextLong = 1;

		[Test]
		public void TS()
		{
			if ( dialect is NHibernate.Dialect.Oracle9Dialect )
			{
				return;
			}

			ISession session = OpenSession();

			Simple sim = new Simple();
			sim.Date = DateTime.Today;	// NB We don't use Now() due to the millisecond alignment problem with SQL Server
			session.Save( sim, 1 );
			IQuery q = session.CreateSQLQuery( "select {sim.*} from Simple {sim} where {sim}.date_ = ?", "sim", typeof( Simple ) );
			q.SetTimestamp( 0, sim.Date );
			Assert.AreEqual( 1, q.List().Count, "q.List.Count");
			session.Delete( sim );
			session.Flush();
			session.Close();
		}

		[Test]
		public void TSNamed()
		{
			if ( dialect is NHibernate.Dialect.Oracle9Dialect )
			{
				return;
			}

			ISession session = OpenSession();

			Simple sim = new Simple();
			sim.Date = DateTime.Today;	// NB We don't use Now() due to the millisecond alignment problem with SQL Server
			session.Save( sim, 1 );
			IQuery q = session.CreateSQLQuery( "select {sim.*} from Simple {sim} where {sim}.date_ = :fred", "sim", typeof( Simple ) );
			q.SetTimestamp( "fred", sim.Date );
			Assert.AreEqual( 1, q.List().Count, "q.List.Count");
			session.Delete( sim );
			session.Flush();
			session.Close();
		}

		[Test]
		public void FindBySQLStar()
		{
			ISession session = OpenSession();

			Category s = new Category();
			s.Name = nextLong.ToString();
			nextLong++;
			session.Save( s );

			Simple simple = new Simple();
			simple.Init();
			session.Save( simple, nextLong++ );

			A a = new A();
			session.Save( a );

			//B b = new B();
			//session.Save( b );

			session.CreateSQLQuery( "select {category.*} from Category {category}", "category", typeof( Category ) ).List();
			session.CreateSQLQuery( "select {simple.*} from Simple {simple}", "simple", typeof( Simple ) ).List();
			session.CreateSQLQuery( "select {a.*} from A {a}", "a", typeof( A ) ).List();

			session.Delete( s );
			session.Delete( simple );
			session.Delete( a );
			//session.Delete( b );
			session.Flush();
			session.Close();
		}

		[Test]
		public void FindBySQLProperties()
		{
			ISession session = OpenSession();

			Category s = new Category();
			s.Name = nextLong.ToString();
			nextLong++;
			session.Save( s );

			s = new Category();
			s.Name = "WannaBeFound";
			session.Flush();

			IQuery query = session.CreateSQLQuery( "select {category.*} from Category {category} where {category}.name = :name", "category", typeof( Category ) );
			query.SetProperties( s );

			query.List();

			session.Delete( "from Category" );
			session.Flush();
			session.Close();
		}

		[Test]
		public void FindBySQLAssociatedObject()
		{
			ISession s = OpenSession();

			Category c = new Category();
			c.Name = "NAME";
			Assignable assn = new Assignable();
			assn.Id = "i.d.";
			IList l = new ArrayList();
			l.Add( c );
			assn.Categories = l;
			c.Assignable = assn;
			s.Save( assn );
			s.Flush();
			s.Close();

			s = OpenSession();
			IList list = s.CreateSQLQuery( "select {category.*} from Category {category}", "category", typeof( Category ) ).List();
			Assert.AreEqual( 1, list.Count, "Count differs" );

			s.Delete( "from Assignable" );
			s.Delete( "from Category" );
			s.Flush();
			s.Close();
		}

		[Test]
		public void FindBySQLMultipleObject()
		{
			ISession s = OpenSession();

			Category c = new Category();
			c.Name = "NAME";
			Assignable assn = new Assignable();
			assn.Id = "i.d.";
			IList l = new ArrayList();
			l.Add( c );
			assn.Categories = l;
			c.Assignable = assn;
			s.Save( assn );
			s.Flush();

			c = new Category();
			c.Name = "NAME2";
			assn = new Assignable();
			assn.Id = "i.d.2";
			l = new ArrayList();
			l.Add( c );
			assn.Categories = l;
			c.Assignable = assn;
			s.Save( assn );
			s.Flush();

			assn = new Assignable();
			assn.Id = "i.d.3";
			s.Save( assn );
			s.Flush();
			s.Close();

			s = OpenSession();

			if ( !(dialect is Dialect.MySQLDialect) )
			{
				IList list = s.CreateSQLQuery( "select {category.*}, {assignable.*} from Category {category}, \"assign able\" {assignable}", new string[] { "category", "assignable" }, new System.Type[] { typeof( Category ), typeof( Assignable ) } ).List();
				Assert.AreEqual( 6, list.Count, "Count differs" ); // cross-product of 2 categories x 3 assignables;
				Assert.IsTrue( list[0] is object[] );
			}

			s.Delete( "from Assignable" );
			s.Delete( "from Category" );
			s.Flush();
			s.Close();
		}

		[Test]
		[Ignore("Test not written")]
		public void FindBySQLParameters()
		{
		}

		[Test]
		[Ignore("Test not written")]
		public void EscapedODBC()
		{
		}

		[Test]
		[Ignore("Test not written")]
		public void DoubleAliasing()
		{
		}

		[Test]
		[Ignore("Test not written")]
		public void EmbeddedCompositeProperties()
		{
		}

		[Test]
		[Ignore("Test not written")]
		public void ComponentStar()
		{
		}

		[Test]
		[Ignore("Test not written")]
		public void ComponentNoStar()
		{
		}

		private void ComponentTest()
		{
		}

		[Test]
		[Ignore("Test not written")]
		public void FindSimpleBySQL()
		{
		}

		[Test]
		[Ignore("Test not written")]
		public void FindBySQLSimpleByDiffSessions()
		{
		}

		[Test]
		[Ignore("Test not written")]
		public void FindBySQLDiscriminatorSameSession()
		{
		}

		[Test]
		[Ignore("Test not written")]
		public void FindBySQLDiscriminatedDiffSessions()
		{
		}

		[Test]
		public void NamedSQLQuery()
		{
			if( this.dialect is Dialect.MySQLDialect )
			{
				return;
			}

			ISession s = OpenSession();
			
			Category c = new Category();
			c.Name = "NAME";
			Assignable assn = new Assignable();
			assn.Id = "i.d.";
			IList l = new ArrayList();
			l.Add( c );
			assn.Categories = l;
			c.Assignable = assn;
			s.Save( assn );
			s.Flush();
			s.Close();

			s = OpenSession();
			IQuery q = s.GetNamedQuery( "namedsql" );
			Assert.IsNotNull( q, "should have found 'namedsql'" );
			IList list = q.List();
			Assert.IsNotNull( list, "executing query returns list" );

			object[] values = list[0] as object[];
			Assert.IsNotNull( values[0], "index 0 should not be null" );
			Assert.IsNotNull( values[1], "index 1 should not be null" );

			Assert.AreEqual( typeof(Category), values[0].GetType(), "should be a Category" );
			Assert.AreEqual( typeof(Assignable), values[1].GetType(), "should be Assignable" );
			s.Delete( "from Category" );
			s.Delete( "from Assignable" );
			s.Flush();
			s.Close();

		}
	}
}
