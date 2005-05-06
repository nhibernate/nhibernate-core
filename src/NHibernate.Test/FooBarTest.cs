using System;
using System.Collections;
using System.Data;

using NHibernate;
using NHibernate.DomainModel;

using NUnit.Framework;

namespace NHibernate.Test
{
	[TestFixture]
	public class FooBarTest : TestCase
	{
		// Equivalent of Java String.getBytes()
		private static byte[] GetBytes( string str )
		{
			return System.Text.Encoding.Unicode.GetBytes( str );
		}

		protected override IList Mappings
		{
			get
			{
				return new string[]
					{
						"FooBar.hbm.xml",
						"Baz.hbm.xml",
						"Qux.hbm.xml",
						"Glarch.hbm.xml",
						"Fum.hbm.xml",
						"Fumm.hbm.xml",
						"Fo.hbm.xml",
						"One.hbm.xml",
						"Many.hbm.xml",
						"Immutable.hbm.xml" ,
						"Fee.hbm.xml",
						"Vetoer.hbm.xml",
						"Holder.hbm.xml",
						"Location.hbm.xml",
						"Stuff.hbm.xml",
						"Container.hbm.xml",
						"Simple.hbm.xml",
						"XY.hbm.xml"
					};
			}
		}

		[Test]
		public void CollectionVersioning()
		{
			using( ISession s = OpenSession() )
			{
				One one = new One();
				one.Manies = new Iesi.Collections.HashedSet();
				s.Save(one);
				s.Flush();

				Many many = new Many();
				many.One = one;
				one.Manies.Add( many );
				s.Save( many );
				s.Flush();
				
				Assert.AreEqual( 0, many.V );
				Assert.AreEqual( 1, one.V );

				s.Delete( many );
				s.Delete( one );
				s.Flush();
			}
		}

		[Test]
		public void ForCertain()
		{
			Glarch g = new Glarch();
			Glarch g2 = new Glarch();
			IList strings = new ArrayList();
			strings.Add( "foo" );
			g2.Strings = strings;

			object gid, g2id;

			using( ISession s = OpenSession() )
			using( ITransaction t = s.BeginTransaction() )
			{
				gid = s.Save( g );
				g2id = s.Save( g2 );
				t.Commit();

				Assert.AreEqual( 0, g.Version );
				Assert.AreEqual( 0, g2.Version );
			}

			using( ISession s = OpenSession() )
			using( ITransaction t = s.BeginTransaction() )
			{
				g = (Glarch) s.Get( typeof( Glarch ), gid );
				g2 = (Glarch) s.Get( typeof( Glarch ), g2id );
				Assert.AreEqual( 1, g2.Strings.Count );
			
				s.Delete( g );
				s.Delete( g2 );
				t.Commit();
			}
		}

		[Test]
		public void BagMultipleElements()
		{
			string bazCode;

			using( ISession s = OpenSession() )
			using( ITransaction t = s.BeginTransaction() )
			{
				Baz baz = new Baz();
				baz.Bag = new ArrayList();
				baz.ByteBag = new ArrayList();
				s.Save( baz );
				baz.Bag.Add( "foo" );
				baz.Bag.Add( "bar" );
				baz.ByteBag.Add( GetBytes( "foo" ) );
				baz.ByteBag.Add( GetBytes( "bar" ) );
				t.Commit();

				bazCode = baz.Code;
			}

			using( ISession s = OpenSession() )
			using( ITransaction t = s.BeginTransaction() )
			{
				//put in cache
				Baz baz = (Baz) s.Get( typeof( Baz ), bazCode );
				Assert.AreEqual( 2, baz.Bag.Count );
				Assert.AreEqual( 2, baz.ByteBag.Count );
				t.Commit();
			}

			using( ISession s = OpenSession() )
			using( ITransaction t = s.BeginTransaction() )
			{
				Baz baz = (Baz) s.Get( typeof( Baz ), bazCode );
				Assert.AreEqual( 2, baz.Bag.Count );
				Assert.AreEqual( 2, baz.ByteBag.Count );

				baz.Bag.Remove( "bar" );
				baz.Bag.Add( "foo" );
				baz.ByteBag.Add( GetBytes( "bar" ) );
				t.Commit();
			}

			using( ISession s = OpenSession() )
			using( ITransaction t = s.BeginTransaction() )
			{
				Baz baz = (Baz) s.Get( typeof( Baz ), bazCode );
				Assert.AreEqual( 2, baz.Bag.Count );
				Assert.AreEqual( 3, baz.ByteBag.Count );
				s.Delete(baz);
				t.Commit();
			}
		}

		[Test]
		public void WierdSession()
		{
			object id;

			using( ISession s = OpenSession() )
			using( ITransaction t = s.BeginTransaction() )
			{
				id =  s.Save( new Foo() );
				t.Commit();
			}

			using( ISession s = OpenSession() )
			{
				s.FlushMode = FlushMode.Never;
				using( ITransaction t = s.BeginTransaction() )
				{
					Foo foo = (Foo) s.Get(typeof( Foo ), id);
					t.Commit();
				}

				s.Disconnect();
				s.Reconnect();

				using( ITransaction t = s.BeginTransaction() )
				{
					s.Flush();
					t.Commit();
				}
			}

			using( ISession s = OpenSession() )
			using( ITransaction t = s.BeginTransaction() )
			{
				Foo foo = (Foo) s.Get( typeof( Foo ), id );
				s.Delete( foo );
				t.Commit();
			}
		}

		[Test]
		public void DereferenceLazyCollection()
		{
			string fooKey;
			string bazCode;

			using( ISession s = OpenSession() )
			{
				Baz baz = new Baz();
				baz.FooSet = new Iesi.Collections.HashedSet();
				Foo foo = new Foo();
				baz.FooSet.Add( foo );
				s.Save( foo );
				s.Save( baz );
				foo.Bytes = GetBytes( "foobar" );
				s.Flush();

				fooKey = foo.Key;
				bazCode = baz.Code;
			}

			using( ISession s = OpenSession() )
			{
				Foo foo = (Foo) s.Get( typeof( Foo ), fooKey );
				Assert.IsTrue( NHibernateUtil.IsInitialized( foo.Bytes ) );
				
				// H2.1 has 6 here, but we are using Unicode
				Assert.AreEqual( 12, foo.Bytes.Length );
			
				Baz baz = (Baz) s.Get( typeof( Baz ), bazCode );
				Assert.AreEqual( 1, baz.FooSet.Count );
				s.Flush();
			}

			sessions.EvictCollection("NHibernate.DomainModel.Baz.FooSet");

			using( ISession s = OpenSession() )
			{
				Baz baz = (Baz) s.Get( typeof( Baz ), bazCode );
				Assert.IsFalse( NHibernateUtil.IsInitialized( baz.FooSet ) );
				baz.FooSet = null;
				s.Flush();
			}

			using( ISession s = OpenSession() )
			{
				Foo foo = (Foo) s.Get( typeof( Foo ), fooKey );
				Assert.AreEqual( 12, foo.Bytes.Length );
				Baz baz = (Baz) s.Get( typeof( Baz ), bazCode );
			
				Assert.IsFalse( NHibernateUtil.IsInitialized( baz.FooSet ) );
				Assert.AreEqual( 0, baz.FooSet.Count );
				s.Delete( baz );
				s.Delete( foo );
				s.Flush();
			}
		}

		[Test]
		public void MoveLazyCollection()
		{
			string fooKey, bazCode, baz2Code;

			using( ISession s = OpenSession() )
			{
				Baz baz = new Baz();
				Baz baz2 = new Baz();
				baz.FooSet = new Iesi.Collections.HashedSet();
				Foo foo = new Foo();
				baz.FooSet.Add( foo );
				s.Save( foo );
				s.Save( baz );
				s.Save( baz2 );
				foo.Bytes = GetBytes( "foobar" );
				s.Flush();

				fooKey = foo.Key;
				bazCode = baz.Code;
				baz2Code = baz2.Code;
			}

			using( ISession s = OpenSession() )
			{
				Foo foo = (Foo) s.Get( typeof( Foo ), fooKey );
				Assert.IsTrue( NHibernateUtil.IsInitialized( foo.Bytes ) );
				Assert.AreEqual( 12, foo.Bytes.Length );
				Baz baz = (Baz) s.Get( typeof( Baz ), bazCode );
				Assert.AreEqual( 1, baz.FooSet.Count );
				s.Flush();
			}

			sessions.EvictCollection("NHibernate.DomainModel.Baz.FooSet");

			using( ISession s = OpenSession() )
			{
				Baz baz = (Baz) s.Get( typeof( Baz ), bazCode );
				Assert.IsFalse( NHibernateUtil.IsInitialized( baz.FooSet ) );
				Baz baz2 = (Baz) s.Get( typeof( Baz ), baz2Code );
				baz2.FooSet = baz.FooSet;
				baz.FooSet = null;
				Assert.IsFalse( NHibernateUtil.IsInitialized( baz2.FooSet ) );
				s.Flush();
			}

			using( ISession s = OpenSession() )
			{
				Foo foo = (Foo) s.Get( typeof( Foo ), fooKey );
				Assert.AreEqual( 12, foo.Bytes.Length );
				Baz baz = (Baz) s.Get( typeof( Baz ), bazCode );
				Baz baz2 = (Baz) s.Get( typeof( Baz ), baz2Code );

				Assert.IsFalse( NHibernateUtil.IsInitialized( baz.FooSet ) );
				Assert.AreEqual( 0, baz.FooSet.Count );
				
				// TODO: batching doesn't work in NH yet
				//Assert.IsTrue( NHibernateUtil.IsInitialized( baz2.FooSet ) ); //fooSet has batching enabled
				
				Assert.AreEqual( 1, baz2.FooSet.Count );
				
				s.Delete( baz );
				s.Delete( baz2 );
				s.Delete( foo );
				s.Flush();
			}
		}

		[Test]
		[Ignore("Requires ICriteria.CreateCriteria")]
		public void CriteriaCollection()
		{
		}

		private static bool IsEmpty( IEnumerable enumerable )
		{
			return !enumerable.GetEnumerator().MoveNext();
		}

		private static bool ContainsSingleObject( IEnumerable enumerable, object obj )
		{
			IEnumerator enumerator = enumerable.GetEnumerator();
			
			// Fail if no items
			if( !enumerator.MoveNext() ) return false;
			
			// Fail if item not equal
			if( !object.Equals( obj, enumerator.Current ) ) return false;

			// Fail if more items
			if( enumerator.MoveNext() ) return false;

			// Succeed
			return true;
		}

		[Test]
		public void Query() 
		{
			bool supportsCountDistinct = !( dialect is Dialect.SQLiteDialect );
			ISession s = OpenSession();
			Foo foo = new Foo();
			s.Save( foo );
			Foo foo2 = new Foo();
			s.Save( foo2 );
			foo.TheFoo = foo2;

			IList list = s.Find( "from Foo foo inner join fetch foo.TheFoo" );
			Foo foof = (Foo)list[0];
			Assert.IsTrue( NHibernateUtil.IsInitialized( foof.TheFoo ) );

			list = s.Find( "from Baz baz left outer join fetch baz.FooToGlarch" );
			
			list = s.Find( "select foo, bar from Foo foo left outer join foo.TheFoo bar where foo = ?",
				foo,
				NHibernateUtil.Entity( typeof(Foo) )
				);

			object[] row1 = (object[])list[0];
			Assert.IsTrue( row1[0]==foo && row1[1]==foo2 );

			s.Find( "select foo.TheFoo.TheFoo.String from foo in class Foo where foo.TheFoo = 'bar'" );
			s.Find( "select foo.TheFoo.TheFoo.TheFoo.String from foo in class Foo where foo.TheFoo.TheFoo = 'bar'" );
			s.Find( "select foo.TheFoo.TheFoo.String from foo in class Foo where foo.TheFoo.TheFoo.TheFoo.String = 'bar'" );
//			if( !( dialect is Dialect.HSQLDialect ) ) 
//			{
			s.Find( "select foo.String from foo in class Foo where foo.TheFoo.TheFoo.TheFoo = foo.TheFoo.TheFoo" );
//			}
			s.Find( "select foo.String from foo in class Foo where foo.TheFoo.TheFoo = 'bar' and foo.TheFoo.TheFoo.TheFoo = 'baz'" );
			s.Find( "select foo.String from foo in class Foo where foo.TheFoo.TheFoo.TheFoo.String = 'a' and foo.TheFoo.String = 'b'" );

			s.Find( "from bar in class Bar, foo in elements(bar.Baz.FooArray)" );
			
			if( dialect is Dialect.DB2Dialect ) 
			{
				s.Find( "from foo in class Foo where lower( foo.TheFoo.String ) = 'foo'" );
				s.Find( "from foo in class Foo where lower( (foo.TheFoo.String || 'foo') || 'bar' ) = 'foo'" );
				s.Find( "from foo in class Foo where repeat( (foo.TheFoo.STring || 'foo') || 'bar', 2 ) = 'foo'" );
				s.Find( "From foo in class Bar where foo.TheFoo.Integer is not null and repeat( (foo.TheFoo.String || 'foo') || 'bar', (5+5)/2 ) = 'foo'" );
				s.Find( "From foo in class Bar where foo.TheFoo.Integer is not null or repeat( (foo.TheFoo.String || 'foo') || 'bar', (5+5)/2 ) = 'foo'" );
			}

			if( ( dialect is Dialect.SybaseDialect ) || ( dialect is Dialect.MsSql2000Dialect ) ) 
			{
				s.Enumerable( "select baz from Baz as baz join baz.FooArray foo group by baz order by sum(foo.Float)" );
			}

			s.Find( "from Foo as foo where foo.Component.Glarch.Name is not null" );
			s.Find( "from Foo as foo left outer join foo.Component.Glarch as glarch where glarch.Name = 'foo'" );

			list = s.Find( "from Foo" );
			Assert.AreEqual( 2, list.Count );
			Assert.IsTrue( list[0] is FooProxy );
			list = s.Find( "from Foo foo left outer join foo.TheFoo" );
			Assert.AreEqual( 2, list.Count );
			Assert.IsTrue( ( (object[])list[0])[0] is FooProxy );

			s.Find( "From Foo, Bar" );
			s.Find( "from Baz baz left join baz.FooToGlarch, Bar bar join bar.TheFoo" );
			s.Find( "from Baz baz left join baz.FooToGlarch join baz.FooSet" );
			s.Find( "from Baz baz left join baz.FooToGlarch join fetch baz.FooSet foo left join fetch foo.TheFoo" );

			list = s.Find( "from foo in class NHibernate.DomainModel.Foo where foo.String='osama bin laden' and foo.Boolean = true order by foo.String asc, foo.Component.Count desc" );
			Assert.AreEqual( 0, list.Count, "empty query" );
			IEnumerable enumerable = s.Enumerable( "from foo in class NHibernate.DomainModel.Foo where foo.String='osama bin laden' order by foo.String asc, foo.Component.Count desc" );
			Assert.IsTrue( IsEmpty( enumerable ), "empty enumerator" );

			list = s.Find( "select foo.TheFoo from foo in class NHibernate.DomainModel.Foo" );
			Assert.AreEqual( 1, list.Count, "query" );
			Assert.AreEqual( foo.TheFoo, list[0], "returned object" );
			foo.TheFoo.TheFoo = foo;
			foo.String = "fizard";

			if( dialect.SupportsSubSelects )
			{
				// add an !InterbaseDialect wrapper around list and assert
				list = s.Find( "from foo in class NHibernate.DomainModel.Foo where ? = some foo.Component.ImportantDates.elements", new DateTime( DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day ), NHibernateUtil.DateTime );
				Assert.AreEqual( 2, list.Count, "componenet query" );

				list = s.Find( "from foo in class NHibernate.DomainModel.Foo where size(foo.Component.ImportantDates) = 3" ); 
				Assert.AreEqual( 2, list.Count, "component query" );
				list = s.Find( "from foo in class Foo where 0 = size(foo.Component.ImportantDates)" );
				Assert.AreEqual( 0, list.Count, "component query" );
				list = s.Find( "from foo in class Foo where exists elements(foo.Component.ImportantDates)" );
				Assert.AreEqual( 2, list.Count, "component query" );
				s.Find( "from foo in class Foo where not exists (from bar in class Bar where bar.id = foo.id)" );

				s.Find( "select foo.TheFoo from foo in class Foo where foo = some(select x from x in class Foo where x.Long > foo.TheFoo.Long)" );
				s.Find( "from foo in class Foo where foo = some(select x from x in class Foo where x.Long > foo.TheFoo.Long) and foo.TheFoo.String='baz'" );
				s.Find( "from foo in class Foo where foo.TheFoo.String='baz' and foo = some(select x from x in class Foo where x.Long>foo.TheFoo.Long)" );
				s.Find( "from foo in class Foo where foo = some(select x from x in class Foo where x.Long > foo.TheFoo.Long)" );

				s.Enumerable( "select foo.String, foo.Date, foo.TheFoo.String, foo.id from foo in class Foo, baz in class Baz where foo in elements(baz.FooArray) and foo.String like 'foo'" );
			}

			list = s.Find( "from foo in class Foo where foo.Component.Count is null order by foo.Component.Count" );
			Assert.AreEqual( 0, list.Count, "component query" );
			
			list = s.Find( "from foo in class Foo where foo.Component.Name='foo'" );
			Assert.AreEqual( 2, list.Count, "component query" );
			
			list = s.Find( "select distinct foo.Component.Name, foo.Component.Name from foo in class Foo where foo.Component.Name='foo'" );
			Assert.AreEqual( 1, list.Count, "component query" );

			list = s.Find( "select distinct foo.Component.Name, foo.id from foo in class Foo where foo.Component.Name='foo'" );
			Assert.AreEqual( 2, list.Count, "component query" );

			list = s.Find( "select foo.TheFoo from foo in class Foo" );
			Assert.AreEqual( 2, list.Count, "query" );

			list = s.Find( "from foo in class Foo where foo.id=?", foo.Key, NHibernateUtil.String );
			Assert.AreEqual( 1, list.Count, "id query" );

			list = s.Find("from foo in class Foo where foo.Key=?", foo.Key, NHibernateUtil.String );
			Assert.AreEqual( 1, list.Count, "named id query" );
			Assert.AreSame( foo, list[0], "id query" );

			list = s.Find("select foo.TheFoo from foo in class Foo where foo.String='fizard'");
			Assert.AreEqual( 1, list.Count, "query" );
			Assert.AreSame( foo.TheFoo, list[0], "returned object" );
			
			list = s.Find("from foo in class Foo where foo.Component.Subcomponent.Name='bar'");
			Assert.AreEqual( 2, list.Count, "components of components" );
			
			list = s.Find("select foo.TheFoo from foo in class Foo where foo.TheFoo.id=?",
				foo.TheFoo.Key, NHibernateUtil.String);
			Assert.AreEqual( 1, list.Count, "by id query" );
			Assert.AreSame( foo.TheFoo, list[0], "by id returned object" );

			s.Find( "from foo in class Foo where foo.TheFoo = ?", foo.TheFoo, NHibernateUtil.Entity( typeof( Foo ) ) );

			Assert.IsTrue( IsEmpty( s.Enumerable("from bar in class Bar where bar.String='a string' or bar.String='a string'")
				) );

			enumerable = s.Enumerable(
				"select foo.Component.Name, foo.Component.ImportantDates.elements from foo in class Foo where foo.TheFoo.id=?",
				foo.TheFoo.Key, NHibernateUtil.String
			);
			int i=0;
			foreach( object[] row in enumerable )
			{
				i++;
				Assert.IsTrue( row[0] is String );
				Assert.IsTrue( row[1] == null || row[1] is DateTime );
			}
			Assert.AreEqual( 3, i ); //WAS: 4

			enumerable = s.Enumerable(
				"select max(foo.Component.ImportantDates.elements) from foo in class Foo group by foo.id"
			);
			IEnumerator enumerator = enumerable.GetEnumerator();
			
			Assert.IsTrue( enumerator.MoveNext() );
			Assert.IsTrue( enumerator.Current is DateTime );

			list = s.Find(
				"select foo.TheFoo.TheFoo.TheFoo from foo in class Foo, foo2 in class Foo where"
				+ " foo = foo2.TheFoo and not not ( not foo.String='fizard' )"
				+ " and foo2.String between 'a' and (foo.TheFoo.String)"
				+ ( dialect is Dialect.SQLiteDialect
					? " and ( foo2.String in ( 'fiz', 'blah') or 1=1 )"
					: " and ( foo2.String in ( 'fiz', 'blah', foo.TheFoo.String, foo.String, foo2.String ) )" )
				);
			Assert.AreEqual( 1, list.Count, "complex query" );
			Assert.AreSame( foo, list[0], "returned object" );

			foo.String = "from BoogieDown  -tinsel town  =!@#$^&*())";

			list = s.Find("from foo in class Foo where foo.String='from BoogieDown  -tinsel town  =!@#$^&*())'");
			Assert.AreEqual( 1, list.Count, "single quotes" );

			list = s.Find("from foo in class Foo where not foo.String='foo''bar'");
			Assert.AreEqual( 2, list.Count, "single quotes" );

			list = s.Find("from foo in class Foo where foo.Component.Glarch.Next is null");
			Assert.AreEqual( 2, list.Count, "query association in component" );

			Bar bar = new Bar();
			Baz baz = new Baz();
			baz.SetDefaults();
			bar.Baz = baz;
			baz.ManyToAny = new ArrayList();
			baz.ManyToAny.Add( bar );
			baz.ManyToAny.Add( foo );
			s.Save(bar);
			s.Save(baz);
			list = s.Find(" from bar in class Bar where bar.Baz.Count=667 and bar.Baz.Count!=123 and not bar.Baz.Name='1-E-1'");
			Assert.AreEqual( 1, list.Count, "query many-to-one" );
			list = s.Find(" from i in class Bar where i.Baz.Name='Bazza'");
			Assert.AreEqual( 1, list.Count, "query many-to-one" );

			if( supportsCountDistinct )
			{
				enumerable = s.Enumerable("select count(distinct foo.TheFoo) from foo in class Foo");
				Assert.IsTrue( ContainsSingleObject( enumerable, 2 ), "count" );
			}

			enumerable = s.Enumerable("select count(foo.TheFoo.Boolean) from foo in class Foo");
			Assert.IsTrue( ContainsSingleObject( enumerable, 2 ), "count" );
			
			enumerable = s.Enumerable("select count(*), foo.Int from foo in class Foo group by foo.Int");
			enumerator = enumerable.GetEnumerator();
			Assert.IsTrue( enumerator.MoveNext() );
			Assert.AreEqual( 3, (int) ((object[]) enumerator.Current)[0] );
			Assert.IsFalse( enumerator.MoveNext() );
			
			enumerable = s.Enumerable("select sum(foo.TheFoo.Int) from foo in class Foo");
			Assert.IsTrue( ContainsSingleObject( enumerable, 4 ), "sum" );

			enumerable = s.Enumerable("select count(foo) from foo in class Foo where foo.id=?",
				foo.Key, NHibernateUtil.String);
			Assert.IsTrue( ContainsSingleObject( enumerable, 1 ), "id query count" );

			list = s.Find( "from foo in class Foo where foo.Boolean = ?",
				true, NHibernateUtil.Boolean );

			list = s.Find("select new Foo(fo.X) from Fo fo");
			list = s.Find("select new Foo(fo.Integer) from Foo fo");

			list = s.CreateQuery("select new Foo(fo.X) from Foo fo")
				// TODO: .SetCacheable(true)
				.List();
			Assert.IsTrue(list.Count==3);
			list = s.CreateQuery("select new Foo(fo.X) from Foo fo")
				// TODO: .SetCacheable(true)
				.List();
			Assert.IsTrue(list.Count==3);

			enumerable = s.Enumerable("select new Foo(fo.X) from Foo fo");
			enumerator = enumerable.GetEnumerator();
			Assert.IsTrue( enumerator.MoveNext(), "projection iterate (results)" );
			Assert.IsTrue( typeof(Foo).IsAssignableFrom( enumerator.Current.GetType() ),
				"projection iterate (return check)" );

			// TODO: ScrollableResults not implemented
			//ScrollableResults sr = s.CreateQuery("select new Foo(fo.x) from Foo fo").Scroll();
			//Assert.IsTrue( "projection scroll (results)", sr.next() );
			//Assert.IsTrue( "projection scroll (return check)", typeof(Foo).isAssignableFrom( sr.get(0).getClass() ) );

			list = s.Find("select foo.Long, foo.Component.Name, foo, foo.TheFoo from foo in class Foo");
			Assert.IsTrue( list.Count > 0 );
			foreach( object[] row in list )
			{
				Assert.IsTrue( row[0] is long );
				Assert.IsTrue( row[1] is string );
				Assert.IsTrue( row[2] is Foo );
				Assert.IsTrue( row[3] is Foo );
			}

			if( supportsCountDistinct )
			{
				list = s.Find("select avg(foo.Float), max(foo.Component.Name), count(distinct foo.id) from foo in class Foo");
				Assert.IsTrue( list.Count > 0 );
				foreach( object[] row in list )
				{
					Assert.IsTrue( row[0] is float );
					Assert.IsTrue( row[1] is string );
					Assert.IsTrue( row[2] is int );
				}
			}

			list = s.Find("select foo.Long, foo.Component, foo, foo.TheFoo from foo in class Foo");
			Assert.IsTrue( list.Count > 0 );
			foreach( object[] row in list )
			{
				Assert.IsTrue( row[0] is long );
				Assert.IsTrue( row[1] is FooComponent );
				Assert.IsTrue( row[2] is Foo );
				Assert.IsTrue( row[3] is Foo );
			}

			s.Save( new Holder("ice T") );
			s.Save( new Holder("ice cube") );

			Assert.AreEqual( 15, s.Find("from o in class System.Object").Count );
			System.Console.WriteLine( s.Find("from n in class INamed") );
			Assert.AreEqual( 7, s.Find("from n in class INamed").Count );
			Assert.IsTrue( s.Find("from n in class INamed where n.Name is not null").Count==4 );

			foreach( INamed named in s.Enumerable("from n in class INamed") )
			{
				Assert.IsNotNull( named );
			}

			s.Save( new Holder("bar") );
			enumerable = s.Enumerable("from n0 in class INamed, n1 in class INamed where n0.Name = n1.Name");
			int cnt = 0;
			foreach( object[] row in enumerable )
			{
				if ( row[0] != row[1] ) cnt++;
			}

			//if ( !(dialect is Dialect.HSQLDialect) )
			//{
				Assert.IsTrue(cnt==2);
				Assert.IsTrue( s.Find("from n0 in class INamed, n1 in class INamed where n0.Name = n1.Name").Count==7 );
			//}

			IQuery qu = s.CreateQuery("from n in class INamed where n.Name = :name");
			object temp = qu.ReturnTypes;
			temp = qu.NamedParameters;

			int c = 0;

			foreach( object obj in s.Enumerable("from o in class System.Object") )
			{
				c++;
			}
			Assert.IsTrue(c==16);

			s.Enumerable("select baz.Code, min(baz.Count) from baz in class Baz group by baz.Code");

			Assert.IsTrue( IsEmpty( s.Enumerable( "selecT baz from baz in class Baz where baz.StringDateMap['foo'] is not null or baz.StringDateMap['bar'] = ?",
				new DateTime(), NHibernateUtil.Date ) ) );

			list = s.Find("select baz from baz in class Baz where baz.StringDateMap['now'] is not null");
			Assert.AreEqual( 1, list.Count );
			
			list = s.Find("select baz from baz in class Baz where baz.StringDateMap['now'] is not null and baz.StringDateMap['big bang'] < baz.StringDateMap['now']");
			Assert.AreEqual( 1, list.Count );
			
			list = s.Find("select index(date) from Baz baz join baz.StringDateMap date");
			System.Console.WriteLine(list);
			Assert.AreEqual( 2, list.Count );

			s.Find("from foo in class Foo where foo.Integer not between 1 and 5 and foo.String not in ('cde', 'abc') and foo.String is not null and foo.Integer<=3");

			s.Find("from Baz baz inner join baz.CollectionComponent.Nested.Foos foo where foo.String is null");
			if( dialect.SupportsSubSelects )
			{
				s.Find("from Baz baz inner join baz.FooSet where '1' in (from baz.FooSet foo where foo.String is not null)");
				s.Find("from Baz baz where 'a' in elements(baz.CollectionComponent.Nested.Foos) and 1.0 in elements(baz.CollectionComponent.Nested.Floats)");
				s.Find("from Baz baz where 'b' in baz.CollectionComponent.Nested.Foos.elements and 1.0 in baz.CollectionComponent.Nested.Floats.elements");
			}

			s.Find("from Foo foo join foo.TheFoo where foo.TheFoo in ('1','2','3')");

			//if ( !(dialect is Dialect.HSQLDialect) )
			s.Find("from Foo foo left join foo.TheFoo where foo.TheFoo in ('1','2','3')");
			s.Find("select foo.TheFoo from Foo foo where foo.TheFoo in ('1','2','3')");
			s.Find("select foo.TheFoo.String from Foo foo where foo.TheFoo in ('1','2','3')");
			s.Find("select foo.TheFoo.String from Foo foo where foo.TheFoo.String in ('1','2','3')");
			s.Find("select foo.TheFoo.Long from Foo foo where foo.TheFoo.String in ('1','2','3')");
			s.Find("select count(*) from Foo foo where foo.TheFoo.String in ('1','2','3') or foo.TheFoo.Long in (1,2,3)");
			s.Find("select count(*) from Foo foo where foo.TheFoo.String in ('1','2','3') group by foo.TheFoo.Long");

			s.Find("from Foo foo1 left join foo1.TheFoo foo2 left join foo2.TheFoo where foo1.String is not null");
			s.Find("from Foo foo1 left join foo1.TheFoo.TheFoo where foo1.String is not null");
			s.Find("from Foo foo1 left join foo1.TheFoo foo2 left join foo1.TheFoo.TheFoo foo3 where foo1.String is not null");

			s.Find("select foo.Formula from Foo foo where foo.Formula > 0");

			int len = s.Find("from Foo as foo join foo.TheFoo as foo2 where foo2.id >'a' or foo2.id <'a'").Count;
			Assert.IsTrue(len==2);

			s.Delete("from Holder");
			s.Flush();
			//s.connection().commit();
			s.Close();

			s = OpenSession();
			baz = (Baz) s.CreateQuery("from Baz baz left outer join fetch baz.ManyToAny").UniqueResult();
			Assert.IsTrue( NHibernateUtil.IsInitialized( baz.ManyToAny ) );
			Assert.IsTrue( baz.ManyToAny.Count == 2 );
			BarProxy barp = (BarProxy) baz.ManyToAny[0];
			s.Find("from Baz baz join baz.ManyToAny");
			Assert.IsTrue( s.Find("select baz from Baz baz join baz.ManyToAny a where index(a) = 0").Count==1 );

			FooProxy foop = (FooProxy) s.Get( typeof(Foo), foo.Key );
			Assert.IsTrue( foop == baz.ManyToAny[1] );

			barp.Baz = baz;
			Assert.IsTrue( s.Find("select bar from Bar bar where bar.Baz.StringDateMap['now'] is not null").Count==1 );
			Assert.IsTrue( s.Find("select bar from Bar bar join bar.Baz b where b.StringDateMap['big bang'] < b.StringDateMap['now'] and b.StringDateMap['now'] is not null").Count==1 );
			Assert.IsTrue( s.Find("select bar from Bar bar where bar.Baz.StringDateMap['big bang'] < bar.Baz.StringDateMap['now'] and bar.Baz.StringDateMap['now'] is not null").Count==1 );

			list = s.Find("select foo.String, foo.Component, foo.id from Bar foo");
			Assert.IsTrue ( ( (FooComponent) ( (object[]) list[0] )[1] ).Name == "foo" );
			list = s.Find("select elements(baz.Components) from Baz baz");
			Assert.IsTrue( list.Count==2 );
			list = s.Find("select bc.Name from Baz baz join baz.Components bc");
			Assert.IsTrue( list.Count==2 );
			//list = s.Find("select bc from Baz baz join baz.components bc");

			s.CreateQuery("from Foo foo where foo.Integer < 10 order by foo.String").SetMaxResults(12).List();

			s.Delete( barp );
			s.Delete( baz );
			s.Delete( foop.TheFoo );
			s.Delete( foop );
			s.Flush();
			s.Close();
		}

		[Test]
		public void CascadeDeleteDetached()
		{
			Baz baz;

			using( ISession s = OpenSession() )
			{
				baz = new Baz();
				IList list = new ArrayList();
				list.Add( new Fee() );
				baz.Fees = list;
				s.Save( baz );
				s.Flush();
			}

			using( ISession s = OpenSession() )
			{
				baz = (Baz) s.Get( typeof( Baz ), baz.Code );
			}

			Assert.IsFalse( NHibernateUtil.IsInitialized( baz.Fees ) );

			using( ISession s = OpenSession() )
			{
				s.Delete( baz );
				s.Flush();

				Assert.IsFalse( s.Enumerable( "from Fee" ).GetEnumerator().MoveNext() );
			}

			using( ISession s = OpenSession() )
			{
				baz = new Baz();
				IList list = new ArrayList();
				list.Add( new Fee() );
				list.Add( new Fee() );
				baz.Fees = list;
				s.Save( baz );
				s.Flush();
			}

			using( ISession s = OpenSession() )
			{
				baz = (Baz) s.Get( typeof( Baz ), baz.Code );
				NHibernateUtil.Initialize( baz.Fees );
			}

			Assert.AreEqual( 2, baz.Fees.Count );

			using( ISession s = OpenSession() )
			{
				s.Delete( baz );
				s.Flush();
				Assert.IsTrue( IsEmpty( s.Enumerable( "from Fee" ) ) );
			}
		}

		[Test]
		public void ForeignKeys()
		{
			Baz baz;
			using( ISession s = OpenSession() )
			{
				baz = new Baz();
				Foo foo = new Foo();
				IList bag = new ArrayList();
				bag.Add( foo );
				baz.IdFooBag = bag;
				baz.Foo = foo;
				s.Save( baz );
				s.Flush();
			}

			using( ISession s = OpenSession() )
			{
				baz = (Baz) s.Load( typeof( Baz ), baz.Code );
				s.Delete( baz );
				s.Flush();
			}
		}

		
		[Test]
		[Ignore("IDatabinder not implemented yet")]
		public void Databinder() 
		{
			BarProxy bar;
			FooProxy foo, foo2;
			Baz baz;

			using( ISession s = OpenSession() )
			{
				bar = new Bar();
				s.Save( bar );
				
				foo = new Foo();
				s.Save( foo );
				
				foo2 = new Foo();
				s.Save( foo2 );
				foo2.TheFoo = foo;
				
				baz = new Baz();
				s.Save( baz );
				baz.SetDefaults();
				baz.FooArray = new FooProxy[] { foo, foo2, bar, null };
				bar.TheFoo = foo;
				s.Flush();
			}

			using( ISession s = OpenSession() )
			{
				bar = (BarProxy) s.Load( typeof( Foo ), bar.Key );
				foo = (FooProxy) s.Load( typeof( Foo ), foo.Key );
				foo2 = (FooProxy) s.Load( typeof( Foo ), foo2.Key );
				baz = (Baz) s.Load( typeof( Baz ), baz.Code );

				IDatabinder binder = sessions.OpenDatabinder();
				binder.InitializeLazy = true;
				binder
					.Bind( foo2 )
					.Bind( baz )
					.Bind( foo )
					.Bind( bar );

				Console.WriteLine( binder.ToGenericXml() );
				
				binder
					.Bind( foo2 )
					.Bind( baz );
				
				Console.WriteLine( binder.ToXML() );

				//assertTrue( "dom", binder.toDOM()!=null );
				//assertTrue( "generic dom", binder.toGenericDOM()!=null );

				s.Delete( foo );
				s.Delete( baz );
				s.Delete( bar );
				s.Delete( foo2 );

				s.Flush();
			}
		}


		[Test]
		public void NonlazyCollections()
		{
			object glarchId;

			using( ISession s = OpenSession() )
			{
				Glarch glarch1 = new Glarch();
				glarch1.ProxySet = new Iesi.Collections.ListSet();

				Glarch glarch2 = new Glarch();
				glarch1.ProxySet.Add( glarch1 );

				s.Save( glarch2 );
				glarchId = s.Save( glarch1 );
				s.Flush();
			}

			Glarch loadedGlarch;
			using( ISession s = OpenSession() )
			{
				loadedGlarch = (Glarch)s.Get( typeof( Glarch ), glarchId );
				Assert.IsTrue( NHibernateUtil.IsInitialized( loadedGlarch.ProxySet ) );
			}

			// ProxySet is a non-lazy collection, so this should work outside
			// a session.
			Assert.AreEqual( 1, loadedGlarch.ProxySet.Count );

			using( ISession s = OpenSession() )
			{
				s.Delete( "from Glarch" );
				s.Flush();
			}
		}


		[Test]
		public void ReuseDeletedCollection()
		{
			Baz baz, baz2;

			using( ISession s = OpenSession() )
			{
				baz = new Baz();
				baz.SetDefaults();
				s.Save( baz );
				s.Flush();
				s.Delete(baz);
				
				baz2 = new Baz();
				baz2.StringArray = new string[] { "x-y-z" };
				s.Save( baz2 );
				s.Flush();
			}

			baz2.StringSet   = baz.StringSet;
			baz2.StringArray = baz.StringArray;
			baz2.FooArray    = baz.FooArray;

			using( ISession s = OpenSession() )
			{
				s.Update( baz2 );
				s.Flush();
			}

			using( ISession s = OpenSession() )
			{
				baz2 = (Baz) s.Load( typeof( Baz ), baz2.Code );
				Assert.AreEqual( 3, baz2.StringArray.Length );
				Assert.AreEqual( 3, baz2.StringSet.Count );
				s.Delete( baz2 );
				s.Flush();
			}
		}

		[Test]
		public void PropertyRef()
		{
			object qid;
			object hid;

			using( ISession s = OpenSession() )
			{
				Holder h = new Holder();
				h.Name = "foo";
				Holder h2 = new Holder();
				h2.Name = "bar";
				h.OtherHolder = h2;
				hid = s.Save(h);
				
				Qux q = new Qux();
				q.Holder = h2;
				qid = s.Save( q );
				s.Flush();
			}

			using( ISession s = OpenSession() )
			{
				Holder h = (Holder) s.Load( typeof(Holder), hid );
				Assert.AreEqual( h.Name, "foo" );
				Assert.AreEqual( h.OtherHolder.Name, "bar" );
				object[] res = (object[]) s.Find("from Holder h join h.OtherHolder oh where h.OtherHolder.Name = 'bar'")[0];
				Assert.AreSame( h, res[0] );

				Qux q = (Qux) s.Get( typeof(Qux), qid );
				Assert.AreSame( q.Holder, h.OtherHolder );
				s.Delete( h );
				s.Delete( q );
				s.Flush();
			}
		}
	
		[Test]
		public void QueryCollectionOfValues()
		{
			object gid;

			using( ISession s = OpenSession() )
			{
				Baz baz = new Baz();
				baz.SetDefaults();
				s.Save(baz);
				Glarch g = new Glarch();
				gid = s.Save(g);

				if ( dialect.SupportsSubSelects )
				{
					s.Filter( baz.FooArray, "where size(this.Bytes) > 0");
					s.Filter( baz.FooArray, "where 0 in elements(this.Bytes)");
				}
				s.Flush();
			}

			using( ISession s = OpenSession() )
			{
				//s.Find("from Baz baz where baz.FooSet.String = 'foo'");
				//s.Find("from Baz baz where baz.FooArray.String = 'foo'");
				//s.Find("from Baz baz where baz.FooSet.foo.String = 'foo'");
				//s.Find("from Baz baz join baz.FooSet.Foo foo where foo.String = 'foo'");
				s.Find("from Baz baz join baz.FooSet foo join foo.TheFoo.TheFoo foo2 where foo2.String = 'foo'");
				s.Find("from Baz baz join baz.FooArray foo join foo.TheFoo.TheFoo foo2 where foo2.String = 'foo'");
				s.Find("from Baz baz join baz.StringDateMap date where index(date) = 'foo'");
				s.Find("from Baz baz join baz.TopGlarchez g where index(g) = 'A'");
				s.Find("select index(g) from Baz baz join baz.TopGlarchez g");

				Assert.AreEqual( 3, s.Find("from Baz baz left join baz.StringSet").Count );
				Baz baz = (Baz) s.Find("from Baz baz join baz.StringSet str where str='foo'")[0];
				Assert.IsFalse( NHibernateUtil.IsInitialized( baz.StringSet ) );
				baz = (Baz) s.Find("from Baz baz left join fetch baz.StringSet")[0];
				Assert.IsTrue( NHibernateUtil.IsInitialized( baz.StringSet ) );
				Assert.AreEqual( 1, s.Find("from Baz baz join baz.StringSet string where string='foo'").Count );
				Assert.AreEqual( 1, s.Find("from Baz baz inner join baz.Components comp where comp.Name='foo'").Count );
				//IList bss = s.Find("select baz, ss from Baz baz inner join baz.StringSet ss");
				s.Find("from Glarch g inner join g.FooComponents comp where comp.Fee is not null");
				s.Find("from Glarch g inner join g.FooComponents comp join comp.Fee fee where fee.Count > 0");
				s.Find("from Glarch g inner join g.FooComponents comp where comp.Fee.Count is not null");

				s.Delete(baz);
				//s.delete("from Glarch g");
				s.Delete( s.Get(typeof( Glarch ), gid) );
				s.Flush();
			}
		}

		[Test]
		[Ignore("Batch loading not implemented")]
		public void BatchLoad()
		{
			Baz baz, baz2, baz3;

			using( ISession s = OpenSession() )
			{
				baz = new Baz();
				Iesi.Collections.SortedSet stringSet = new Iesi.Collections.SortedSet();
				stringSet.Add("foo");
				stringSet.Add("bar");
				Iesi.Collections.ISet fooSet = new Iesi.Collections.HashedSet();

				for( int i = 0; i < 3; i++ )
				{
					Foo foo = new Foo();
					s.Save( foo );
					fooSet.Add( foo );
				}

				baz.FooSet = fooSet;
				baz.StringSet = stringSet;
				s.Save( baz );

				baz2 = new Baz();
				fooSet = new Iesi.Collections.HashedSet();
				for( int i = 0; i < 2; i++ )
				{
					Foo foo = new Foo();
					s.Save( foo );
					fooSet.Add( foo );
				}
				baz2.FooSet = fooSet;
				s.Save( baz2 );

				baz3 = new Baz();
				stringSet = new Iesi.Collections.SortedSet();
				stringSet.Add( "foo" );
				stringSet.Add( "baz" );
				baz3.StringSet = stringSet;
				s.Save( baz3 );
				s.Flush();
			}

			using( ISession s = OpenSession() )
			{
				baz = (Baz) s.Load( typeof( Baz ), baz.Code );
				baz2 = (Baz) s.Load( typeof( Baz ), baz2.Code );
				baz3 = (Baz) s.Load( typeof( Baz ), baz3.Code );

				Assert.IsFalse( NHibernateUtil.IsInitialized( baz.FooSet ) );
				Assert.IsFalse( NHibernateUtil.IsInitialized( baz2.FooSet ) );
				Assert.IsFalse( NHibernateUtil.IsInitialized( baz3.FooSet ) );

				Assert.IsFalse( NHibernateUtil.IsInitialized( baz.StringSet ) );
				Assert.IsFalse( NHibernateUtil.IsInitialized( baz2.StringSet ) );
				Assert.IsFalse( NHibernateUtil.IsInitialized( baz3.StringSet ) );

				Assert.AreEqual( 3, baz.FooSet.Count );

				Assert.IsTrue( NHibernateUtil.IsInitialized( baz.FooSet ) );
				Assert.IsTrue( NHibernateUtil.IsInitialized( baz2.FooSet ) );
				Assert.IsTrue( NHibernateUtil.IsInitialized( baz3.FooSet ) );

				Assert.AreEqual( 2, baz2.FooSet.Count );

				Assert.IsTrue( baz3.StringSet.Contains( "baz" ) );

				Assert.IsTrue( NHibernateUtil.IsInitialized( baz.StringSet ) );
				Assert.IsTrue( NHibernateUtil.IsInitialized( baz2.StringSet ) );
				Assert.IsTrue( NHibernateUtil.IsInitialized( baz3.StringSet ) );

				Assert.AreEqual( 2, baz.StringSet.Count );
				Assert.AreEqual( 0, baz2.StringSet.Count );

				s.Delete( baz );
				s.Delete( baz2 );
				s.Delete( baz3 );

				IEnumerable en = new Util.JoinedEnumerable(
					new IEnumerable[] { baz.FooSet, baz2.FooSet } );

				foreach( object obj in en )
				{
					s.Delete( obj );
				}

				s.Flush();
			}
		}

		[Test]
		public void FetchInitializedCollection()
		{
			ISession s = OpenSession();
			Baz baz = new Baz();
			IList fooBag = new ArrayList();
			fooBag.Add( new Foo() );
			fooBag.Add( new Foo() );
			baz.FooBag=fooBag;
			s.Save(baz);
			fooBag = baz.FooBag;
			s.Find("from Baz baz left join fetch baz.FooBag");
			Assert.IsTrue( NHibernateUtil.IsInitialized(fooBag) );
			Assert.IsTrue( fooBag==baz.FooBag );
			Assert.IsTrue( baz.FooBag.Count==2 );
			s.Close();
			
			s = OpenSession();
			baz = (Baz) s.Load( typeof(Baz), baz.Code );
			Object bag = baz.FooBag;
			Assert.IsFalse( NHibernateUtil.IsInitialized(bag) );
			s.Find("from Baz baz left join fetch fooBag");
			Assert.IsFalse( NHibernateUtil.IsInitialized(bag) );
			Assert.IsTrue( bag==baz.FooBag );
			Assert.IsTrue( baz.FooBag.Count==2 );
			s.Delete(baz);
			s.Flush();

			s.Close();
		}

		
		[Test]
		public void LateCollectionAdd()
		{
			object id;

			using( ISession s = OpenSession() )
			{
				Baz baz = new Baz();
				IList l = new ArrayList();
				baz.StringList = l;

				l.Add( "foo" );
				id = s.Save( baz );

				l.Add( "bar" );
				s.Flush();

				l.Add("baz");
				s.Flush();
			}

			using( ISession s = OpenSession() )
			{
				Baz baz = (Baz) s.Load( typeof( Baz ), id );
				Assert.AreEqual( 3, baz.StringList.Count );
				Assert.IsTrue( baz.StringList.Contains( "foo" ) );
				Assert.IsTrue( baz.StringList.Contains( "bar" ) );
				Assert.IsTrue( baz.StringList.Contains( "baz" ) );

				s.Delete( baz );
				s.Flush();
			}
		}

		[Test]
		public void Update() 
		{
			ISession s = OpenSession();
			Foo foo = new Foo();
			s.Save(foo);
			s.Flush();
			s.Close();

			s = OpenSession();
			FooProxy foo2 = (FooProxy)s.Load( typeof(Foo), foo.Key );
			foo2.String = "dirty";
			foo2.Boolean = false;
			foo2.Bytes = new byte[] {1,2,3};
			foo2.Date = DateTime.Today;
			foo2.Short = 69;
			s.Flush();
			s.Close();

			s = OpenSession();
			Foo foo3 = new Foo();
			s.Load( foo3, foo.Key );
			Assert.IsTrue( foo2.EqualsFoo(foo3), "update" );
			s.Delete(foo3);
			s.Flush();
			s.Close();
		}


		[Test]
		public void ListRemove()
		{
			using( ISession s = OpenSession() )
			{
				Baz b = new Baz();
				IList stringList = new ArrayList();
				IList feeList = new ArrayList();
				b.Fees = feeList;
				b.StringList = stringList;
				feeList.Add( new Fee() );
				feeList.Add( new Fee() );
				feeList.Add( new Fee() );
				feeList.Add( new Fee() );
				stringList.Add( "foo" );
				stringList.Add( "bar" );
				stringList.Add( "baz" );
				stringList.Add( "glarch" );
				s.Save( b );
				s.Flush();
				
				stringList.RemoveAt( 1 );
				feeList.RemoveAt( 1 );
				s.Flush();
				
				s.Evict( b );
				s.Refresh( b );
				Assert.AreEqual( 3, b.Fees.Count );
				stringList = b.StringList;
				Assert.AreEqual( 3, stringList.Count );
				Assert.AreEqual( "baz", stringList[1] );
				Assert.AreEqual( "foo", stringList[0] );
				
				s.Delete( b );
				s.Delete( "from Fee" );
				s.Flush();
			}
		}

		[Test]
		public void FetchInitializedCollectionDupe()
		{
			string bazCode;

			using( ISession s = OpenSession() )
			{
				Baz baz = new Baz();
				IList fooBag = new ArrayList();
				fooBag.Add( new Foo() );
				fooBag.Add( new Foo() );
				baz.FooBag = fooBag;
				s.Save( baz );
				s.Flush();
				fooBag = baz.FooBag;
				s.Find("from Baz baz left join fetch baz.FooBag");
				Assert.IsTrue( NHibernateUtil.IsInitialized( fooBag ) );
				Assert.AreSame( fooBag, baz.FooBag );
				Assert.AreEqual( 2, baz.FooBag.Count );

				bazCode = baz.Code;
			}

			using( ISession s = OpenSession() )
			{
				Baz baz = (Baz) s.Load( typeof( Baz ), bazCode );
				object bag = baz.FooBag;
				Assert.IsFalse( NHibernateUtil.IsInitialized( bag ) );
				s.Find("from Baz baz left join fetch baz.FooBag");
				Assert.IsTrue( NHibernateUtil.IsInitialized( bag ) );
				Assert.AreSame( bag, baz.FooBag );
				Assert.AreEqual( 2, baz.FooBag.Count );
				s.Delete( baz );
				s.Flush();
			}
		}

		[Test]
		public void Sortables()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Baz b = new Baz();
			Iesi.Collections.ISet ss = new Iesi.Collections.HashedSet();
			ss.Add( new Sortable("foo") );
			ss.Add( new Sortable("bar") );
			ss.Add( new Sortable("baz") );
			b.Sortablez = ss;
			s.Save(b);
			s.Flush();
			t.Commit();
			s.Close();
		
			s = OpenSession();
			t = s.BeginTransaction();
			IList result = s.CreateCriteria(typeof(Baz))
				.AddOrder( Expression.Order.Asc("Name") )
				.List();
			b = (Baz) result[0];
			Assert.IsTrue( b.Sortablez.Count==3 );
			
			// compare the first item in the "Set" sortablez - can't reference
			// the first item using b.sortablez[0] because it thinks 0 is the
			// DictionaryEntry key - not the index.
			foreach(Sortable sortable in b.Sortablez) 
			{
				Assert.AreEqual( sortable.name, "bar");
				break;
			}
		
			s.Flush();
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			result = s.CreateQuery("from Baz baz left join fetch baz.Sortablez order by baz.Name asc")
				.List();
			b = (Baz) result[0];
			Assert.IsTrue( b.Sortablez.Count==3 );
			foreach(Sortable sortable in b.Sortablez) 
			{
				Assert.AreEqual( sortable.name, "bar");
				break;
			}
			s.Flush();
			t.Commit();
			s.Close();
		
			s = OpenSession();
			t = s.BeginTransaction();
			result = s.CreateQuery("from Baz baz order by baz.Name asc")
				.List();
			b = (Baz) result[0];
			Assert.IsTrue( b.Sortablez.Count==3 );
			foreach(Sortable sortable in b.Sortablez) 
			{
				Assert.AreEqual( sortable.name, "bar");
				break;
			}
			s.Delete(b);
			s.Flush();
			t.Commit();
			s.Close();

		}

		
		[Test]
		public void FetchList() 
		{
			ISession s = OpenSession();
			Baz baz = new Baz();
			s.Save(baz);
			Foo foo = new Foo();
			s.Save(foo);
			Foo foo2 = new Foo();
			s.Save(foo2);
			s.Flush();
			IList list = new ArrayList();
			for ( int i=0; i<5; i++ ) 
			{
				Fee fee = new Fee();
				list.Add(fee);
			}
			baz.Fees = list;
			list = s.Find("from Foo foo, Baz baz left join fetch baz.Fees");
			Assert.IsTrue( NHibernateUtil.IsInitialized( ( (Baz) ( (object[]) list[0] )[1] ).Fees ) );
			s.Delete(foo);
			s.Delete(foo2);
			s.Delete(baz);
			s.Flush();
			s.Close();
		}

		
		[Test]
		public void BagOneToMany() 
		{
			ISession s = OpenSession();
			Baz baz = new Baz();
			IList list = new ArrayList();
			baz.Bazez =list;
			list.Add( new Baz() );
			s.Save(baz);
			s.Flush();
			list.Add( new Baz() );
			s.Flush();
			list.Insert( 0, new Baz() );
			s.Flush();
			object toDelete = list[1];
			list.RemoveAt(1);
			s.Delete( toDelete );
			s.Flush();
			s.Delete(baz);
			s.Flush();
			s.Close();
		}

		
		[Test]
		public void QueryLockMode() 
		{
			ISession s = OpenSession();
			Bar bar = new Bar();
			s.Save(bar);
			s.Flush();

			bar.String = "changed";
			Baz baz = new Baz();
			baz.Foo = bar;
			s.Save(baz);

			IQuery q = s.CreateQuery("from Foo foo, Bar bar");
			q.SetLockMode("bar", LockMode.Upgrade);
			object[] result = (object[])q.List()[0];

			object b = result[0];

			Assert.IsTrue( s.GetCurrentLockMode(b)==LockMode.Write && s.GetCurrentLockMode(result[1])==LockMode.Write);
			s.Flush();
			s.Disconnect();

			s.Reconnect();

			Assert.AreEqual( LockMode.None, s.GetCurrentLockMode(b) );
			s.Find("from Foo foo");
			Assert.AreEqual( LockMode.None, s.GetCurrentLockMode(b) );
			q = s.CreateQuery("from Foo foo");
			q.SetLockMode("foo", LockMode.Read);
			q.List();

			Assert.AreEqual( LockMode.Read, s.GetCurrentLockMode(b) );
			s.Evict(baz);

			s.Disconnect();
			s.Reconnect();

			Assert.AreEqual( LockMode.None, s.GetCurrentLockMode(b) );
			s.Delete( s.Load( typeof(Baz), baz.Code ) );
			Assert.AreEqual( LockMode.None, s.GetCurrentLockMode(b) );

			s.Flush();
			s.Close();

			s = OpenSession();
			q = s.CreateQuery("from Foo foo, Bar bar, Bar bar2");
			q.SetLockMode("bar", LockMode.Upgrade);
			q.SetLockMode("bar2", LockMode.Read);
			result = (object[])q.List()[0];

			Assert.IsTrue( s.GetCurrentLockMode(result[0])==LockMode.Upgrade && s.GetCurrentLockMode(result[1])==LockMode.Upgrade );
			s.Delete(result[0]);
			s.Flush();
			s.Close();
		}


		[Test]
		public void ManyToManyBag() 
		{
			ISession s = OpenSession();
			Baz baz = new Baz();
			object id = s.Save(baz);
			s.Flush();
			s.Close();

			s = OpenSession();
			baz = (Baz)s.Load( typeof(Baz), id );
			baz.FooBag.Add( new Foo() );
			s.Flush();
			s.Close();

			s = OpenSession();
			baz = (Baz)s.Load( typeof(Baz), id );
			Assert.IsFalse( NHibernateUtil.IsInitialized( baz.FooBag ) );
			Assert.AreEqual( 1, baz.FooBag.Count );

			Assert.IsTrue( NHibernateUtil.IsInitialized( baz.FooBag[0] ) );
			s.Delete(baz);
			s.Flush();
			s.Close();
		}


		[Test]
		public void IdBag() 
		{
			ISession s = OpenSession();
			Baz baz = new Baz();
			s.Save(baz);

			IList l = new ArrayList();
			IList l2 = new ArrayList();

			baz.IdFooBag = l;
			baz.ByteBag = l2;

			l.Add( new Foo() );
			l.Add( new Bar() );

			byte[] bytes = GetBytes( "ffo" );
			l2.Add(bytes);
			l2.Add( GetBytes( "foo" ) );
			s.Flush();

			l.Add( new Foo() );
			l.Add( new Bar() );
			l2.Add( GetBytes( "bar" ) );
			s.Flush();
			
			object removedObject = l[3];
			l.RemoveAt(3);
			s.Delete(removedObject);
			
			bytes[1] = Convert.ToByte('o'); 
			s.Flush();
			s.Close();

			s = OpenSession();
			baz = (Baz) s.Load( typeof(Baz), baz.Code );
			Assert.AreEqual( 3, baz.IdFooBag.Count );
			Assert.AreEqual( 3, baz.ByteBag.Count );
			bytes = GetBytes( "foobar" );

			foreach(object obj in baz.IdFooBag) 
			{
				s.Delete(obj);
			}
			baz.IdFooBag = null;
			baz.ByteBag.Add(bytes);
			baz.ByteBag.Add(bytes);
			s.Flush();
			s.Close();

			s = OpenSession();
			baz = (Baz)s.Load( typeof(Baz), baz.Code );
			Assert.AreEqual( 0, baz.IdFooBag.Count );
			Assert.AreEqual( 5, baz.ByteBag.Count );
			s.Delete(baz);
			s.Flush();
			s.Close();

		}


		[Test]
		public void ForceOuterJoin() 
		{
			if( ((Engine.ISessionFactoryImplementor)sessions).IsOuterJoinedFetchEnabled == false ) 
			{
				// don't bother to run the test if we can't test it
				return;
			}

			ISession s = OpenSession();
			Glarch g = new Glarch();
			FooComponent fc = new FooComponent();
			fc.Glarch = g;
			FooProxy f = new Foo();
			FooProxy f2 = new Foo();
			f.Component = fc;
			f.TheFoo = f2;
			s.Save( f2 );
			object id = s.Save( f );
			object gid = s.GetIdentifier( f.Component.Glarch );
			s.Flush();
			s.Close();

			s = OpenSession();
			f = (FooProxy)s.Load( typeof(Foo), id );
			Assert.IsFalse( NHibernateUtil.IsInitialized( f ) );
			Assert.IsTrue( NHibernateUtil.IsInitialized( f.Component.Glarch ) ); //outer-join="true"
			Assert.IsFalse( NHibernateUtil.IsInitialized( f.TheFoo ) ); //outer-join="auto"
			Assert.AreEqual( gid, s.GetIdentifier( f.Component.Glarch ) );
			s.Delete( f );
			s.Delete( f.TheFoo );
			s.Delete( f.Component.Glarch );
			s.Flush();
			s.Close();
			
		}


		[Test]
		public void EmptyCollection()
		{
			ISession s = OpenSession();
			object id = s.Save( new Baz() );
			s.Flush();
			s.Close();
			s = OpenSession();
			Baz baz = (Baz) s.Load(typeof(Baz), id);
			Iesi.Collections.ISet foos = baz.FooSet;
			Assert.IsTrue( foos.Count==0 );
			Foo foo = new Foo();
			foos.Add( foo );
			s.Save(foo);
			s.Flush();
			s.Delete(foo);
			s.Delete(baz);
			s.Flush();
			s.Close();
		}

		
		[Test]
		public void OneToOneGenerator() 
		{
			ISession s = OpenSession();
			X x = new X();
			Y y = new Y();
			x.Y = y;
			y.TheX = x;

			object yId = s.Save(y);
			object xId = s.Save(x);

			Assert.AreEqual( yId, xId);
			s.Flush();

			Assert.IsTrue( s.Contains(y) && s.Contains(x) );
			s.Close();

			Assert.AreEqual( x.Id, y.Id);


			s = OpenSession();
			x = new X();
			y = new Y();

			x.Y = y;
			y.TheX = x;

			s.Save(y);
			s.Flush();

			Assert.IsTrue( s.Contains(y) && s.Contains(x) );
			s.Close();

			Assert.AreEqual( x.Id, y.Id);


			s = OpenSession();
			x = new X();
			y = new Y();
			x.Y = y;
			y.TheX = x;
			xId = s.Save(x);

			Assert.AreEqual(xId, y.Id);
			Assert.AreEqual(xId, x.Id);
			s.Flush();

			Assert.IsTrue( s.Contains(y) && s.Contains(x) );
			s.Delete("from X x");
			s.Flush();
			s.Close();

			// check to see if Y can exist without a X
			y = new Y();
			s = OpenSession();
			s.Save( y );
			s.Flush();
			s.Close();

			s = OpenSession();
			y = (Y)s.Load( typeof(Y), y.Id );
			Assert.IsNull( y.X, "y does not need an X" );
			s.Delete( y );
			s.Flush();
			s.Close();
		}


		[Test]
		public void Limit() 
		{
			ISession s = OpenSession();
			for( int i=0; i<10; i++) 
			{
				s.Save( new Foo() );
			}

			IEnumerable enumerable = s.CreateQuery("from Foo foo")
				.SetMaxResults(4)
				.SetFirstResult(2)
				.Enumerable();

			int count = 0;
			IEnumerator e = enumerable.GetEnumerator();
			while( e.MoveNext() )
			{
				object temp = e.Current;
				count++;
			}

			Assert.AreEqual( 4, count );
			Assert.AreEqual( 10, s.Delete("from Foo foo") );
			s.Flush();
			s.Close();
		}


		[Test]
		public void Custom() 
		{
			GlarchProxy g = new Glarch();
			Multiplicity m = new Multiplicity();
			m.count = 12;
			m.glarch = (Glarch)g;
			g.Multiple = m;
			
			ISession s = OpenSession();
			object gid = s.Save(g);
			s.Flush();
			s.Close();

			s = OpenSession();
			g = (Glarch)s.Find("from Glarch g where g.Multiple.glarch=g and g.Multiple.count=12")[0];
			Assert.IsNotNull( g.Multiple );
			Assert.AreEqual( 12, g.Multiple.count );
			Assert.AreSame( g, g.Multiple.glarch );
			s.Flush();
			s.Close();
			
			s = OpenSession();
			g = (GlarchProxy)s.Load( typeof(Glarch), gid );
			Assert.IsNotNull(g.Multiple);
			Assert.AreEqual( 12, g.Multiple.count );
			Assert.AreSame( g, g.Multiple.glarch );
			s.Delete(g);
			s.Flush();
			s.Close();

		}


		[Test]
		public void SaveAddDelete() 
		{
			ISession s = OpenSession();
			Baz baz = new Baz();
			Iesi.Collections.ISet bars = new Iesi.Collections.HashedSet();
			baz.CascadingBars = bars;
			s.Save(baz);
			s.Flush();

			baz.CascadingBars.Add( new Bar() );
			s.Delete(baz);
			s.Flush();
			s.Close();

		}


		[Test]
		public void NamedParams() 
		{
			Bar bar = new Bar();
			Bar bar2 = new Bar();
			bar.Name = "Bar";
			bar2.Name = "Bar Two";
			Baz baz = new Baz();
			baz.CascadingBars = new Iesi.Collections.HashedSet();
			baz.CascadingBars.Add( bar );
			bar.Baz = baz;

			ISession s = OpenSession();
			s.Save(baz);
			s.Save(bar2);

			IList list = s.Find("from Bar bar left join bar.Baz baz left join baz.CascadingBars b where bar.Name like 'Bar %'");
			object row = list[0];
			Assert.IsTrue( row is object[] && ( (object[])row).Length==3 );

			IQuery q = s.CreateQuery( 
				"select bar, b " +
				"from Bar bar " +
				"left join bar.Baz baz left join baz.CascadingBars b " +
				"where (bar.Name in (:nameList) or bar.Name in (:nameList)) and bar.String = :stringVal" );
			
			IList nameList = new ArrayList();
			nameList.Add( "bar" );
			nameList.Add( "Bar" );
			nameList.Add( "Bar Two" );
			q.SetParameterList( "nameList", nameList );
			q.SetParameter( "stringVal", "a string" );
			list = q.List();
			// a check for SAPDialect here
			Assert.AreEqual( 2, list.Count );

			q = s.CreateQuery( 
				"select bar, b " +
				"from Bar bar " + 
				"inner join bar.Baz baz inner join baz.CascadingBars b " +
				"where bar.Name like 'Bar%'" );
			list = q.List();
			Assert.AreEqual( 1, list.Count );

			q = s.CreateQuery( 
				"select bar, b " +
				"from Bar bar " +
				"left join bar.Baz baz left join baz.CascadingBars b " +
				"where bar.Name like :name and b.Name like :name" );

			// add a check for HSQLDialect
			q.SetString( "name", "Bar%" );
			list = q.List();
			Assert.AreEqual( 1, list.Count );

			s.Delete( baz );
			s.Delete( bar2 );
			s.Flush();
			s.Close();

		}

		[Test, ExpectedException( typeof( QueryException ) )]
		public void VerifyParameterNamedMissing()
		{
			using( ISession s = OpenSession() )
			{
				IQuery q = s.CreateQuery("select bar from Bar as bar where bar.X > :myX");
				q.List();
			}
		}

		[Test, ExpectedException( typeof( QueryException ) )]
		public void VerifyParameterPositionalMissing()
		{
			using( ISession s = OpenSession() )
			{
				IQuery q = s.CreateQuery("select bar from Bar as bar where bar.X > ?");
				q.List();
			}
		}

		[Test]
		public void VerifyParameterPositionalInQuotes()
		{
			using( ISession s = OpenSession() )
			{
				IQuery q = s.CreateQuery("select bar from Bar as bar where bar.X > ? or bar.Short = 1 or bar.String = 'ff ? bb'");
				q.SetInt32(0, 1);
				q.List();
			}
		}

		[Test]
		public void VerifyParameterPositionalInQuotes2()
		{
			using( ISession s = OpenSession() )
			{
				IQuery q = s.CreateQuery("select bar from Bar as bar where bar.String = ' ? ' or bar.String = '?'");
				q.List();
			}
		}

		[Test, ExpectedException( typeof( QueryException ) )]
		public void VerifyParameterPositionalMissing2()
		{
			using( ISession s = OpenSession() )
			{
				IQuery q = s.CreateQuery("select bar from Bar as bar where bar.String = ? or bar.String = ? or bar.String = ?");
				q.SetParameter( 0, "bull" );
				q.SetParameter( 2, "shit" );
				q.List();
			}
		}

		[Test]
		[Ignore("Dynamic components not implemented yet")]
		public void Dyna()
		{
		}

		[Test]
		public void FindByCriteria() 
		{
			ISession s = OpenSession();
			Foo f = new Foo();
			s.Save(f);
			s.Flush();

			IList list = s.CreateCriteria(typeof(Foo))
				.Add( Expression.Expression.Eq( "Integer", f.Integer ) )
				.Add( Expression.Expression.EqProperty("Integer", "Integer") )
				.Add( Expression.Expression.Like( "String", f.String) )
				.Add( Expression.Expression.In("Boolean", new bool[] {f.Boolean, f.Boolean} ) )
				.SetFetchMode("TheFoo", FetchMode.Eager)
				.SetFetchMode("Baz", FetchMode.Lazy)
				.List();

			Assert.IsTrue( list.Count==1 && list[0]==f );

			list = s.CreateCriteria( typeof(Foo) ).Add(
				Expression.Expression.Disjunction()
				.Add( Expression.Expression.Eq( "Integer", f.Integer ) )
				.Add( Expression.Expression.Like( "String", f.String ) )
				.Add( Expression.Expression.Eq( "Boolean", f.Boolean ) )
				)
				.Add( Expression.Expression.IsNotNull("Boolean") )
				.List();

			Assert.IsTrue( list.Count==1 && list[0]==f );

			Expression.ICriterion andExpression;
			Expression.ICriterion orExpression;

			andExpression = Expression.Expression.And( Expression.Expression.Eq( "Integer", f.Integer ), Expression.Expression.Like( "String", f.String ) );
			orExpression = Expression.Expression.Or( andExpression, Expression.Expression.Eq( "Boolean", f.Boolean ) );

			list = s.CreateCriteria(typeof(Foo))
				.Add( orExpression )
				.List();

			Assert.IsTrue( list.Count==1 && list[0]==f );

			
			list = s.CreateCriteria(typeof(Foo))
				.SetMaxResults(5)
				.AddOrder(Expression.Order.Asc("Date"))
				.List();

			Assert.IsTrue(list.Count==1 && list[0]==f);

			list = s.CreateCriteria(typeof(Foo)).SetMaxResults(0).List();
			Assert.AreEqual(0, list.Count);
			
			list = s.CreateCriteria(typeof(Foo))
				.SetFirstResult(1)
				.AddOrder( Expression.Order.Asc("Date") )
				.AddOrder( Expression.Order.Desc("String") )
				.List();

			Assert.AreEqual(0, list.Count);

			f.TheFoo = new Foo();
			s.Save(f.TheFoo);
			s.Flush();
			s.Close();

			//TODO: some HSQLDialect specific code here

			s = OpenSession();
			list = s.CreateCriteria(typeof(Foo))
				.Add( Expression.Expression.Eq( "Integer", f.Integer ) )
				.Add( Expression.Expression.Like( "String", f.String ) )
				.Add( Expression.Expression.In( "Boolean", new bool[] { f.Boolean, f.Boolean } ) )
				.Add( Expression.Expression.IsNotNull("TheFoo") )
				.SetFetchMode("TheFoo", FetchMode.Eager)
				.SetFetchMode("Baz", FetchMode.Lazy)
				.SetFetchMode("Component.Glarch", FetchMode.Lazy)
				.SetFetchMode("TheFoo.Baz", FetchMode.Lazy)
				.SetFetchMode("TheFoo.Component.Glarch", FetchMode.Lazy)
				.List();
			
			f = (Foo) list[0];
			Assert.IsTrue(NHibernateUtil.IsInitialized(f.TheFoo));
			
			Assert.IsFalse( NHibernateUtil.IsInitialized(f.Component.Glarch) );

			s.Delete(f.TheFoo);
			s.Delete(f);
			s.Flush();
			s.Close();
		
		}


		[Test]
		public void AfterDelete() 
		{
			ISession s = OpenSession();
			Foo foo = new Foo();
			s.Save(foo);
			s.Flush();
			s.Delete(foo);
			s.Save(foo);
			s.Delete(foo);
			s.Flush();
			s.Close();
		}


		[Test]
		public void CollectionWhere() 
		{
			Foo foo1 = new Foo();
			Foo foo2 = new Foo();
			Baz baz = new Baz();
			Foo[] arr = new Foo[10];
			arr[0] = foo1;
			arr[9] = foo2;

			ISession s = OpenSession();
			s.Save(foo1);
			s.Save(foo2);
			baz.FooArray = arr;
			s.Save(baz);
			s.Flush();
			s.Close();

			s = OpenSession();
			baz = (Baz)s.Load(typeof(Baz), baz.Code);
			Assert.AreEqual( 1, baz.FooArray.Length );
			Assert.AreEqual( 1, s.Find("from Baz baz, baz.FooArray foo").Count );
			Assert.AreEqual( 2, s.Find("from Foo foo").Count );
			Assert.AreEqual( 1, s.Filter(baz.FooArray, "").Count );

			s.Delete("from Foo foo");
			s.Delete(baz);

			IDbCommand deleteCmd = s.Connection.CreateCommand();
			deleteCmd.CommandText = "delete from FooArray where id_='" + baz.Code + "' and i>=8";
			deleteCmd.CommandType = CommandType.Text;
			int rows = deleteCmd.ExecuteNonQuery();
			Assert.AreEqual(1, rows);

			s.Flush();
			s.Close();

		}
		

		[Test]
		public void ComponentParent()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			BarProxy bar = new Bar();
			bar.Component = new FooComponent();
			Baz baz = new Baz();
			baz.Components = new FooComponent[] { new FooComponent(), new FooComponent()};
			s.Save(bar);
			s.Save(baz);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			
			bar = (BarProxy)s.Load(typeof(Bar), bar.Key);
			s.Load(baz, baz.Code);

			Assert.AreEqual(bar, bar.BarComponent.Parent);
			Assert.IsTrue( baz.Components[0].Baz==baz && baz.Components[1].Baz==baz);

			s.Delete(baz);
			s.Delete(bar);

			t.Commit();
			s.Close();

		}


		[Test]
		public void CollectionCache() 
		{
			ISession s = OpenSession();
			Baz baz = new Baz();
			baz.SetDefaults();
			s.Save(baz);
			s.Flush();
			s.Close();

			s = OpenSession();
			s.Load( typeof(Baz), baz.Code );
			s.Flush();
			s.Close();

			s = OpenSession();
			baz = (Baz) s.Load( typeof(Baz), baz.Code );
			s.Delete(baz);
			s.Flush();
			s.Close();
		}


		[Test]
		//[Ignore("TimeZone Portions commented out - http://jira.nhibernate.org:8080/browse/NH-88")]
		public void AssociationId() 
		{
			string id;
			Bar bar;
			MoreStuff more;

			using( ISession s = OpenSession() )
			using( ITransaction t = s.BeginTransaction() )
			{
				bar = new Bar();
				id = (string)s.Save( bar );
				more = new MoreStuff();
				more.Name = "More Stuff";
				more.IntId = 12;
				more.StringId = "id";
				Stuff stuf = new Stuff();
				stuf.MoreStuff = more;
				more.Stuffs = new ArrayList();
				more.Stuffs.Add(stuf);
				stuf.Foo = bar;
				stuf.Id = 1234;
			
				//TODO: http://jira.nhibernate.org:8080/browse/NH-88
				//stuf.setProperty(TimeZone.getDefault() );
				s.Save(more);
				t.Commit();
			}

			using( ISession s = OpenSession() )
			using( ITransaction t = s.BeginTransaction() )
			{
				//The special property (lowercase) id may be used to reference the unique identifier of an object. (You may also use its property name.) 
				string hqlString = "from s in class Stuff where s.Foo.id = ? and s.id.Id = ? and s.MoreStuff.id.IntId = ? and s.MoreStuff.id.StringId = ?";
				object[] values = new object[] {bar, (long)1234, 12, "id" };
				Type.IType[] types = new Type.IType[]
				{
					NHibernateUtil.Entity(typeof(Foo)),
					NHibernateUtil.Int64,
					NHibernateUtil.Int32,
					NHibernateUtil.String 
				};
		

				IList results = s.Find(hqlString, values, types);
				Assert.AreEqual(1, results.Count);

				hqlString = "from s in class Stuff where s.Foo.id = ? and s.id.Id = ? and s.MoreStuff.Name = ?";
				values = new object[] {bar, (long)1234, "More Stuff"};
				types = new Type.IType[] 
				{
					NHibernateUtil.Entity(typeof(Foo)),
					NHibernateUtil.Int64,
					NHibernateUtil.String
				};
			
				results = s.Find(hqlString, values, types);
				Assert.AreEqual(1, results.Count);


				hqlString = "from s in class Stuff where s.Foo.String is not null";
				s.Find(hqlString);

				hqlString = "from s in class Stuff where s.Foo > '0' order by s.Foo";
				results = s.Find(hqlString);
				Assert.AreEqual(1, results.Count);

				t.Commit();
			}

			FooProxy foo;

			using( ISession s = OpenSession() )
			using( ITransaction t = s.BeginTransaction() )
			{
				foo = (FooProxy)s.Load(typeof(Foo), id);
				s.Load(more, more);
				t.Commit();
			}

			using( ISession s = OpenSession() )
			using( ITransaction t = s.BeginTransaction() )
			{
				Stuff stuff = new Stuff();
				stuff.Foo = foo;
				stuff.Id = 1234;
				stuff.MoreStuff = more;
				s.Load(stuff, stuff);

				// TODO: figure out what to do with TimeZone
				// http://jira.nhibernate.org:8080/browse/NH-88
				//Assert.IsTrue( stuff.getProperty().equals( TimeZone.getDefault() ) );
				Assert.AreEqual("More Stuff", stuff.MoreStuff.Name);
				s.Delete("from ms in class MoreStuff");
				s.Delete("from foo in class Foo");
			
				t.Commit();
			}

		}


		[Test]
		public void CascadeSave() 
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Baz baz = new Baz();
			IList list = new ArrayList();
			list.Add( new Fee() );
			list.Add( new Fee() );
			baz.Fees = list;
			s.Save(baz);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();

			baz = (Baz)s.Load( typeof(Baz), baz.Code );
			Assert.AreEqual(2, baz.Fees.Count);
			s.Delete(baz);

			Assert.IsTrue( IsEmpty( s.Enumerable( "from fee in class Fee" ) ) );
			t.Commit();
			s.Close();
		}


		[Test]
		public void CompositeKeyPathExpressions() 
		{
			ISession s = OpenSession();

			string hql = "select fum1.Fo from fum1 in class Fum where fum1.Fo.FumString is not null";
			s.Find(hql);
			
			hql = "from fum1 in class Fum where fum1.Fo.FumString is not null order by fum1.Fo.FumString";
			s.Find(hql);

			if( dialect.SupportsSubSelects )
			{
				hql = "from fum1 in class Fum where size(fum1.Friends) = 0";
				s.Find(hql);

				hql = "from fum1 in class Fum where exists elements (fum1.Friends)";
				s.Find(hql);
			}

			hql = "select fum1.Friends.elements from fum1 in class Fum";
			s.Find(hql);

			hql = "from fum1 in class Fum, fr in elements( fum1.Friends )";
			s.Find(hql);

			s.Close();
		}


		[Test]
		public void CollectionsInSelect() 
		{
			ISession s = OpenSession();
//			ITransaction t = s.BeginTransaction();
			Foo[] foos = new Foo[] { null, new Foo() };
			s.Save(foos[1]);
			Baz baz = new Baz();
			baz.SetDefaults();
			baz.FooArray = foos;
			s.Save(baz);
			Baz baz2 = new Baz();
			baz2.SetDefaults();
			s.Save(baz2);

			Bar bar = new Bar();
			bar.Baz = baz;
			s.Save(bar);

			IList list = s.Find("select new Result(foo.String, foo.Long, foo.Integer) from foo in class Foo");
			Assert.AreEqual( 2, list.Count );
			Assert.IsTrue( list[0] is Result );
			Assert.IsTrue( list[1] is Result );

			list = s.Find("select new Result( baz.Name, foo.Long, count(elements(baz.FooArray)) ) from Baz baz join baz.FooArray foo group by baz.Name, foo.Long");
			Assert.AreEqual( 1, list.Count );
			Assert.IsTrue( list[0] is Result );
			Result r = (Result)list[0];

			Assert.AreEqual( baz.Name, r.Name );
			Assert.AreEqual( 1, r.Count );
			Assert.AreEqual( foos[1].Long, r.Amount );


			list = s.Find("select new Result( baz.Name, max(foo.Long), count(foo) ) from Baz baz join baz.FooArray foo group by baz.Name");
			Assert.AreEqual(1, list.Count);
			Assert.IsTrue( list[0] is Result );
			r = (Result)list[0];
			Assert.AreEqual( baz.Name, r.Name );
			Assert.AreEqual( 1, r.Count );
			
			// TODO: figure out a better way
			// in hibernate this is hard coded as 696969696969696938l which is very dependant upon 
			// how the test are run because it is calculated on a global static variable...
			// maybe a better way to test this would be to assume that the first 
			//Assert.AreEqual( 696969696969696969L, r.Amount );

			s.Find("select max( elements(bar.Baz.FooArray) ) from Bar as bar");
			// the following test is disable for databases with no subselects... also for Interbase (not sure why) - comment from h2.0.3
			if( dialect.SupportsSubSelects )
			{
				s.Find("select count(*) from Baz as baz where 1 in indices(baz.FooArray)");
				s.Find("select count(*) from Bar as bar where 'abc' in elements(bar.Baz.FooArray)");
				s.Find("select count(*) from Bar as bar where 1 in indices(bar.Baz.FooArray)");
				s.Find("select count(*) from Bar as bar, bar.Component.Glarch.ProxyArray as g where g.id in indices(bar.Baz.FooArray)");
				s.Find("select max( elements(bar.Baz.FooArray) ) from Bar as bar, bar.Component.Glarch.ProxyArray as g where g.id in indices(bar.Baz.FooArray)");
				s.Find("select count(*) from Bar as bar where 1 in (from bar.Component.Glarch.ProxyArray g where g.Name='foo')");
				s.Find("select count(*) from Bar as bar where 1 in (from g in bar.Component.Glarch.ProxyArray.elements where g.Name='foo')");
				
				// TODO: figure out why this is throwing an ORA-1722 error
				if( !( dialect is Dialect.Oracle9Dialect) && !( dialect is Dialect.OracleDialect) ) 
				{
					s.Find("select count(*) from Bar as bar left outer join bar.Component.Glarch.ProxyArray as pg where 1 in (from g in bar.Component.Glarch.ProxyArray)");
				}
			}
            
			list = s.Find("from Baz baz left join baz.FooToGlarch join fetch baz.FooArray foo left join fetch foo.TheFoo");
			Assert.AreEqual( 1, list.Count );
			Assert.AreEqual( 2, ((object[])list[0]).Length );

			list = s.Find("select baz.Name from Bar bar inner join bar.Baz baz inner join baz.FooSet foo where baz.Name = bar.String");
			s.Find("SELECT baz.Name FROM Bar AS bar INNER JOIN bar.Baz AS baz INNER JOIN baz.FooSet AS foo WHERE baz.Name = bar.String");

			s.Find("select baz.Name from Bar bar join bar.Baz baz left outer join baz.FooSet foo where baz.Name = bar.String");

			s.Find("select baz.Name from Bar bar, bar.Baz baz, baz.FooSet foo where baz.Name = bar.String");
			s.Find("SELECT baz.Name FROM Bar AS bar, bar.Baz AS baz, baz.FooSet AS foo WHERE baz.Name = bar.String");

			s.Find("select baz.Name from Bar bar left join bar.Baz baz left join baz.FooSet foo where baz.Name = bar.String");
			s.Find("select foo.String from Bar bar left join bar.Baz.FooSet foo where bar.String = foo.String");
		
			s.Find("select baz.Name from Bar bar left join bar.Baz baz left join baz.FooArray foo where baz.Name = bar.String");
			s.Find("select foo.String from Bar bar left join bar.Baz.FooArray foo where bar.String = foo.String");
		
			s.Find("select bar.String, foo.String from bar in class Bar inner join bar.Baz as baz inner join elements(baz.FooSet) as foo where baz.Name = 'name'");
			s.Find("select foo from bar in class Bar inner join bar.Baz as baz inner join baz.FooSet as foo");
			s.Find("select foo from bar in class Bar inner join bar.Baz.FooSet as foo");
		
			s.Find("select bar.String, foo.String from bar in class Bar, bar.Baz as baz, elements(baz.FooSet) as foo where baz.Name = 'name'");
			s.Find("select foo from bar in class Bar, bar.Baz as baz, baz.FooSet as foo");
			s.Find("select foo from bar in class Bar, bar.Baz.FooSet as foo");

			Assert.AreEqual( 1, s.Find("from Bar bar join bar.Baz.FooArray foo").Count );
			
			Assert.AreEqual( 0, s.Find("from bar in class Bar, foo in bar.Baz.FooSet.elements").Count );
			Assert.AreEqual( 1, s.Find("from bar in class Bar, foo in elements( bar.Baz.FooArray )").Count );

			s.Delete(bar);

			s.Delete(baz);
			s.Delete(baz2);
			s.Delete(foos[1]);

			s.Flush();
//			t.Commit();
			s.Close();
		}


		[Test]
		public void NewFlushing() 
		{
			ISession s = OpenSession();
			Baz baz = new Baz();
			baz.SetDefaults();
			s.Save(baz);
			s.Flush();

			baz.StringArray[0] = "a new value";
			IEnumerator enumer = s.Enumerable("from baz in class Baz").GetEnumerator(); // no flush
			Assert.IsTrue( enumer.MoveNext() );
			Assert.AreSame( baz, enumer.Current );

			enumer = s.Enumerable("select baz.StringArray.elements from baz in class Baz").GetEnumerator();
			bool found = false;
			while (enumer.MoveNext() ) 
			{
				if ( enumer.Current.Equals("a new value") ) 
				{
					found = true;
				}
			}
			Assert.IsTrue(found);

			baz.StringArray = null;
			s.Enumerable("from baz in class Baz"); // no flush
			enumer = s.Enumerable("select baz.StringArray.elements from baz in class Baz").GetEnumerator();
			Assert.IsFalse( enumer.MoveNext() );

			baz.StringList.Add("1E1");
			enumer = s.Enumerable("from foo in class Foo").GetEnumerator(); // no flush
			Assert.IsFalse( enumer.MoveNext() );

			enumer = s.Enumerable("select baz.StringList.elements from baz in class Baz").GetEnumerator();
			found = false;
			while( enumer.MoveNext() ) 
			{
				if( enumer.Current.Equals("1E1") ) 
				{
					found = true;
				}
			}
			Assert.IsTrue(found);

			baz.StringList.Remove("1E1");
			s.Enumerable("select baz.StringArray.elements from baz in class Baz");//no flush
			enumer = s.Enumerable("select baz.StringList.elements from baz in class Baz").GetEnumerator();
			found = false;
			while( enumer.MoveNext() ) 
			{
				if( enumer.Current.Equals("1E1") ) 
				{
					found = true;
				}
			}
			Assert.IsFalse(found);

			IList newList = new ArrayList();
			newList.Add("value");
			baz.StringList = newList;
			enumer = s.Enumerable("from foo in class Foo").GetEnumerator(); //no flush
			baz.StringList = null;
			enumer = s.Enumerable("select baz.StringList.elements from baz in class Baz").GetEnumerator();
			Assert.IsFalse( enumer.MoveNext() );

			s.Delete(baz);
			s.Flush();
			s.Close();

		}


		[Test]
		public void PersistCollections() 
		{
			ISession s = OpenSession();

			IEnumerator enumer = s.Enumerable("select count(*) from b in class Bar").GetEnumerator();
			enumer.MoveNext();
			Assert.AreEqual( 0, enumer.Current );

			Baz baz = new Baz();
			s.Save(baz);
			baz.SetDefaults();
			baz.StringArray = new string[] { "stuff" };
			Iesi.Collections.ISet bars = new Iesi.Collections.HashedSet();
			bars.Add( new Bar() );
			baz.CascadingBars = bars;
			IDictionary sgm = new Hashtable();
			sgm["a"] = new Glarch();
			sgm["b"] = new Glarch();
			baz.StringGlarchMap = sgm;
			s.Flush();
			s.Close();

			s = OpenSession();
			baz = (Baz) ( (object[])s.Find("select baz, baz from baz in class NHibernate.DomainModel.Baz")[0] )[1];
			Assert.AreEqual( 1, baz.CascadingBars.Count, "baz.CascadingBars.Count" );
			Foo foo = new Foo();
			s.Save(foo);
			Foo foo2 = new Foo();
			s.Save(foo2);
			baz.FooArray = new Foo[] { foo, foo, null, foo2 } ;
			baz.FooSet.Add(foo );
			baz.Customs.Add( new string[] { "new", "custom" } );
			baz.StringArray = null;
			baz.StringList[0] = "new value";
			baz.StringSet = new Iesi.Collections.HashedSet();

			// NOTE: We put two items in the map, but expect only one to come back, because
			// of where="..." specified in the mapping for StringGlarchMap
			Assert.AreEqual( 1, baz.StringGlarchMap.Count, "baz.StringGlarchMap.Count" );
			IList list;

			// disable this for dbs with no subselects
			if( dialect.SupportsSubSelects )
			{
				list = s.Find("select foo from foo in class NHibernate.DomainModel.Foo, baz in class NHibernate.DomainModel.Baz where foo in baz.FooArray.elements and 3 = some baz.IntArray.elements and 4 > all baz.IntArray.indices");
				Assert.AreEqual( 2, list.Count, "collection.elements find" );
			}

			// sapdb doesn't like distinct with binary type
			//if( !(dialect is Dialect.SAPDBDialect) ) 
			//{
				list = s.Find("select distinct foo from baz in class NHibernate.DomainModel.Baz, foo in baz.FooArray.elements");
				Assert.AreEqual( 2, list.Count, "collection.elements find" );
			//}

			list = s.Find("select foo from baz in class NHibernate.DomainModel.Baz, foo in baz.FooSet.elements");
			Assert.AreEqual( 1, list.Count, "association.elements find");

			s.Flush();
			s.Close();

			s = OpenSession();
			baz = (Baz)s.Find("select baz from baz in class NHibernate.DomainModel.Baz order by baz")[0];
			Assert.AreEqual(4, baz.Customs.Count, "collection of custom types - added element");
			Assert.IsNotNull(baz.Customs[0], "collection of custom types - added element");
			Assert.IsNotNull(baz.Components[1].Subcomponent, "component of component in collection");
			Assert.AreSame(baz, baz.Components[1].Baz);
			
			IEnumerator fooSetEnumer = baz.FooSet.GetEnumerator();
			fooSetEnumer.MoveNext();
			Assert.IsTrue( ((FooProxy)fooSetEnumer.Current).Key.Equals( foo.Key ) , "set of objects" );
			Assert.AreEqual( 0, baz.StringArray.Length, "collection removed" );
			Assert.AreEqual( "new value", baz.StringList[0], "changed element" );
			Assert.AreEqual( 0, baz.StringSet.Count, "replaced set" );
			
			baz.StringSet.Add( "two" );
			baz.StringSet.Add( "one" );
			baz.Bag.Add("three");
			s.Flush();
			s.Close();

			s = OpenSession();
			baz = (Baz)s.Find("select baz from baz in class NHibernate.DomainModel.Baz order by baz")[0];
			Assert.AreEqual( 2, baz.StringSet.Count );
			int index = 0;
			foreach(string key in baz.StringSet ) 
			{
				// h2.0.3 doesn't have this because the Set has a first() and last() method
				index++;
				if(index==1) Assert.AreEqual( "one", key );
				if(index==2) Assert.AreEqual( "two", key );
				if(index>2) Assert.Fail("should not be more than 2 items in StringSet");
			}
			Assert.AreEqual( 5, baz.Bag.Count );
			baz.StringSet.Remove("two");
			baz.Bag.Remove("duplicate");
			s.Flush();
			s.Close();

			s = OpenSession();
			baz = (Baz)s.Load( typeof(Baz), baz.Code );
			Bar bar = new Bar();
			Bar bar2 = new Bar();
			s.Save(bar);
			s.Save(bar2);
			baz.TopFoos = new Iesi.Collections.HashedSet();
			baz.TopFoos.Add( bar );
			baz.TopFoos.Add( bar2 );
			baz.TopGlarchez = new Hashtable();
			GlarchProxy g = new Glarch();
			s.Save(g);
			baz.TopGlarchez['G'] = g;
			Hashtable map = new Hashtable();
			map[bar] = g;
			map[bar2] = g;
			baz.FooToGlarch = map;
			map = new Hashtable();
			map[new FooComponent("name", 123, null, null)] = bar;
			map[new FooComponent("nameName", 12, null, null)] = bar;
			baz.FooComponentToFoo = map;
			map = new Hashtable();
			map[bar] = g;
			baz.GlarchToFoo = map;
			s.Flush();
			s.Close();
			
			s = OpenSession();
			baz = (Baz)s.Find("select baz from baz in class NHibernate.DomainModel.Baz order by baz")[0];
			ISession s2 = OpenSession();
			baz = (Baz)s.Find("select baz from baz in class NHibernate.DomainModel.Baz order by baz")[0];
			object o = baz.FooComponentToFoo[new FooComponent("name", 123, null, null)];
			Assert.IsNotNull( o );
			Assert.AreEqual( o, baz.FooComponentToFoo[new FooComponent("nameName", 12, null, null)] );
			s2.Close();
			Assert.AreEqual( 2, baz.TopFoos.Count );
			Assert.AreEqual( 1, baz.TopGlarchez.Count );
			enumer = baz.TopFoos.GetEnumerator();
			Assert.IsTrue( enumer.MoveNext() );
			Assert.IsNotNull( enumer.Current );
			Assert.AreEqual( 1, baz.StringSet.Count );
			Assert.AreEqual( 4, baz.Bag.Count );
			Assert.AreEqual( 2, baz.FooToGlarch.Count );
			Assert.AreEqual( 2, baz.FooComponentToFoo.Count );
			Assert.AreEqual( 1, baz.GlarchToFoo.Count );

			enumer = baz.FooToGlarch.Keys.GetEnumerator();
			for( int i=0; i<2; i++ ) 
			{
				enumer.MoveNext();
				Assert.IsTrue( enumer.Current is BarProxy );
			}
			enumer = baz.FooComponentToFoo.Keys.GetEnumerator();
			enumer.MoveNext();
			FooComponent fooComp = (FooComponent)enumer.Current;
			Assert.IsTrue (
				(fooComp.Count==123 && fooComp.Name.Equals("name"))
				|| (fooComp.Count==12 && fooComp.Name.Equals("nameName"))
				);
			Assert.IsTrue( baz.FooComponentToFoo[fooComp] is BarProxy );

			Glarch g2 = new Glarch();
			s.Save(g2);
			g = (GlarchProxy)baz.TopGlarchez['G'];
			baz.TopGlarchez['H'] = g;
			baz.TopGlarchez['G'] = g2;
			s.Flush();
			s.Close();

			s = OpenSession();
			baz = (Baz)s.Find("select baz from baz in class NHibernate.DomainModel.Baz order by baz")[0];
			Assert.AreEqual( 2, baz.TopGlarchez.Count );
			s.Disconnect();
				
			// serialize and then deserialize the session.
			System.IO.Stream stream = new System.IO.MemoryStream();
			System.Runtime.Serialization.IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
			formatter.Serialize(stream, s);

			s.Close();

			stream.Position = 0;
			s = (ISession)formatter.Deserialize(stream);
			stream.Close();

			s.Reconnect();
			baz = (Baz)s.Load( typeof(Baz), baz.Code );
			s.Delete(baz);
			s.Delete(baz.TopGlarchez['G']);
			s.Delete(baz.TopGlarchez['H']);

			IDbCommand cmd = s.Connection.CreateCommand();
			cmd.CommandText = "update " + dialect.QuoteForTableName("glarchez") + " set baz_map_id=null where baz_map_index='a'";
			int rows = cmd.ExecuteNonQuery();
			Assert.AreEqual( 1, rows );
			Assert.AreEqual( 1, s.Delete("from bar in class NHibernate.DomainModel.Bar") );
			FooProxy[] arr = baz.FooArray;
			Assert.AreEqual( 4, arr.Length );
			Assert.AreEqual( foo.Key, arr[1].Key );
			for(int i=0; i<arr.Length; i++) 
			{
				if(arr[i]!=null) 
				{
					s.Delete(arr[i]);
				}
			}

			try 
			{
				s.Load( typeof(Qux), (long)666 ); //nonexistent
			}
			catch(ObjectNotFoundException onfe) 
			{
				Assert.IsNotNull(onfe, "should not find a Qux with id of 666 when Proxies are not implemented.");
			}

			Assert.AreEqual( 1, s.Delete("from g in class Glarch"), "Delete('from g in class Glarch')" );
			s.Flush();
			s.Disconnect();

			// serialize and then deserialize the session.
			stream = new System.IO.MemoryStream();
			formatter.Serialize( stream, s );

			s.Close();

			stream.Position = 0;
			s = (ISession)formatter.Deserialize(stream);
			stream.Close();

			Qux nonexistentQux = (Qux)s.Load( typeof(Qux), (long)666 ) ; //nonexistent
			Assert.IsNotNull( nonexistentQux, "even though it doesn't exists should still get a proxy - no db hit." );

			s.Close();
		}


		[Test]
		public void SaveFlush() 
		{
			ISession s = OpenSession();
			Fee fee = new Fee();
			s.Save( fee, "key" );
			fee.Fi = "blah";
			s.Flush();
			s.Close();

			s = OpenSession();
			fee = (Fee)s.Load( typeof(Fee), fee.Key );
			Assert.AreEqual( "blah", fee.Fi );
			Assert.AreEqual( "key", fee.Key );
			s.Delete(fee);
			s.Flush();
			s.Close();
		}


		[Test]
		public void CreateUpdate() 
		{
			ISession s = OpenSession();
			Foo foo = new Foo();
			s.Save(foo);
			foo.String = "dirty";
			s.Flush();
			s.Close();

			s = OpenSession();
			Foo foo2 = new Foo();
			s.Load( foo2, foo.Key );
			Assert.IsTrue( foo.EqualsFoo(foo2), "create-update" );
			s.Delete(foo2);
			s.Flush();
			s.Close();

			s = OpenSession();
			foo = new Foo();
			s.Save(foo, "assignedid");
			foo.String = "dirty";
			s.Flush();
			s.Close();

			s = OpenSession();
			s.Load(foo2, "assignedid");
			Assert.IsTrue( foo.EqualsFoo(foo2), "create-update" );
			s.Delete(foo2);
			s.Flush();
			s.Close();


		}


		[Test]
		public void UpdateCollections() 
		{
			ISession s = OpenSession();
			Holder baz = new Holder();
			baz.Name = "123";
			Foo f1 = new Foo();
			Foo f2 = new Foo();
			Foo f3 = new Foo();
			One o = new One();
			baz.Ones = new ArrayList();
			baz.Ones.Add(o);
			Foo[] foos = new Foo[] { f1, null, f2 };
			baz.FooArray = foos;
			// in h2.0.3 this is a Set
			baz.Foos = new Iesi.Collections.HashedSet();
			baz.Foos.Add( f1 );
			s.Save(f1);
			s.Save(f2);
			s.Save(f3);
			s.Save(o);
			s.Save(baz);
			s.Flush();
			s.Close();

			baz.Ones[0] = null;
			baz.Ones.Add(o);
			baz.Foos.Add( f2 );
			foos[0] = f3;
			foos[1] = f1;

			s = OpenSession();
			s.SaveOrUpdate(baz);
			s.Flush();
			s.Close();

			s = OpenSession();
			Holder h = (Holder)s.Load( typeof(Holder), baz.Id );
			Assert.IsNull( h.Ones[0] );
			Assert.IsNotNull( h.Ones[1] );
			Assert.IsNotNull( h.FooArray[0] );
			Assert.IsNotNull( h.FooArray[1] );
			Assert.IsNotNull( h.FooArray[2] );
			Assert.AreEqual( 2, h.Foos.Count );
			
			// new to nh to make sure right items in right index
			Assert.AreEqual( f3.Key, h.FooArray[0].Key ); 
			Assert.AreEqual( o.Key, ((One)h.Ones[1]).Key ); 
			Assert.AreEqual ( f1.Key, h.FooArray[1].Key );
			Assert.AreEqual ( f2.Key, h.FooArray[2].Key );
			s.Close();

			baz.Foos.Remove(f1);
			baz.Foos.Remove(f2);
			baz.FooArray[0] = null;
			baz.FooArray[1] = null;
			baz.FooArray[2] = null;
			
			s = OpenSession();
			s.SaveOrUpdate(baz);
			s.Delete("from f in class NHibernate.DomainModel.Foo");
			baz.Ones.Remove(o);
			s.Delete("from o in class NHibernate.DomainModel.One");
			s.Delete(baz);
			s.Flush();
			s.Close();
		}


		[Test]
		public void Load() 
		{
			ISession s = OpenSession();
			Qux q = new Qux();
			s.Save(q);
			BarProxy b = new Bar();
			s.Save(b);
			s.Flush();
			s.Close();

			s = OpenSession();
			q = (Qux)s.Load( typeof(Qux), q.Key );
			b = (BarProxy)s.Load( typeof(Foo), b.Key );
			string tempKey = b.Key;
			Assert.IsFalse( NHibernateUtil.IsInitialized(b), "b should have been an unitialized Proxy" );
			string tempString = b.BarString;
			Assert.IsTrue( NHibernateUtil.IsInitialized(b), "b should have been an initialized Proxy" );
			BarProxy b2 = (BarProxy)s.Load( typeof(Bar), tempKey );
			Qux q2 = (Qux)s.Load( typeof(Qux), q.Key );
			Assert.AreSame( q, q2, "loaded same Qux" );
			Assert.AreSame( b, b2, "loaded same BarProxy" );
			s.Delete(q2);
			s.Delete(b2);
			s.Flush();
			s.Close();

		}


		[Test]
		public void Create() 
		{
			ISession s = OpenSession();
			Foo foo = new Foo();
			s.Save(foo);
			s.Flush();
			s.Close();

			s = OpenSession();
			Foo foo2 = new Foo();
			s.Load( foo2, foo.Key );

			Assert.IsTrue( foo.EqualsFoo(foo2), "create" );
			s.Delete(foo2);
			s.Flush();
			s.Close();
			
		}


		[Test]
		public void Callback() 
		{
			ISession s = OpenSession();
			Qux q = new Qux("0");
			s.Save(q);
			q.Child = ( new Qux("1") );
			s.Save( q.Child );
			
			Qux q2 = new Qux("2");
			q2.Child = q.Child;
			
			Qux q3 = new Qux("3");
			q.Child.Child = q3;
			s.Save(q3);

			Qux q4 = new Qux("4");
			q4.Child = q3;

			s.Save(q4);
			s.Save(q2);
			s.Flush();
			s.Close();

			s = OpenSession();
			IList l = s.Find("from q in class NHibernate.DomainModel.Qux");
			Assert.AreEqual(5, l.Count);
			
			s.Delete( l[0] );
			s.Delete( l[1] );
			s.Delete( l[2] );
			s.Delete( l[3] );
			s.Delete( l[4] );
			s.Flush();
			s.Close();
		}


		[Test]
		public void Polymorphism() 
		{
			ISession s = OpenSession();
			Bar bar = new Bar();
			s.Save(bar);
			bar.BarString = "bar bar";
			s.Flush();
			s.Close();

			s = OpenSession();
			FooProxy foo = (FooProxy)s.Load( typeof(Foo), bar.Key );
			Assert.IsTrue( foo is BarProxy, "polymorphic" );
			Assert.IsTrue( ((BarProxy)foo).BarString.Equals( bar.BarString ), "subclass property" );
			s.Delete(foo);
			s.Flush();
			s.Close();
										  
		}


		[Test]
		public void RemoveContains() 
		{
			ISession s = OpenSession();
			Baz baz = new Baz();
			baz.SetDefaults();
			s.Save(baz);
			s.Flush();

			Assert.IsTrue( s.Contains(baz) );
			
			s.Evict(baz);
			Assert.IsFalse( s.Contains(baz), "baz should have been evicted" );

			Baz baz2 = (Baz)s.Load( typeof(Baz), baz.Code );
			Assert.IsFalse(baz==baz2, "should be different objects because Baz not contained in Session");

			s.Delete(baz2);
			s.Flush();
			s.Close();
		}


		[Test]
		public void CollectionOfSelf() 
		{
			ISession s = OpenSession();
			Bar bar = new Bar();
			s.Save(bar);
			// h2.0.3 was a set
			bar.Abstracts = new Iesi.Collections.HashedSet();
			bar.Abstracts.Add( bar );
			Bar bar2 = new Bar();
			bar.Abstracts.Add( bar2 );
			bar.TheFoo = bar;
			s.Save(bar2);
			s.Flush();
			s.Close();

			bar.Abstracts = null;
			s = OpenSession();
			s.Load( bar, bar.Key );

			Assert.AreEqual( 2, bar.Abstracts.Count);
			Assert.IsTrue(bar.Abstracts.Contains(bar), "collection contains self");
			Assert.AreSame(bar, bar.TheFoo, "association to self");

			foreach(object obj in bar.Abstracts) 
			{
				s.Delete(obj);
			}

			s.Flush();
			s.Close();

		}


		[Test]
		public void Find() 
		{
			ISession s = OpenSession();

			// some code commented out in h2.0.3

			Bar bar = new Bar();
			s.Save( bar );
			bar.BarString = "bar bar";
			bar.String = "xxx";
			Foo foo = new Foo();
			s.Save( foo );
			foo.String = "foo bar";
			s.Save( new Foo() );
			s.Save( new Bar() );
	
			IList list1 = s.Find( "select foo from foo in class NHibernate.DomainModel.Foo where foo.String='foo bar'" );
			Assert.AreEqual( 1, list1.Count, "find size" );
			Assert.AreSame( foo, list1[0], "find ==" );

			IList list2 = s.Find( "from foo in class NHibernate.DomainModel.Foo order by foo.String, foo.Date" );
			Assert.AreEqual( 4, list2.Count, "find size" );
			list1 = s.Find( "from foo in class NHibernate.DomainModel.Foo where foo.class='B'" );
			Assert.AreEqual( 2, list1.Count, "class special property" );
			list1 = s.Find( "from foo in class NHibernate.DomainModel.Foo where foo.class=NHibernate.DomainModel.Bar" );
			Assert.AreEqual( 2, list1.Count, "class special property" );
			list1 = s.Find( "from foo in class NHibernate.DomainModel.Foo where foo.class=Bar" );
			list2 = s.Find( "select bar from bar in class NHibernate.DomainModel.Bar, foo in class NHibernate.DomainModel.Foo where bar.String = foo.String and not bar=foo" );
			
			Assert.AreEqual( 2, list1.Count, "class special property" );
			Assert.AreEqual( 1, list2.Count, "select from a subclass" );

			Trivial t= new Trivial();
			s.Save( t );
			s.Flush();
			s.Close();

			s = OpenSession();
			list1 = s.Find( "from foo in class NHibernate.DomainModel.Foo where foo.String='foo bar'" );
			Assert.AreEqual( 1, list1.Count, "find count" );
			// There is an interbase bug that causes null integers to return as 0, also numeric precision is <=15 -h2.0.3 comment
			Assert.IsTrue( ( (Foo)list1[0]).EqualsFoo( foo ), "find equals" );
			list2 = s.Find( "select foo from foo in class NHibernate.DomainModel.Foo" );
			Assert.AreEqual( 5, list2.Count, "find count" );
			IList list3 = s.Find( "from bar in class NHibernate.DomainModel.Bar where bar.BarString='bar bar'" );
			Assert.AreEqual( 1, list3.Count, "find count" );
			Assert.IsTrue( list2.Contains( list1[0] ) && list2.Contains( list2[0] ), "find same instance" );
			Assert.AreEqual( 1, s.Find( "from t in class NHibernate.DomainModel.Trivial").Count );
			s.Delete( "from t in class NHibernate.DomainModel.Trivial" );

			list2 = s.Find( "from foo in class NHibernate.DomainModel.Foo where foo.Date = ?", new DateTime(1970, 01, 01), NHibernateUtil.Date );
			Assert.AreEqual( 4, list2.Count, "find by date" );
			IEnumerator enumer = list2.GetEnumerator();
			while( enumer.MoveNext() ) 
			{
				s.Delete( enumer.Current );
			}

			list2 = s.Find( "from foo in class NHibernate.DomainModel.Foo" );
			Assert.AreEqual( 0, list2.Count, "find deleted" );
			s.Flush();
			s.Close();
		}

	
		[Test]
		public void DeleteRecursive() 
		{
			ISession s = OpenSession();
			Foo x = new Foo();
			Foo y = new Foo();
			x.TheFoo = y;
			y.TheFoo = x;
			s.Save(x);
			s.Save(y);
			s.Flush();
			s.Delete(y);
			s.Delete(x);
			s.Flush();
			s.Close();
		}


		[Test]
		public void Reachability() 
		{
			// first for unkeyed collections
			ISession s = OpenSession();
			Baz baz1 = new Baz();
			s.Save(baz1);
			Baz baz2 = new Baz();
			s.Save(baz2);
			baz1.IntArray = new int[] {1, 2, 3, 4};
			baz1.FooSet = new Iesi.Collections.HashedSet();
			Foo foo = new Foo();
			s.Save(foo);
			baz1.FooSet.Add( foo );
			s.Flush();
			s.Close();

			s = OpenSession();
			baz2 = (Baz) s.Load( typeof(Baz), baz2.Code );
			baz1 = (Baz) s.Load( typeof(Baz), baz1.Code );
			baz2.FooSet = baz1.FooSet;
			baz1.FooSet = null;
			baz2.IntArray = baz1.IntArray;
			baz1.IntArray = null;
			s.Flush();
			s.Close();

			s = OpenSession();
			baz2 = (Baz)s.Load( typeof(Baz), baz2.Code );
			baz1 = (Baz)s.Load( typeof(Baz), baz1.Code );
			Assert.AreEqual(4, baz2.IntArray.Length, "unkeyed reachability - baz2.IntArray");
			Assert.AreEqual(1, baz2.FooSet.Count, "unkeyed reachability - baz2.FooSet");
			Assert.AreEqual(0, baz1.IntArray.Length, "unkeyed reachability - baz1.IntArray");
			Assert.AreEqual(0, baz1.FooSet.Count, "unkeyed reachability - baz1.FooSet");

			foreach(object obj in baz2.FooSet) 
			{
				s.Delete( (FooProxy)obj );
			}

			s.Delete(baz1);
			s.Delete(baz2);
			s.Flush();
			s.Close();

			// now for collections of collections
			s = OpenSession();
			baz1 = new Baz();
			s.Save(baz1);
			baz2 = new Baz();
			s.Save(baz2);
			s.Flush();
			s.Close();

			s = OpenSession();
			baz2 = (Baz)s.Load( typeof(Baz), baz2.Code );
			baz1 = (Baz)s.Load( typeof(Baz), baz1.Code );
			s.Flush();
			s.Close();

			s = OpenSession();
			baz2 = (Baz)s.Load( typeof(Baz), baz2.Code );
			s.Flush();
			s.Close();

			s = OpenSession();
			baz2 = (Baz)s.Load( typeof(Baz), baz2.Code );
			baz1 = (Baz)s.Load( typeof(Baz), baz1.Code );
			s.Delete(baz1);
			s.Delete(baz2);
			s.Flush();
			s.Close();

			// now for keyed collections
			s = OpenSession();
			baz1 = new Baz();
			s.Save(baz1);
			baz2 = new Baz();
			s.Save(baz2);
			Foo foo1 = new Foo();
			Foo foo2 = new Foo();

			s.Save(foo1);
			s.Save(foo2);
			baz1.FooArray = new Foo[] { foo1, null, foo2 };
			baz1.StringDateMap = new Hashtable();
			baz1.StringDateMap["today"] = DateTime.Today;
			baz1.StringDateMap["tomm"] = new DateTime(DateTime.Today.Ticks +  (new TimeSpan(1, 0, 0, 0, 0)).Ticks);
			s.Flush();
			s.Close();

			s = OpenSession();
			baz2 = (Baz)s.Load( typeof(Baz), baz2.Code );
			baz1 = (Baz)s.Load( typeof(Baz), baz1.Code );
			baz2.FooArray = baz1.FooArray;
			baz1.FooArray = null;
			baz2.StringDateMap = baz1.StringDateMap;
			baz1.StringDateMap = null;
			s.Flush();
			s.Close();

			s = OpenSession();
			baz2 = (Baz)s.Load( typeof(Baz), baz2.Code );
			baz1 = (Baz)s.Load( typeof(Baz), baz1.Code );
			Assert.AreEqual( 2, baz2.StringDateMap.Count, "baz2.StringDateMap count - reachability");
			Assert.AreEqual( 3, baz2.FooArray.Length, "baz2.FooArray length - reachability");
			Assert.AreEqual( 0, baz1.StringDateMap.Count, "baz1.StringDateMap count - reachability");
			Assert.AreEqual( 0, baz1.FooArray.Length, "baz1.FooArray length - reachability");

			Assert.IsNull(baz2.FooArray[1], "null element");
			Assert.IsNotNull(baz2.StringDateMap["today"], "today non-null element");
			Assert.IsNotNull(baz2.StringDateMap["tomm"], "tomm non-null element");
			Assert.IsNull(baz2.StringDateMap["foo"], "foo is null element");

			s.Delete(baz2.FooArray[0]);
			s.Delete(baz2.FooArray[2]);
			s.Delete(baz1);
			s.Delete(baz2);
			s.Flush();
			s.Close();
		}


		[Test]
		public void PersistentLifecycle() 
		{
			ISession s = OpenSession();
			Qux q = new Qux();
			s.Save(q);
			q.Stuff = "foo bar baz qux";
			s.Flush();
			s.Close();

			s = OpenSession();
			q = (Qux)s.Load( typeof(Qux), q.Key );
			Assert.IsTrue( q.Created, "lifecycle create" );
			Assert.IsTrue( q.Loaded, "lifecycle load" );
			Assert.IsNotNull( q.Foo, "lifecycle subobject" );
			s.Delete(q);
			Assert.IsTrue( q.Deleted, "lifecyle delete" );
			s.Flush();
			s.Close();

			s = OpenSession();
			Assert.AreEqual(0, s.Find("from foo in class NHibernate.DomainModel.Foo").Count, "subdeletion");
			s.Flush();
			s.Close();

		}


		[Test]
		public void Enumerable() 
		{
			// this test used to be called Iterators()

			ISession s = OpenSession();
			for( int i=0; i<10; i++ ) 
			{
				Qux q = new Qux();
				object qid = s.Save(q);
				Assert.IsNotNull(q, "q is not null");
				Assert.IsNotNull(qid, "qid is not null");
			}
			s.Flush();
			s.Close();

			s = OpenSession();
			IEnumerator enumer = s.Enumerable("from q in class NHibernate.DomainModel.Qux where q.Stuff is null").GetEnumerator();
			int count = 0;
			while( enumer.MoveNext() ) 
			{
				Qux q = (Qux)enumer.Current;
				q.Stuff = "foo";
				// can't remove item from IEnumerator in .net 
				if (count==0 || count==5) s.Delete(q);
				count++;
			}

			Assert.AreEqual(10, count, "found 10 items");
			s.Flush();
			s.Close();
			
			s = OpenSession();

			Assert.AreEqual(8, 
				s.Delete("from q in class NHibernate.DomainModel.Qux where q.Stuff=?", "foo", NHibernateUtil.String),
				"delete by query");
			
			s.Flush();
			s.Close();

			s = OpenSession();
			enumer = s.Enumerable("from q in class NHibernate.DomainModel.Qux").GetEnumerator();
			Assert.IsFalse( enumer.MoveNext() , "no items in enumerator" );
			s.Flush();
			s.Close();


		}

		/// <summary>
		/// Adding a test to verify that a database action can occur in the
		/// middle of an Enumeration.  Under certain conditions an open 
		/// DataReader can be kept open and cause any other action to fail. 
		/// </summary>
		[Test]
		public void EnumerableDisposable() 
		{
			// this test used to be called Iterators()

			ISession s = OpenSession();
			for( int i=0; i<10; i++ ) 
			{
				Simple simple = new Simple();
				simple.Count = i;
				s.Save(simple, i);
				Assert.IsNotNull(simple, "simple is not null");
			}
			s.Flush();
			s.Close();

			s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Simple simp = (Simple)s.Load( typeof(Simple), 8 );
			
			// the reader under the enum has to still be a SqlDataReader (subst db name here) and 
			// can't be a NDataReader - the best way to get this result is to query on just a property
			// of an object.  If the query is "from Simple as s" then it will be converted to a NDataReader
			// on the MoveNext so it can get the object from the id - thus needing another open DataReader so
			// it must convert to an NDataReader.
			IEnumerable enumer = s.Enumerable("select s.Count from Simple as s");
			//int count = 0;
			foreach( object obj in enumer )
			{
				if( (int)obj==7 )
				{
					break;
				}
			}

			// if Enumerable doesn't implement Dispose() then the test fails on this line
			t.Commit();
			s.Close();

			s = OpenSession();
			Assert.AreEqual( 10, 
				s.Delete( "from Simple" ),
				"delete by query" );
			
			s.Flush();
			s.Close();

			s = OpenSession();
			enumer = s.Enumerable("from Simple");
			Assert.IsFalse( enumer.GetEnumerator().MoveNext() , "no items in enumerator" );
			s.Flush();
			s.Close();

		}


		[Test]
		public void Versioning() 
		{
			ISession s = OpenSession();
			GlarchProxy g = new Glarch();
			s.Save(g);
			GlarchProxy g2 = new Glarch();
			s.Save(g2);
			object gid = s.GetIdentifier(g);
			object g2id = s.GetIdentifier(g2);
			g.Name = "glarch";
			s.Flush();
			//s.Close();
			s.Dispose();

			// grab a version of g that is old and hold onto it until later
			// for a StaleObjectException check.
			ISession sOld = OpenSession();
			GlarchProxy gOld = (GlarchProxy)sOld.Load( typeof(Glarch), gid );
			// want gOld to be initialized so later I can change a property
			NHibernateUtil.Initialize( gOld );
			Assert.IsTrue( NHibernateUtil.IsInitialized( gOld ), "should be initialized" );
			//sOld.Close();
			sOld.Dispose();

			s = OpenSession();
			g = (GlarchProxy)s.Load( typeof(Glarch), gid );
			s.Lock(g, LockMode.Upgrade);
			g2 = (GlarchProxy)s.Load( typeof(Glarch), g2id );
			Assert.AreEqual(1, g.Version, "g's version");
			Assert.AreEqual(1, g.DerivedVersion, "g's derived version");
			Assert.AreEqual(0, g2.Version, "g2's version");
			g.Name = "foo";
			Assert.AreEqual(1, s.Find("from g in class NHibernate.DomainModel.Glarch where g.Version=2").Count, "find by version");
			g.Name = "bar";
			s.Flush();
			//s.Close();
			s.Dispose();

			// now that g has been changed verify that we can't go back and update 
			// it with an old version of g
			bool isStale = false;
			sOld = OpenSession();
			gOld.Name = "should not update";
			try 
			{
				sOld.Update( gOld, gid );
				sOld.Flush();
				//sOld.Close();
				sOld.Dispose();
			}
			catch(Exception e) 
			{
				Exception exc = e;
				while( exc!=null ) 
				{
					if( exc is StaleObjectStateException ) 
					{
						isStale = true;
						break;
					}
					exc = exc.InnerException;
				}
			}

			Assert.IsTrue( isStale, "Did not catch a stale object exception when updating an old GlarchProxy." );

			s = OpenSession();
			g = (GlarchProxy)s.Load( typeof(Glarch), gid );
			g2 = (GlarchProxy)s.Load( typeof(Glarch), g2id );

			Assert.AreEqual(3, g.Version, "g's version");
			Assert.AreEqual(3, g.DerivedVersion, "g's derived version");
			Assert.AreEqual(0, g2.Version, "g2's version");

			g.Next = null;
			g2.Next = g;
			s.Delete(g2);
			s.Delete(g);
			s.Flush();
			//s.Close();
			s.Dispose();
		}


		[Test]
		public void VersionedCollections() 
		{
			ISession s = OpenSession();
			GlarchProxy g = new Glarch();
			s.Save(g);
			g.ProxyArray = new GlarchProxy[] {g};
			string gid = (string) s.GetIdentifier(g);
			ArrayList list = new ArrayList();
			list.Add("foo");
			g.Strings = list;
			// <sets> in h2.0.3
			Iesi.Collections.ISet hashset = new Iesi.Collections.HashedSet();
			hashset.Add( g );
			g.ProxySet = hashset;
			s.Flush();
			s.Close();

			s = OpenSession();
			g = (GlarchProxy)s.Load( typeof(Glarch), gid );
			Assert.AreEqual( 1, g.Strings.Count );
			Assert.AreEqual( 1, g.ProxyArray.Length );
			Assert.AreEqual( 1, g.ProxySet.Count );
			Assert.AreEqual( 1, g.Version, "version collection before");
			s.Flush();
			s.Close();

			s = OpenSession();
			g = (GlarchProxy)s.Load( typeof(Glarch), gid );
			Assert.AreEqual( "foo", g.Strings[0] );
			Assert.AreSame( g, g.ProxyArray[0] );
			IEnumerator enumer = g.ProxySet.GetEnumerator();
			enumer.MoveNext();
			Assert.AreSame( g, enumer.Current );
			Assert.AreEqual(1, g.Version, "versioned collection before");
			s.Flush();
			s.Close();

			s = OpenSession();
			g = (GlarchProxy)s.Load( typeof(Glarch), gid );
			Assert.AreEqual( 1, g.Version, "versioned collection before");
			g.Strings.Add("bar");
			s.Flush();
			s.Close();

			s = OpenSession();
			g = (GlarchProxy)s.Load( typeof(Glarch), gid );
			Assert.AreEqual( 2, g.Version, "versioned collection after" );
			Assert.AreEqual( 2, g.Strings.Count, "versioned collection after" );
			g.ProxyArray = null;
			s.Flush();
			s.Close();

			s = OpenSession();
			g = (GlarchProxy)s.Load( typeof(Glarch), gid );
			Assert.AreEqual( 3, g.Version, "versioned collection after" );
			Assert.AreEqual( 0, g.ProxyArray.Length, "version collection after" );
			g.FooComponents = new ArrayList();
			g.ProxyArray = null;
			s.Flush();
			s.Close();

			s = OpenSession();
			g = (GlarchProxy)s.Load( typeof(Glarch), gid );
			Assert.AreEqual( 4, g.Version, "versioned collection after");
			s.Delete(g);
			s.Flush();
			s.Close();

		}


		[Test]
		public void RecursiveLoad() 
		{
			// Non polymorphisc class (there is an implementation optimization
			// being tested here) - from h2.0.3 - what does that mean?
			ISession s = OpenSession();
			GlarchProxy last = new Glarch();
			s.Save(last);
			last.Order = 0;
			for( int i=0; i<5; i++ ) 
			{
				GlarchProxy next = new Glarch();
				s.Save(next);
				last.Next = next;
				last = next;
				last.Order = (short)(i+1);
			}

			IEnumerator enumer = s.Enumerable("from g in class NHibernate.DomainModel.Glarch").GetEnumerator();
			while( enumer.MoveNext() ) 
			{
				object objTemp = enumer.Current;
			}

			IList list = s.Find("from g in class NHibernate.DomainModel.Glarch");
			Assert.AreEqual( 6, list.Count, "recursive find" );
			s.Flush();
			s.Close();

			s = OpenSession();
			list = s.Find("from g in class NHibernate.DomainModel.Glarch");
			Assert.AreEqual( 6, list.Count, "recursive iter" );
			list = s.Find("from g in class NHibernate.DomainModel.Glarch where g.Next is not null");
			Assert.AreEqual( 5, list.Count, "exclude the null next" );
			s.Flush();
			s.Close();

			s = OpenSession();
			enumer = s.Enumerable("from g in class NHibernate.DomainModel.Glarch order by g.Order asc").GetEnumerator();
			while ( enumer.MoveNext() ) 
			{
				GlarchProxy g = (GlarchProxy)enumer.Current;
				Assert.IsNotNull( g, "not null");
				// no equiv in .net - so ran a delete query
				// iter.remove();
			}

			s.Delete("from NHibernate.DomainModel.Glarch as g");
			s.Flush();
			s.Close();

			// same thing bug using polymorphic class (no optimization possible)
			s = OpenSession();
			FooProxy flast = new Bar();
			s.Save(flast);
			for( int i=0; i<5; i++ ) 
			{
				FooProxy foo = new Bar();
				s.Save(foo);
				flast.TheFoo = foo;
				flast = flast.TheFoo;
				flast.String = "foo" + (i+1);
			}

			enumer = s.Enumerable("from foo in class NHibernate.DomainModel.Foo").GetEnumerator();
			while( enumer.MoveNext() ) 
			{
				object objTemp = enumer.Current;
			}

			list = s.Find("from foo in class NHibernate.DomainModel.Foo");
			Assert.AreEqual( 6, list.Count, "recursive find");
			s.Flush();
			s.Close();

			s = OpenSession();
			list = s.Find("from foo in class NHibernate.DomainModel.Foo");
			Assert.AreEqual( 6, list.Count, "recursive iter" );
			enumer = list.GetEnumerator();
			while( enumer.MoveNext() ) 
			{
				Assert.IsTrue( enumer.Current is BarProxy, "polymorphic recursive load" );
			}
			s.Flush();
			s.Close();

			s = OpenSession();
			enumer = s.Enumerable("from foo in class NHibernate.DomainModel.Foo order by foo.String asc").GetEnumerator();
			string currentString = String.Empty;

			while( enumer.MoveNext() ) 
			{
				
				BarProxy bar = (BarProxy)enumer.Current;
				string theString = bar.String;
				Assert.IsNotNull( bar, "not null");
				if(currentString!=String.Empty) 
				{
					Assert.IsTrue( theString.CompareTo(currentString) >= 0 , "not in asc order" );
				}
				currentString = theString;
				// no equiv in .net - so made a hql delete
				// iter.remove();
			}

			s.Delete("from NHibernate.DomainModel.Foo as foo");
			s.Flush();
			s.Close();
		}


		[Test]
		[Ignore("Not applicable to NHibernate?")]
		public void ScrollableIterator()
		{
		}


		[Test]
		public void MultiColumnQueries() 
		{
			ISession s = OpenSession();
			Foo foo = new Foo();
			s.Save(foo);
			Foo foo1 = new Foo();
			s.Save(foo1);
			foo.TheFoo = foo1;
			IList l = s.Find("select parent, child from parent in class NHibernate.DomainModel.Foo, child in class NHibernate.DomainModel.Foo where parent.TheFoo = child");
			Assert.AreEqual( 1, l.Count, "multi-column find" );

			bool supportsCountDistinct = !( dialect is Dialect.SQLiteDialect );

			IEnumerator rs;
			object[] row;

			if( supportsCountDistinct )
			{
				rs = s.Enumerable("select count(distinct child.id), count(distinct parent.id) from parent in class NHibernate.DomainModel.Foo, child in class NHibernate.DomainModel.Foo where parent.TheFoo = child").GetEnumerator();
				Assert.IsTrue( rs.MoveNext() );
				row = (object[]) rs.Current;
				Assert.AreEqual( 1, row[0], "multi-column count" );
				Assert.AreEqual( 1, row[1], "multi-column count" );
				Assert.IsFalse( rs.MoveNext() );
			}

			rs = s.Enumerable("select child.id, parent.id, child.Long from parent in class NHibernate.DomainModel.Foo, child in class NHibernate.DomainModel.Foo where parent.TheFoo = child").GetEnumerator();
			Assert.IsTrue( rs.MoveNext() );
			row = (object[]) rs.Current;
			Assert.AreEqual( foo.TheFoo.Key, row[0], "multi-column id" );
			Assert.AreEqual( foo.Key, row[1], "multi-column id" );
			Assert.AreEqual( foo.TheFoo.Long, row[2], "multi-column property" );
			Assert.IsFalse( rs.MoveNext() );

			rs = s.Enumerable("select child.id, parent.id, child.Long, child, parent.TheFoo from parent in class NHibernate.DomainModel.Foo, child in class NHibernate.DomainModel.Foo where parent.TheFoo = child").GetEnumerator();
			Assert.IsTrue( rs.MoveNext() );
			row = (object[]) rs.Current;
			Assert.AreEqual( foo.TheFoo.Key, row[0], "multi-column id" );
			Assert.AreEqual( foo.Key, row[1], "multi-column id" );
			Assert.AreEqual( foo.TheFoo.Long, row[2], "multi-column property" );
			Assert.AreSame( foo.TheFoo, row[3], "multi-column object" );
			Assert.AreSame( row[3], row[4], "multi-column same object" );
			Assert.IsFalse( rs.MoveNext() );

			row = (object[])l[0];
			Assert.AreSame( foo, row[0], "multi-column find" );
			Assert.AreSame( foo.TheFoo, row[1], "multi-column find" );
			s.Flush();
			s.Close();

			s = OpenSession();
			IEnumerator enumer = s.Enumerable("select parent, child from parent in class NHibernate.DomainModel.Foo, child in class NHibernate.DomainModel.Foo where parent.TheFoo = child and parent.String='a string'").GetEnumerator();
			int deletions = 0;
			while( enumer.MoveNext() ) 
			{
				object[] pnc = (object[]) enumer.Current;
				s.Delete(pnc[0]);
				s.Delete(pnc[1]);
				deletions++;
			}
			Assert.AreEqual( 1, deletions, "multi-column enumerate");
			s.Flush();
			s.Close();
		}


		[Test]
		public void DeleteTransient() 
		{
			Fee fee = new Fee();
			ISession s = OpenSession();
			ITransaction tx = s.BeginTransaction();
			s.Save(fee);
			s.Flush();
			fee.Count = 123;
			tx.Commit();
			s.Close();

			s = OpenSession();
			tx = s.BeginTransaction();
			s.Delete(fee);
			tx.Commit();
			s.Close();

			s = OpenSession();
			tx = s.BeginTransaction();
			Assert.AreEqual( 0, s.Find("from fee in class Fee").Count );
			tx.Commit();
			s.Close();
		}

		[Test]
		public void DeleteUpdatedTransient()
		{
			Fee fee = new Fee();
			Fee fee2 = new Fee();
			fee2.AnotherFee = fee;
			
			using( ISession s = OpenSession() )
			using( ITransaction tx = s.BeginTransaction() )
			{
				s.Save( fee );
				s.Save( fee2 );
				s.Flush();
				fee.Count = 123;
				tx.Commit();
			}

			using( ISession s = OpenSession() )
			using( ITransaction tx = s.BeginTransaction() )
			{
				s.Update( fee );
				//fee2.AnotherFee = null;
				s.Update( fee2 );
				s.Delete( fee );
				s.Delete( fee2 );
				tx.Commit();
			}

			using( ISession s = OpenSession() )
			using( ITransaction tx = s.BeginTransaction() )
			{
				Assert.AreEqual( 0, s.Find("from fee in class Fee").Count );
				tx.Commit();
			}
		}

		[Test]
		public void UpdateOrder()
		{
			Fee fee1, fee2, fee3;

			using( ISession s = OpenSession() )
			{
				fee1 = new Fee();
				s.Save( fee1 );
				
				fee2 = new Fee();
				fee1.TheFee = fee2;
				fee2.TheFee = fee1;
				fee2.Fees = new Iesi.Collections.HashedSet();

				fee3 = new Fee();
				fee3.TheFee = fee1;
				fee3.AnotherFee = fee2;
				fee2.AnotherFee = fee3;
				s.Save( fee3 );
				s.Save( fee2 );
				s.Flush();
			}

			using( ISession s = OpenSession() )
			{
				fee1.Count = 10;
				fee2.Count = 20;
				fee3.Count = 30;
				s.Update( fee1 );
				s.Update( fee2 );
				s.Update( fee3 );
				s.Flush( );
				s.Delete( fee1 );
				s.Delete( fee2 );
				s.Delete( fee3 );
				s.Flush();
			}

			using( ISession s = OpenSession() )
			using( ITransaction tx = s.BeginTransaction() )
			{
				Assert.AreEqual( 0, s.Find("from fee in class Fee").Count );
				tx.Commit();
			}
		}

		[Test]
		public void UpdateFromTransient() 
		{
			ISession s = OpenSession();
			Fee fee1 = new Fee();
			s.Save(fee1);
			Fee fee2 = new Fee();
			fee1.TheFee = fee2;
			fee2.TheFee = fee1;
			fee2.Fees = new Iesi.Collections.HashedSet();
			Fee fee3 = new Fee();
			fee3.TheFee = fee1;
			fee3.AnotherFee = fee2;
			fee2.AnotherFee = fee3;
			s.Save(fee3);
			s.Save(fee2);
			s.Flush();
			s.Close();

			fee1.Fi = "changed";
			s = OpenSession();
			s.SaveOrUpdate(fee1);
			s.Flush();
			s.Close();

			Qux q = new Qux("quxxy");
			fee1.Qux = q;
			s = OpenSession();
			s.SaveOrUpdate(fee1);
			s.Flush();
			s.Close();

			s = OpenSession();
			fee1 = (Fee)s.Load( typeof(Fee), fee1.Key );
			Assert.AreEqual( "changed", fee1.Fi, "updated from transient" );
			Assert.IsNotNull( fee1.Qux, "unsaved-value" );
			s.Delete( fee1.Qux );
			fee1.Qux = null;
			s.Flush();
			s.Close();

			fee2.Fi = "CHANGED";
			fee2.Fees.Add( "an element" );
			fee1.Fi = "changed again";
			s = OpenSession();
			s.SaveOrUpdate(fee2);
			s.Update( fee1, fee1.Key );
			s.Flush();
			s.Close();

			s = OpenSession();
			Fee fee = new Fee();
			s.Load( fee, fee2.Key );
			fee1 = (Fee)s.Load( typeof(Fee), fee1.Key );
			Assert.AreEqual("changed again", fee1.Fi, "updated from transient" );
			Assert.AreEqual("CHANGED", fee.Fi, "updated from transient" );
			Assert.IsTrue( fee.Fees.Contains("an element"), "updated collection" );
			s.Flush();
			s.Close();

			fee.Fees.Clear();
			fee.Fees.Add( "new element" );
			fee1.TheFee = null;
			s = OpenSession();
			s.SaveOrUpdate(fee);
			s.SaveOrUpdate(fee1);
			s.Flush();
			s.Close();

			s = OpenSession();
			s.Load( fee, fee.Key );
			Assert.IsNotNull( fee.AnotherFee, "update" );
			Assert.IsNotNull( fee.TheFee, "update" );
			Assert.AreSame( fee.AnotherFee.TheFee, fee.TheFee, "update" );
			Assert.IsTrue( fee.Fees.Contains("new element"), "updated collection" );
			Assert.IsFalse( fee.Fees.Contains("an element"), "updated collection" );
			s.Flush();
			s.Close();

			fee.Qux = new Qux("quxy");
			s = OpenSession();
			s.SaveOrUpdate(fee);
			s.Flush();
			s.Close();

			fee.Qux.Stuff = "xxx";
			s = OpenSession();
			s.SaveOrUpdate(fee);
			s.Flush();
			s.Close();

			s = OpenSession();
			s.Load( fee, fee.Key );
			Assert.IsNotNull( fee.Qux, "cascade update" );
			Assert.AreEqual( "xxx", fee.Qux.Stuff, "cascade update" );
			Assert.IsNotNull( fee.AnotherFee, "update" );
			Assert.IsNotNull( fee.TheFee, "update" );
			Assert.AreSame( fee.AnotherFee.TheFee, fee.TheFee, "update" );
			fee.AnotherFee.AnotherFee = null;
			s.Delete(fee);
			s.Delete("from fee in class NHibernate.DomainModel.Fee");
			s.Flush();
			s.Close();
		}


		[Test]
		public void ArraysOfTimes()
		{
			Baz baz;

			using( ISession s = OpenSession() )
			{
				baz = new Baz();
				s.Save(baz);
				baz.SetDefaults();
				s.Flush();
			}

			using( ISession s = OpenSession() )
			{
				baz.TimeArray[2] = new DateTime( 123 );  // H2.1: new Date(123)
				baz.TimeArray[3] = new DateTime( 1234 ); // H2.1: new java.sql.Time(1234)
				s.Update( baz );	// missing in H2.1
				s.Flush();
			}

			using( ISession s = OpenSession() )
			{
				baz = (Baz) s.Load( typeof( Baz ), baz.Code );
				s.Delete( baz );
				s.Flush();
			}
		}

		[Test]
		public void Components() 
		{
			ISession s = OpenSession();
			Foo foo = new Foo();
			foo.Component = new FooComponent( "foo", 69, null, new FooComponent("bar", 96, null, null) );
			s.Save(foo);
			foo.Component.Name = "IFA";
			s.Flush();
			s.Close();

			foo.Component = null;
			s = OpenSession();
			s.Load( foo, foo.Key );

			Assert.AreEqual( "IFA", foo.Component.Name, "save components" );
			Assert.AreEqual( "bar", foo.Component.Subcomponent.Name, "save subcomponent" );
			Assert.IsNotNull( foo.Component.Glarch, "cascades save via component");
			foo.Component.Subcomponent.Name = "baz";
			s.Flush();
			s.Close();

			foo.Component = null;
			s = OpenSession();
			s.Load( foo, foo.Key );
			Assert.AreEqual( "IFA", foo.Component.Name, "update components" );
			Assert.AreEqual( "baz", foo.Component.Subcomponent.Name, "update subcomponent" );
			s.Delete(foo);
			s.Flush();
			s.Close();

			s = OpenSession();
			foo = new Foo();
			s.Save(foo);
			foo.Custom = new string[] {"one", "two"};
			
			// Custom.s1 uses the first column under the <property name="Custom"...>
			// which is first_name
			Assert.AreSame( foo, s.Find("from Foo foo where foo.Custom.s1 = 'one'")[0] );
			s.Delete(foo);
			s.Flush();
			s.Close();
		}


		[Test]
		public void Enum() 
		{
			ISession s = OpenSession();
			FooProxy foo = new Foo();
			object id = s.Save(foo);
			foo.Status = FooStatus.ON;
			s.Flush();
			s.Close();

			// verify an enum can be in the ctor
			s = OpenSession();
			IList list = s.Find("select new Result(foo.String, foo.Long, foo.Integer, foo.Status) from foo in class Foo" );
			Assert.AreEqual( 1, list.Count, "Should have found foo" );
			Assert.AreEqual( FooStatus.ON, ((Result)list[0]).Status, "verifying enum set in ctor - should have been ON" );
			s.Close();

			s = OpenSession();
			foo = (FooProxy)s.Load( typeof(Foo), id );
			Assert.AreEqual( FooStatus.ON, foo.Status );
			foo.Status = FooStatus.OFF;
			s.Flush();
			s.Close();

			s = OpenSession();
			foo = (FooProxy)s.Load( typeof(Foo), id);
			Assert.AreEqual( FooStatus.OFF, foo.Status );
			s.Close();

			// verify that SetEnum with named params works correctly
			s = OpenSession();
			IQuery q = s.CreateQuery( "from Foo as f where f.Status = :status" );
			q.SetEnum( "status", FooStatus.OFF );
			IList results = q.List();
			Assert.AreEqual( 1, results.Count, "should have found 1" );
			foo = (Foo)results[0];
			
			q = s.CreateQuery( "from Foo as f where f.Status = :status" );
			q.SetEnum( "status", FooStatus.ON );
			results = q.List();
			Assert.AreEqual( 0, results.Count, "no foo with status of ON" );

			// try to have the Query guess the enum type
			q = s.CreateQuery( "from Foo as f where f.Status = :status" );
			q.SetParameter( "status", FooStatus.OFF );
			results = q.List();
			Assert.AreEqual( 1, results.Count, "found the 1 result" );

			// have the query guess the enum type in a ParameterList.
			q = s.CreateQuery( "from Foo as f where f.Status in (:status)" );
			q.SetParameterList( "status", new FooStatus[] { FooStatus.OFF, FooStatus.ON } );
			results = q.List();
			Assert.AreEqual( 1, results.Count, "should have found the 1 foo" );
			
			q = s.CreateQuery( "from Foo as f where f.Status = FooStatus.OFF" );
			Assert.AreEqual( 1, q.List().Count, "Enum in string - should have found OFF" );

			q = s.CreateQuery( "from Foo as f where f.Status = FooStatus.ON" );
			Assert.AreEqual( 0, q.List().Count, "Enum in string - should not have found ON" );

			s.Delete(foo);
			s.Flush();
			s.Close();

		}


		[Test]
		public void NoForeignKeyViolations() 
		{
			ISession s = OpenSession();
			Glarch g1 = new Glarch();
			Glarch g2 = new Glarch();
			g1.Next = g2;
			g2.Next = g1;
			s.Save(g1);
			s.Save(g2);
			s.Flush();
			s.Close();

			s = OpenSession();
			IList l = s.Find("from g in class NHibernate.DomainModel.Glarch where g.Next is not null");
			s.Delete( l[0] );
			s.Delete( l[1] );
			s.Flush();
			s.Close();

		}

		[Test]
		public void LazyCollections() 
		{
			ISession s = OpenSession();
			Qux q = new Qux();
			s.Save(q);
			s.Flush();
			s.Close();

			s = OpenSession();
			q = (Qux)s.Load( typeof(Qux), q.Key );
			s.Flush();
			s.Close();

			// two exceptions are supposed to occur:")
			bool ok = false;
			try 
			{
				int countMoreFums = q.MoreFums.Count;
			}
			catch (LazyInitializationException lie) 
			{
				System.Diagnostics.Debug.WriteLine("caught expected " + lie.ToString());
				ok = true;
			}
			Assert.IsTrue( ok, "lazy collection with one-to-many" );

			ok = false;
			try 
			{
				int countFums = q.Fums.Count;
			}
			catch (LazyInitializationException lie) 
			{
				System.Diagnostics.Debug.WriteLine("caught expected " + lie.ToString());
				ok = true;
			}

			Assert.IsTrue( ok, "lazy collection with many-to-many" );

			s = OpenSession();
			q = (Qux)s.Load( typeof(Qux), q.Key );
			s.Delete(q);
			s.Flush();
			s.Close();
		}

		[Test]
		public void NewSessionLifecycle() 
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			object fid = null;
			try 
			{
				Foo f = new Foo();
				s.Save(f);
				fid = s.GetIdentifier(f);
				//s.Flush();
				t.Commit();
			}
			catch (Exception) 
			{
				t.Rollback();
				throw;
			}
			finally 
			{
				s.Close();
			}

			s = OpenSession();
			t = s.BeginTransaction();
			try 
			{
				Foo f = new Foo();
				s.Delete(f);
				//s.Flush();
				t.Commit();
			}
			catch (Exception e) 
			{
				Assert.IsNotNull(e); //getting ride of 'e' is never used compile warning
				t.Rollback();
			}
			finally 
			{
				s.Close();
			}

			s = OpenSession();
			t = s.BeginTransaction();
			try 
			{
				Foo f = (Foo)s.Load( typeof(Foo), fid, LockMode.Upgrade );
				s.Delete(f);
				// s.Flush();
				t.Commit();
			}
			catch(Exception) 
			{
				t.Rollback();
				throw;
			}
			finally 
			{
				Assert.IsNull( s.Close() );
			}

		}

		[Test]
		public void Disconnect() 
		{
			ISession s = OpenSession();
			Foo foo = new Foo();
			Foo foo2 = new Foo();
			s.Save(foo);
			s.Save(foo2);
			foo2.TheFoo = foo;
			s.Flush();
			s.Disconnect();
			
			s.Reconnect();
			s.Delete(foo);
			foo2.TheFoo = null;
			s.Flush();
			s.Disconnect();

			s.Reconnect();
			s.Delete(foo2);
			s.Flush();
			s.Close();


		}

		[Test]
		public void OrderBy() 
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Foo foo = new Foo();
			s.Save(foo);
			IList list = s.Find("select foo from foo in class Foo, fee in class Fee where foo.Dependent = fee order by foo.String desc, foo.Component.Count asc, fee.id");
			Assert.AreEqual( 1, list.Count, "order by");
			Foo foo2 = new Foo();
			s.Save(foo2);
			foo.TheFoo = foo2;
			list = s.Find("select foo.TheFoo, foo.Dependent from foo in class Foo order by foo.TheFoo.String desc, foo.Component.Count asc, foo.Dependent.id");
			Assert.AreEqual( 1, list.Count, "order by" );
			list = s.Find("select foo from foo in class NHibernate.DomainModel.Foo order by foo.Dependent.id, foo.Dependent.Fi");
			Assert.AreEqual( 2, list.Count, "order by");
			s.Delete(foo);
			s.Delete(foo2);
			t.Commit();
			s.Close();

			s = OpenSession();
			Many manyB = new Many();
			s.Save(manyB);
			One oneB = new One();
			s.Save(oneB);
			oneB.Value = "b";
			manyB.One = oneB;
			Many manyA = new Many();
			s.Save(manyA);
			One oneA = new One();
			s.Save(oneA);
			oneA.Value = "a";
			manyA.One = oneA;
			s.Flush();
			s.Close();

			s = OpenSession();
			IEnumerable enumerable = s.Enumerable("SELECT one FROM one IN CLASS " + typeof(One).Name + " ORDER BY one.Value ASC");
			int count = 0;
			foreach(One one in enumerable) 
			{
				switch(count) 
				{
					case 0:
						Assert.AreEqual( "a", one.Value, "a - ordering failed" );
						break;
					case 1:
						Assert.AreEqual( "b", one.Value, "b - ordering failed" );
						break;
					default:
						Assert.Fail("more than two elements");
						break;
				}
				count++;
			}
	
			s.Flush();
			s.Close();

			s = OpenSession();
			enumerable = s.Enumerable("SELECT many.One FROM many IN CLASS " + typeof(Many).Name + " ORDER BY many.One.Value ASC, many.One.id");
			count = 0;
			foreach(One one in enumerable) 
			{
				switch(count) 
				{
					case 0:
						Assert.AreEqual( "a", one.Value, "'a' should be first element" );
						break;
					case 1:
						Assert.AreEqual( "b", one.Value, "'b' should be second element" );
						break;
					default:
						Assert.Fail("more than 2 elements");
						break;
				}
				count++;
			}
			s.Flush();
			s.Close();

			s = OpenSession();
			oneA = (One)s.Load( typeof(One), oneA.Key );
			manyA = (Many)s.Load( typeof(Many), manyA.Key );
			oneB = (One)s.Load( typeof(One), oneB.Key );
			manyB = (Many)s.Load( typeof(Many), manyB.Key );
			s.Delete(manyA);
			s.Delete(oneA);
			s.Delete(manyB);
			s.Delete(oneB);
			s.Flush();
			s.Close();

		}

		[Test]
		public void ManyToOne() 
		{
			ISession s = OpenSession();
			One one = new One();
			s.Save(one);
			one.Value = "yada";
			Many many = new Many();
			many.One = one;
			s.Save(many);
			s.Flush();
			s.Close();

			s = OpenSession();
			one = (One)s.Load( typeof(One), one.Key );
			int countManies = one.Manies.Count;
			s.Close();

			s = OpenSession();
			many = (Many)s.Load( typeof(Many), many.Key );
			Assert.IsNotNull( many.One, "many-to-one assoc" );
			s.Delete( many.One );
			s.Delete(many);
			s.Flush();
			s.Close();

		}

		[Test]
		public void SaveDelete()
		{
			ISession s = OpenSession();
			Foo f = new Foo();
			s.Save(f);
			s.Flush();
			s.Close();
		
			s = OpenSession();
			s.Delete( s.Load( typeof(Foo), f.Key ) );
			s.Flush();
			s.Close();
		}

		[Test]
		public void ProxyArray() 
		{
			ISession s = OpenSession();
			GlarchProxy g = new Glarch();
			Glarch g1 = new Glarch();
			Glarch g2 = new Glarch();
			g.ProxyArray = new GlarchProxy[] { g1, g2 };
			Glarch g3 = new Glarch();
			s.Save( g3 );
			g2.ProxyArray = new GlarchProxy[] { null, g3, g };

			Iesi.Collections.ISet hashset = new Iesi.Collections.HashedSet();
			hashset.Add( g1 );
			hashset.Add( g2 );
			g.ProxySet = hashset;
			s.Save( g );
			s.Save( g1 );
			s.Save( g2 );
			object id = s.GetIdentifier( g );
			s.Flush();
			s.Close();

			s = OpenSession();
			g = (GlarchProxy)s.Load( typeof(Glarch), id );
			Assert.AreEqual( 2, g.ProxyArray.Length, "array of proxies" );
			Assert.IsNotNull( g.ProxyArray[0], "array of proxies" );
			Assert.IsNull( g.ProxyArray[1].ProxyArray[0], "deferred load test" );
			Assert.AreEqual( g, g.ProxyArray[1].ProxyArray[2], "deferred load test" );
			Assert.AreEqual( 2, g.ProxySet.Count, "set of proxies" );

			IEnumerator enumer = s.Enumerable( "from g in class NHibernate.DomainModel.Glarch" ).GetEnumerator();
			while( enumer.MoveNext() ) 
			{
				s.Delete( enumer.Current );
			}

			s.Flush();
			s.Disconnect();

			// serialize the session.
			System.IO.Stream stream = new System.IO.MemoryStream();
			System.Runtime.Serialization.IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
			formatter.Serialize(stream, s);

			// close the original session
			s.Close();

			// deserialize the session
			stream.Position = 0;
			s = (ISession)formatter.Deserialize(stream);
			stream.Close();

			s.Close();
		}

		[Test]
		public void Cache() 
		{
			Immutable im = new Immutable();

			using( ISession s = OpenSession() )
			{
				s.Save(im);
				s.Flush();
			}

			using( ISession s = OpenSession() )
			{
				s.Load( im, im.Id);
			}

			using( ISession s = OpenSession() )
			{
				s.Load( im, im.Id);
	
				Immutable imFromFind = (Immutable)s.Find("from im in class Immutable where im = ?", im, NHibernateUtil.Entity(typeof(Immutable)))[0];
				Immutable imFromLoad = (Immutable)s.Load(typeof(Immutable), im.Id);
			
				Assert.IsTrue(im==imFromFind, "cached object identity from Find ");
				Assert.IsTrue(im==imFromLoad, "cached object identity from Load ");
			}

			// Clean up the immutable. Need to do this using direct SQL, since ISession
			// refuses to delete immutable objects.
			using( ISession s = OpenSession() )
			{
				IDbConnection connection = s.Connection;
				using( IDbCommand command = connection.CreateCommand() )
				{
					command.CommandText = "delete from immut";
					command.ExecuteNonQuery();
				}
			}
		}

		[Test]
		public void FindLoad() 
		{
			ISession s = OpenSession();
			FooProxy foo = new Foo();
			s.Save(foo);
			s.Flush();
			s.Close();

			s = OpenSession();
			foo = (FooProxy)s.Find("from foo in class NHibernate.DomainModel.Foo")[0];
			FooProxy foo2 = (FooProxy)s.Load( typeof(Foo), foo.Key );
			Assert.AreSame( foo, foo2, "find returns same object as load" );
			s.Flush();
			s.Close();

			s = OpenSession();
			foo2 = (FooProxy)s.Load( typeof(Foo), foo.Key );
			foo = (FooProxy)s.Find("from foo in class NHibernate.DomainModel.Foo")[0];
			Assert.AreSame( foo2, foo, "find returns same object as load" );
			s.Delete("from foo in class NHibernate.DomainModel.Foo");
			s.Flush();
			s.Close();
		}

		[Test]
		public void Refresh() 
		{
			ISession s = OpenSession();
			Foo foo = new Foo();
			s.Save(foo);
			s.Flush();

			IDbCommand cmd = s.Connection.CreateCommand();
			cmd.CommandText = "update " + dialect.QuoteForTableName("foos") + " set long_ = -3";
			cmd.ExecuteNonQuery();
			s.Refresh(foo);
			Assert.AreEqual( (long)-3, foo.Long ) ;
			s.Delete(foo);
			s.Flush();
			s.Close();

		}

		[Test]
		public void AutoFlush() 
		{
			ISession s = OpenSession();
			FooProxy foo = new Foo();
			s.Save(foo);
			Assert.AreEqual( 1, s.Find("from foo in class NHibernate.DomainModel.Foo").Count, "autoflush inserted row" );
			foo.Char = 'X';
			Assert.AreEqual( 1, s.Find("from foo in class NHibernate.DomainModel.Foo where foo.Char='X'").Count, "autflush updated row" );
			s.Close();

			s = OpenSession();
			foo = (FooProxy)s.Load( typeof(Foo), foo.Key );
			
			if( dialect.SupportsSubSelects )
			{
				foo.Bytes = GetBytes( "osama" );
				Assert.AreEqual( 1, s.Find("from foo in class NHibernate.DomainModel.Foo where 111 in foo.Bytes.elements").Count, "autoflush collection update" );
				foo.Bytes[0] = 69;
				Assert.AreEqual( 1, s.Find("from foo in class NHibernate.DomainModel.Foo where 69 in foo.Bytes.elements").Count, "autoflush collection update" );
			}

			s.Delete(foo);
			Assert.AreEqual( 0, s.Find("from foo in class NHibernate.DomainModel.Foo").Count, "autoflush delete" );
			s.Close();

		}

		[Test]
		public void Veto() 
		{
			ISession s = OpenSession();
			Vetoer v = new Vetoer();
			s.Save(v);
			object id = s.Save(v);
			s.Flush();
			s.Close();

			s = OpenSession();
			s.Update(v, id);
			s.Update(v, id);
			s.Delete(v);
			s.Delete(v);
			s.Flush();
			s.Close();
		}

		[Test]
		public void SerializableType() 
		{
			ISession s = OpenSession();
			Vetoer v = new Vetoer();
			v.Strings = new string[] {"foo", "bar", "baz"};
			s.Save(v);
			object id = s.Save(v);
			v.Strings[1] = "osama";
			s.Flush();
			s.Close();

			s = OpenSession();
			v = (Vetoer)s.Load( typeof(Vetoer), id );
			Assert.AreEqual( "osama", v.Strings[1], "serializable type" );
			s.Delete(v);
			s.Delete(v);
			s.Flush();
			s.Close();
		}

		[Test]
		public void AutoFlushCollections() 
		{
			ISession s = OpenSession();
			ITransaction tx = s.BeginTransaction();
			Baz baz = new Baz();
			baz.SetDefaults();
			s.Save(baz);
			tx.Commit();
			s.Close();

			s = OpenSession();
			tx = s.BeginTransaction();
			baz = (Baz)s.Load( typeof(Baz), baz.Code );
			baz.StringArray[0] = "bark";
			IEnumerator e = s.Enumerable("select baz.StringArray.elements from baz in class NHibernate.DomainModel.Baz").GetEnumerator();
			bool found = false;
			while( e.MoveNext() ) 
			{
				if ( "bark".Equals( e.Current ) )
				{
					found = true;
				}
			}
			Assert.IsTrue(found);
			baz.StringArray = null;
			e = s.Enumerable("select distinct baz.StringArray.elements from baz in class NHibernate.DomainModel.Baz").GetEnumerator();
			Assert.IsFalse( e.MoveNext() );
			baz.StringArray = new string[] {"foo", "bar"};
			e = s.Enumerable("select baz.StringArray.elements from baz in class NHibernate.DomainModel.Baz").GetEnumerator();
			Assert.IsTrue( e.MoveNext() );

			Foo foo = new Foo();
			s.Save(foo);
			s.Flush();
			baz.FooArray = new Foo[]{foo};

			e = s.Enumerable("select foo from baz in class NHibernate.DomainModel.Baz, foo in baz.FooArray.elements").GetEnumerator();
			found = false;
			while( e.MoveNext() ) 
			{
				if ( foo==e.Current ) 
				{
					found = true;
				}
			}
			Assert.IsTrue(found);

			baz.FooArray[0] = null;
			e = s.Enumerable("select foo from baz in class NHibernate.DomainModel.Baz, foo in baz.FooArray.elements").GetEnumerator();
			Assert.IsFalse( e.MoveNext() );
			baz.FooArray[0] = foo;
			e = s.Enumerable("select baz.FooArray.elements from baz in class NHibernate.DomainModel.Baz").GetEnumerator();
			Assert.IsTrue( e.MoveNext() );

			if( dialect.SupportsSubSelects ) 
			{
				baz.FooArray[0] = null;
				e = s.Enumerable("from baz in class NHibernate.DomainModel.Baz where ? in baz.FooArray.elements", 
					foo, 
					NHibernateUtil.Entity( typeof(Foo) ) ).GetEnumerator();
				
				Assert.IsFalse( e.MoveNext() );
				baz.FooArray[0] = foo;
				e = s.Enumerable("select foo from foo in class NHibernate.DomainModel.Foo where foo in "
					+ "(select elt from baz in class NHibernate.DomainModel.Baz, elt in baz.FooArray.elements)"
					).GetEnumerator();
				Assert.IsTrue( e.MoveNext() );
			}
			s.Delete(foo);
			s.Delete(baz);
			tx.Commit();
			s.Close();
		}

		[Test]
		public void UserProvidedConnection() 
		{
			Connection.IConnectionProvider prov = Connection.ConnectionProviderFactory.NewConnectionProvider(cfg.Properties);
			ISession s = sessions.OpenSession( prov.GetConnection() );
			ITransaction tx = s.BeginTransaction();
			s.Find("from foo in class NHibernate.DomainModel.Fo");
			tx.Commit();

			IDbConnection c = s.Disconnect();
			Assert.IsNotNull(c);

			s.Reconnect(c);
			tx = s.BeginTransaction();
			s.Find("from foo in class NHibernate.DomainModel.Fo");
			tx.Commit();
			Assert.AreSame( c, s.Close() );
			c.Close();
		}

		[Test]
		public void CachedCollection() 
		{
			ISession s = OpenSession();
			Baz baz = new Baz();
			baz.SetDefaults();
			s.Save(baz);
			s.Flush();
			s.Close();

			s = OpenSession();
			baz = (Baz)s.Load( typeof(Baz), baz.Code );
			( (FooComponent)baz.TopComponents[0]).Count = 99;
			s.Flush();
			s.Close();

			s = OpenSession();
			baz = (Baz)s.Load( typeof(Baz), baz.Code );
			Assert.AreEqual( 99,  ( (FooComponent)baz.TopComponents[0]).Count );
			s.Delete(baz);
			s.Flush();
			s.Close();

		}

		[Test]
		public void ComplicatedQuery() 
		{
			ISession s = OpenSession();
			Foo foo = new Foo();
			object id = s.Save(foo);
			Assert.IsNotNull(id);
			Qux q = new Qux("q");
			foo.Dependent.Qux = q;
			s.Save(q);
			q.Foo.String = "foo2";

			IEnumerator enumer = s.Enumerable("from foo in class Foo where foo.Dependent.Qux.Foo.String = 'foo2'").GetEnumerator();
			Assert.IsTrue( enumer.MoveNext() );
			s.Delete(foo);
			s.Flush();
			s.Close();
		}

		[Test]
		public void LoadAfterDelete() 
		{
			ISession s = OpenSession();
			Foo foo = new Foo();
			object id = s.Save(foo);
			s.Flush();
			s.Delete(foo);

			bool err = false;
			try 
			{
				s.Load( typeof(Foo), id );
			}
			catch(ObjectDeletedException ode) 
			{
				Assert.IsNotNull(ode); //getting ride of 'ode' is never used compile warning
				err = true;
			}
			Assert.IsTrue(err);
			s.Flush();
			err = false;

			try 
			{
				bool proxyBoolean = ( (FooProxy)s.Load( typeof(Foo), id )).Boolean;
			}
			// this won't work until Proxies are implemented because now it throws an 
			// ObjectNotFoundException
			catch(LazyInitializationException lie) 
			{
				Assert.IsNotNull( lie ); //getting ride of 'lie' is never used compile warning
				err = true;
			}
			Assert.IsTrue(err);

			Fo fo = Fo.NewFo();
			id = FumTest.FumKey("abc"); //yuck!
			s.Save(fo, id);
			s.Flush();
			s.Delete(fo);
			err = false;

			try 
			{
				s.Load( typeof(Fo), id );
			}
			catch(ObjectDeletedException ode) 
			{
				Assert.IsNotNull(ode); //getting ride of 'ode' is never used compile warning
				err = true;
			}

			Assert.IsTrue(err);
			s.Flush();
			s.Close();

		}


		[Test]
		public void ObjectType()
		{
			object gid;

			using( ISession s = OpenSession() )
			{
				GlarchProxy g = new Glarch();
				Foo foo = new Foo();
				g.Any = foo;
				gid = s.Save( g );
				s.Save( foo );
				s.Flush();
			}

			using( ISession s = OpenSession() )
			{
				GlarchProxy g = (GlarchProxy) s.Load( typeof( Glarch ), gid );
				Assert.IsNotNull( g.Any );
				Assert.IsTrue( g.Any is FooProxy );
				s.Delete( g.Any );
				s.Delete( g );
				s.Flush();
			}
		}


		[Test]
		public void Any() 
		{
			ISession s = OpenSession();
			One one = new One();
			BarProxy foo = new Bar();
			foo.Object = one;
			object fid = s.Save(foo);
			object oid = one.Key;
			s.Flush();
			s.Close();

			s = OpenSession();
			IList list = s.Find(
				"from Bar bar where bar.Object.id = ? and bar.Object.class = ?",
				new object[] { oid, typeof(One) },
				new Type.IType[] { NHibernateUtil.Int64, NHibernateUtil.Class } );
			Assert.AreEqual(1, list.Count);

			// this is a little different from h2.0.3 because the full type is stored, not
			// just the class name.
			list = s.Find("select one from One one, Bar bar where bar.Object.id = one.id and bar.Object.class LIKE 'NHibernate.DomainModel.One%'");
			Assert.AreEqual(1, list.Count);
			s.Flush();
			s.Close();

			s = OpenSession();
			foo = (BarProxy)s.Load( typeof(Foo), fid );
			Assert.IsNotNull(foo);
			Assert.IsTrue( foo.Object is One );
			Assert.AreEqual( oid, s.GetIdentifier( foo.Object ) );
			s.Delete( foo );
			s.Delete( foo.Object );
			s.Flush();
			s.Close();

		}

		[Test]
		public void EmbeddedCompositeID() 
		{
			ISession s = OpenSession();
			Location l = new Location();
			l.CountryCode = "AU";
			l.Description = "foo bar";
			l.Locale = System.Globalization.CultureInfo.CreateSpecificCulture("en-AU");
			l.StreetName = "Brunswick Rd";
			l.StreetNumber = 300;
			l.City = "Melbourne";
			s.Save(l);
			s.Flush();
			s.Close();

			s = OpenSession();
			s.FlushMode = FlushMode.Never;
			l = (Location)s.Find("from l in class Location where l.CountryCode = 'AU' and l.Description='foo bar'")[0];
			Assert.AreEqual( "AU", l.CountryCode );
			Assert.AreEqual( "Melbourne", l.City );
			Assert.AreEqual( System.Globalization.CultureInfo.CreateSpecificCulture("en-AU"), l.Locale );
			s.Close();

			s = OpenSession();
			l.Description = "sick're";
			s.Update(l);
			s.Flush();
			s.Close();

			s = OpenSession();
			l = new Location();
			l.CountryCode = "AU";
			l.Description = "foo bar";
			l.Locale = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");
			l.StreetName = "Brunswick Rd";
			l.StreetNumber = 300;
			l.City = "Melbourne";
			Assert.AreSame( l, s.Load( typeof(Location), l ) );
			Assert.AreEqual( System.Globalization.CultureInfo.CreateSpecificCulture("en-AU"), l.Locale );
			s.Delete(l);
			s.Flush();
			s.Close();

		}

		[Test]
		public void AutosaveChildren() 
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Baz baz = new Baz();
			Iesi.Collections.ISet bars = new Iesi.Collections.HashedSet();
			baz.CascadingBars = bars;
			s.Save(baz);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			baz = (Baz) s.Load( typeof(Baz), baz.Code );
			baz.CascadingBars.Add( new Bar() );
			baz.CascadingBars.Add( new Bar() );
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			baz = (Baz)s.Load( typeof(Baz), baz.Code );
			Assert.AreEqual( 2, baz.CascadingBars.Count );
			IEnumerator enumer = baz.CascadingBars.GetEnumerator();
			Assert.IsTrue( enumer.MoveNext() );
			Assert.IsNotNull( enumer.Current );
			baz.CascadingBars.Clear(); // test all-delete-orphan
			s.Flush();

			Assert.AreEqual( 0, s.Find("from Bar bar").Count );
			s.Delete(baz);
			t.Commit();
			s.Close();

		}

		[Test]
		public void OrphanDelete() 
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Baz baz = new Baz();
			IDictionary bars = new Hashtable();
			bars.Add( new Bar(), new object() );
			bars.Add( new Bar(), new object() );
			bars.Add( new Bar(), new object() );
			s.Save(baz);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			baz = (Baz) s.Load( typeof(Baz), baz.Code );
			IEnumerator enumer = bars.GetEnumerator();
			enumer.MoveNext();
			bars.Remove( enumer.Current );
			s.Delete(baz);
			enumer.MoveNext();
			bars.Remove( enumer.Current );
			s.Flush();

			Assert.AreEqual( 0, s.Find("from Bar bar").Count );
			t.Commit();
			s.Close();
		}

		[Test]
		public void TransientOrphanDelete() 
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Baz baz = new Baz();
			Iesi.Collections.ISet bars = new Iesi.Collections.HashedSet();
			baz.CascadingBars = bars;
			bars.Add( new Bar() );
			bars.Add( new Bar() );
			bars.Add( new Bar() );
			IList foos = new ArrayList();
			foos.Add( new Foo() );
			foos.Add( new Foo() );
			baz.FooBag = foos;
			s.Save(baz);

			IEnumerator enumer = new Util.JoinedEnumerable( new IEnumerable[] { foos , bars } ).GetEnumerator();
			while( enumer.MoveNext() ) 
			{
				FooComponent cmp = ((Foo)enumer.Current).Component;
				s.Delete( cmp.Glarch );
				cmp.Glarch = null;
			}

			t.Commit();
			s.Close();

			enumer = bars.GetEnumerator();
			enumer.MoveNext();
			bars.Remove( enumer.Current );
			foos.RemoveAt(1);
			s = OpenSession();
			t = s.BeginTransaction();
			s.Update(baz);
			Assert.AreEqual( 2, s.Find("from Bar bar").Count );
			Assert.AreEqual( 3, s.Find("from Foo foo").Count );
			t.Commit();
			s.Close();

			foos.RemoveAt(0);
			s = OpenSession();
			t = s.BeginTransaction();
			s.Update(baz);
			enumer = bars.GetEnumerator();
			enumer.MoveNext();
			bars.Remove( enumer.Current );
			s.Delete(baz);
			s.Flush();
			Assert.AreEqual( 0, s.Find("from Foo foo").Count );
			t.Commit();
			s.Close();
		}

		[Test]
		public void ProxiesInCollections() 
		{
			ISession s = OpenSession();
			Baz baz = new Baz();
			Bar bar = new Bar();
			Bar bar2 = new Bar();
			s.Save( bar );
			object bar2id = s.Save( bar2 );
			baz.FooArray = new Foo[] { bar, bar2 };

			Iesi.Collections.ISet hashset = new Iesi.Collections.HashedSet();
			bar = new Bar();
			s.Save( bar );
			hashset.Add( bar );
			baz.FooSet = hashset;
			hashset = new Iesi.Collections.HashedSet();
			hashset.Add( new Bar() );
			hashset.Add( new Bar() );
			baz.CascadingBars = hashset;
			ArrayList list = new ArrayList();
			list.Add( new Foo() );
			baz.FooBag = list;
			object id = s.Save( baz );
			IEnumerator enumer = baz.CascadingBars.GetEnumerator();
			enumer.MoveNext();
			object bid = ((Bar)enumer.Current).Key;
			s.Flush();
			s.Close();

			s = OpenSession();
			BarProxy barprox = (BarProxy)s.Load( typeof(Bar), bid );
			BarProxy bar2prox = (BarProxy)s.Load( typeof(Bar), bar2id );
			Assert.IsTrue( bar2prox is Proxy.INHibernateProxy );
			Assert.IsTrue( barprox is Proxy.INHibernateProxy );
			baz = (Baz)s.Load( typeof(Baz), id );
			enumer = baz.CascadingBars.GetEnumerator();
			enumer.MoveNext();
			BarProxy b1 = (BarProxy)enumer.Current;
			enumer.MoveNext();
			BarProxy b2 = (BarProxy)enumer.Current;
			Assert.IsTrue( 
				( b1==barprox && !(b2 is Proxy.INHibernateProxy) )
				|| ( b2==barprox && !(b1 is Proxy.INHibernateProxy) ) ); //one-to-many
			Assert.IsTrue( baz.FooArray[0] is Proxy.INHibernateProxy ); //many-to-many
			Assert.AreEqual( bar2prox, baz.FooArray[1] );
			if( ((Engine.ISessionFactoryImplementor)sessions).IsOuterJoinedFetchEnabled ) 
			{
				enumer = baz.FooBag.GetEnumerator();
				enumer.MoveNext();
				Assert.IsFalse( enumer.Current is Proxy.INHibernateProxy ); // many-to-many outer-join="true"
			}

			enumer = baz.FooSet.GetEnumerator();
			enumer.MoveNext();
			Assert.IsFalse( enumer.Current is Proxy.INHibernateProxy ); //one-to-many
			s.Delete( "from o in class Baz" );
			s.Delete( "from o in class Foo" );
			s.Flush();
			s.Close();
		}

		[Test]
		[Ignore("Not applicable to NHibernate?")]
		public void Service() 
		{
		}

		[Test]
		public void PSCache() 
		{
			using( ISession s = OpenSession() )
			{
				for( int i = 0; i < 10; i++ ) s.Save( new Foo() );

				IQuery q = s.CreateQuery( "from f in class Foo" );
				q.SetMaxResults( 2 );
				q.SetFirstResult( 5 );
				
				Assert.AreEqual( 2, q.List().Count );

				q = s.CreateQuery( "from f in class Foo" );

				Assert.AreEqual( 10, q.List().Count );
				Assert.AreEqual( 10, q.List().Count );

				q.SetMaxResults( 3 );
				q.SetFirstResult( 3 );

				Assert.AreEqual( 3, q.List().Count );

				q = s.CreateQuery( "from f in class Foo" );
				Assert.AreEqual( 10, q.List().Count );
			}

			using( ISession s = OpenSession() )
			{
				IQuery q = s.CreateQuery( "from f in class Foo" );
				Assert.AreEqual( 10, q.List().Count );

				q.SetMaxResults(5);
				Assert.AreEqual( 5, q.List().Count );

				s.Delete( "from f in class Foo" );
				s.Flush();
			}
		}


		#region NHibernate specific tests
		[Test]
		public void Formula()
		{
			Foo foo = new Foo();
			ISession s = OpenSession();
			object id = s.Save( foo );
			s.Flush();
			s.Close();

			s = OpenSession();
			foo = (Foo)s.Find( "from Foo as f where f.id = ?", id, NHibernateUtil.String)[0];
			Assert.AreEqual( 4, foo.Formula, "should be 2x 'Int' property that is defaulted to 2" );

			s.Delete( foo );
			s.Flush();
			s.Close();
		}

		/// <summary>
		/// This test verifies that the AddAll() method works
		/// correctly for a persistent Set.
		/// </summary>
		[Test]
		public void AddAll()						
		{
 			using (ISession s = OpenSession())
			{
 				Foo foo1 = new Foo();
 				s.Save(foo1);
 				Foo foo2 = new Foo();
 				s.Save(foo2);
 				Foo foo3 = new Foo();
 				s.Save(foo3);
 				Baz baz = new Baz();
 				baz.FooSet = new Iesi.Collections.HashedSet();
 				baz.FooSet.Add(foo1);
 				s.Save(baz);
 				Assert.AreEqual(1, baz.FooSet.Count);
	
 				IList foos = new ArrayList();
 				foos.Add(foo2);
 				foos.Add(foo3);
 				baz.FooSet.AddAll(foos);
 				Assert.AreEqual(3, baz.FooSet.Count);
	
 				s.Flush();

				// Clean up
				foreach( Foo foo in baz.FooSet )
				{
					s.Delete( foo );
				}

				s.Delete( baz );
				s.Flush();
 			}
		}
		#endregion
	}
}
