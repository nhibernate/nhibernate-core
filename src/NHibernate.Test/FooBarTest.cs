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
		[SetUp]
		public void SetUp()
		{
			ExportSchema(new string[] {   
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
				//"Vetoer.hbm.xml",
				"Holder.hbm.xml",
				"Location.hbm.xml",
				"Stuff.hbm.xml",
				"Container.hbm.xml",
				"Simple.hbm.xml",
				"XY.hbm.xml"
				}, true);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <remarks>
		/// This test is still not completely working because it depends on Proxies which
		/// has not been implemented yet.
		/// </remarks>
		[Test]
		public void FetchInitializedCollection()
		{
			ISession s = sessions.OpenSession();
			Baz baz = new Baz();
			IList fooBag = new ArrayList();
			fooBag.Add( new Foo() );
			fooBag.Add( new Foo() );
			baz.FooBag=fooBag;
			s.Save(baz);
			fooBag = baz.FooBag;
			s.Find("from Baz baz left join fetch baz.FooBag");
			Assert.IsTrue( NHibernate.IsInitialized(fooBag) );
			Assert.IsTrue( fooBag==baz.FooBag );
			Assert.IsTrue( baz.FooBag.Count==2 );
			s.Close();
			
			s = sessions.OpenSession();
			baz = (Baz) s.Load( typeof(Baz), baz.Code );
			Object bag = baz.FooBag;
			Assert.IsFalse( NHibernate.IsInitialized(bag) );
			s.Find("from Baz baz left join fetch fooBag");
			Assert.IsFalse( NHibernate.IsInitialized(bag) );
			Assert.IsTrue( bag==baz.FooBag );
			Assert.IsTrue( baz.FooBag.Count==2 );
			s.Delete(baz);
			s.Flush();

			s.Close();
		}

		
		[Test]
		public void Sortables()
		{
			ISession s = sessions.OpenSession();
			ITransaction t = s.BeginTransaction();
			Baz b = new Baz();
			IDictionary ss = new Hashtable();
			ss.Add(new Sortable("foo"), null);
			ss.Add(new Sortable("bar"), null);
			ss.Add(new Sortable("baz"), null);
			b.Sortablez = ss;
			s.Save(b);
			s.Flush();
			t.Commit();
			s.Close();
		
			s = sessions.OpenSession();
			t = s.BeginTransaction();
			IList result = s.CreateCriteria(typeof(Baz))
				.AddOrder( Expression.Order.Asc("Name") )
				.List();
			b = (Baz) result[0];
			Assert.IsTrue( b.Sortablez.Count==3 );
			
			// compare the first item in the "Set" sortablez - can't reference
			// the first item using b.sortablez[0] because it thinks 0 is the
			// DictionaryEntry key - not the index.
			foreach(DictionaryEntry de in b.Sortablez) 
			{
				Assert.AreEqual( ((Sortable)de.Key).name, "bar");
				break;
			}
		
			s.Flush();
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			result = s.CreateQuery("from Baz baz left join fetch baz.Sortablez order by baz.Name asc")
				.List();
			b = (Baz) result[0];
			Assert.IsTrue( b.Sortablez.Count==3 );
			foreach(DictionaryEntry de in b.Sortablez) 
			{
				Assert.AreEqual( ((Sortable)de.Key).name, "bar");
				break;
			}
			s.Flush();
			t.Commit();
			s.Close();
		
			s = sessions.OpenSession();
			t = s.BeginTransaction();
			result = s.CreateQuery("from Baz baz order by baz.Name asc")
				.List();
			b = (Baz) result[0];
			Assert.IsTrue( b.Sortablez.Count==3 );
			foreach(DictionaryEntry de in b.Sortablez) 
			{
				Assert.AreEqual( ((Sortable)de.Key).name, "bar");
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
			ISession s = sessions.OpenSession();
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
			Assert.IsTrue( NHibernate.IsInitialized( ( (Baz) ( (object[]) list[0] )[1] ).Fees ) );
			s.Delete(foo);
			s.Delete(foo2);
			s.Delete(baz);
			s.Flush();
			s.Close();
		}

		
		[Test]
		public void BagOneToMany() 
		{
			ISession s = sessions.OpenSession();
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
		[Ignore("Test is failing and need to debug.")]
		public void QueryLockMode() 
		{
			ISession s = sessions.OpenSession();
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
			//TODO: test is failing here because CurrentLock Mode is LockMode.Write
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

			s = sessions.OpenSession();
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
			ISession s = sessions.OpenSession();
			Baz baz = new Baz();
			object id = s.Save(baz);
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			baz = (Baz)s.Load( typeof(Baz), id );
			baz.FooBag.Add( new Foo() );
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			baz = (Baz)s.Load( typeof(Baz), id );
			Assert.IsFalse( NHibernate.IsInitialized( baz.FooBag ) );
			Assert.AreEqual( 1, baz.FooBag.Count );

			Assert.IsTrue( NHibernate.IsInitialized( baz.FooBag[0] ) );
			s.Delete(baz);
			s.Flush();
			s.Close();

		}

		[Test]
		public void IdBag() 
		{
			ISession s = sessions.OpenSession();
			Baz baz = new Baz();
			s.Save(baz);

			IList l = new ArrayList();
			IList l2 = new ArrayList();

			baz.IdFooBag = l;
			baz.ByteBag = l2;

			l.Add( new Foo() );
			l.Add( new Bar() );

			System.Text.Encoding encoding = new System.Text.UnicodeEncoding();

			byte[] bytes = encoding.GetBytes("ffo");
			l2.Add(bytes);
			l2.Add( encoding.GetBytes("foo") );
			s.Flush();

			l.Add( new Foo() );
			l.Add( new Bar() );
			l2.Add( encoding.GetBytes("bar") );
			s.Flush();
			
			object removedObject = l[3];
			l.RemoveAt(3);
			s.Delete(removedObject);
			
			bytes[1] = Convert.ToByte('o'); 
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			baz = (Baz) s.Load( typeof(Baz), baz.Code );
			Assert.AreEqual( 3, baz.IdFooBag.Count );
			Assert.AreEqual( 3, baz.ByteBag.Count );
			bytes = encoding.GetBytes("foobar");

			foreach(object obj in baz.IdFooBag) 
			{
				s.Delete(obj);
			}
			baz.IdFooBag = null;
			baz.ByteBag.Add(bytes);
			baz.ByteBag.Add(bytes);
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			baz = (Baz)s.Load( typeof(Baz), baz.Code );
			Assert.AreEqual( 0, baz.IdFooBag.Count );
			Assert.AreEqual( 5, baz.ByteBag.Count );
			s.Delete(baz);
			s.Flush();
			s.Close();

		}

		[Test]
		[Ignore("Test not written yet.")]
		public void ForceOuterJoin() 
		{
			
		}

		[Test]
		public void EmptyCollection()
		{
			ISession s = sessions.OpenSession();
			object id = s.Save( new Baz() );
			s.Flush();
			s.Close();
			s = sessions.OpenSession();
			Baz baz = (Baz) s.Load(typeof(Baz), id);
			IDictionary foos = baz.FooSet;
			Assert.IsTrue( foos.Count==0 );
			Foo foo = new Foo();
			foos.Add(foo, null);
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
			ISession s = sessions.OpenSession();
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


			s = sessions.OpenSession();
			x = new X();
			y = new Y();

			x.Y = y;
			y.TheX = x;

			s.Save(y);
			s.Flush();

			Assert.IsTrue( s.Contains(y) && s.Contains(x) );
			s.Close();

			Assert.AreEqual( x.Id, y.Id);


			s = sessions.OpenSession();
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
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void Limit() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void Custom() 
		{
		}

		[Test]
		public void SaveAddDelete() 
		{
			ISession s = sessions.OpenSession();
			Baz baz = new Baz();
			IDictionary bars = new Hashtable();
			baz.CascadingBars = bars;
			s.Save(baz);
			s.Flush();

			baz.CascadingBars.Add( new Bar(), new object() );
			s.Delete(baz);
			s.Flush();
			s.Close();

		}

		[Test]
		[Ignore("Fails because Proxies not written yet.")]
		public void NamedParams() 
		{
			Bar bar = new Bar();
			Bar bar2 = new Bar();
			bar.Name = "Bar";
			bar2.Name = "Bar Two";
			Baz baz = new Baz();
			baz.CascadingBars = new Hashtable();
			baz.CascadingBars.Add( bar, new object() );
			bar.Baz = baz;

			ISession s = sessions.OpenSession();
			s.Save(baz);
			s.Save(bar2);

			// TODO: at this point it fails because of SessionImpl.ProxyFor
			IList list = s.Find("from Bar bar left join bar.Baz baz left join baz.CascadingBars b where bar.Name like 'Bar %'");
			object row = list[0];
			Assert.IsTrue( row is object[] && ( (object[])row).Length==3 );


		}

		[Test]
		[Ignore("Test not written yet.")]
		public void Dyna() 
		{
		}

		[Test]
		//[Ignore("Test not written yet.")]
		public void FindByCriteria() 
		{
			ISession s = sessions.OpenSession();
			Foo f = new Foo();
			s.Save(f);
			s.Flush();

			//TODO: need to add PropertyExpressions to Expression namespace.
			IList list = s.CreateCriteria(typeof(Foo))
				.Add( Expression.Expression.Eq( "Integer", f.Integer ) )
				//.Add( Expression.Expression.EqProperty("integer", "integer") )
				.Add( Expression.Expression.Like( "String", f.String) )
				.Add( Expression.Expression.In("Boolean", new bool[] {f.Boolean, f.Boolean} ) )
				.SetFetchMode("TheFoo", FetchMode.Eager)
				.SetFetchMode("baz", FetchMode.Lazy)
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

			Expression.Expression andExpression;
			Expression.Expression orExpression;

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

			//TODO: the SetMaxResults doesn't seem to have any impact
//			list = s.CreateCriteria(typeof(Foo)).SetMaxResults(0).List();
//			Assert.AreEqual(0, list.Count);
			
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

			s = sessions.OpenSession();
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
			Assert.IsTrue(NHibernate.IsInitialized(f.TheFoo));
			
			//TODO: this is initialized because Proxies are not implemented yet.
			//Assert.IsFalse( NHibernate.IsInitialized(f.component.Glarch) );

			s.Delete(f.TheFoo);
			s.Delete(f);
			s.Flush();
			s.Close();
		
		}

		[Test]
		public void AfterDelete() 
		{
			ISession s = sessions.OpenSession();
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
		//[Ignore("Test not written yet.")]
		public void CollectionWhere() 
		{
			Foo foo1 = new Foo();
			Foo foo2 = new Foo();
			Baz baz = new Baz();
			Foo[] arr = new Foo[10];
			arr[0] = foo1;
			arr[9] = foo2;

			ISession s = sessions.OpenSession();
			s.Save(foo1);
			s.Save(foo2);
			baz.FooArray = arr;
			s.Save(baz);
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			baz = (Baz)s.Load(typeof(Baz), baz.Code);
			Assert.AreEqual( 1, baz.FooArray.Length );
			Assert.AreEqual( 1, s.Find("from Baz baz, baz.FooArray foo").Count );
			Assert.AreEqual( 2, s.Find("from Foo foo").Count );
			// TODO: filter is not working because QueryKeyCacheFactor is null
			//Assert.AreEqual( 1, s.Filter(baz.FooArray, "").Count );

			s.Delete("from Foo foo");
			s.Delete(baz);

			IDbCommand deleteCmd = s.Connection.CreateCommand();
			deleteCmd.CommandText = "delete from fooArray where id_='" + baz.Code + "' and i>=8";
			deleteCmd.CommandType = CommandType.Text;
			int rows = deleteCmd.ExecuteNonQuery();
			Assert.AreEqual(1, rows);

			s.Flush();
			s.Close();

		}
		
		[Test]
		public void ComponentParent()
		{
			ISession s = sessions.OpenSession();
			ITransaction t = s.BeginTransaction();

			BarProxy bar = new Bar();
			bar.Component = new FooComponent();
			Baz baz = new Baz();
			baz.Components = new FooComponent[] { new FooComponent(), new FooComponent()};
			s.Save(bar);
			s.Save(baz);
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
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
			ISession s = sessions.OpenSession();
			Baz baz = new Baz();
			baz.SetDefaults();
			s.Save(baz);
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			s.Load( typeof(Baz), baz.Code );
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			baz = (Baz) s.Load( typeof(Baz), baz.Code );
			s.Delete(baz);
			s.Flush();
			s.Close();
		}

		[Test]
		//[Ignore("Test not written yet.")]
		public void AssociationId() 
		{
			ISession s = sessions.OpenSession();
			ITransaction t = s.BeginTransaction();
			Bar bar = new Bar();
			string id = (string)s.Save(bar);
			MoreStuff more = new MoreStuff();
			more.Name = "More Stuff";
			more.IntId = 12;
			more.StringId = "id";
			Stuff stuf = new Stuff();
			stuf.MoreStuff = more;
			more.Stuffs = new ArrayList();
			more.Stuffs.Add(stuf);
			stuf.Foo = bar;
			stuf.Id = 1234;
			
			//stuf.setProperty(TimeZone.getDefault() );
			s.Save(more);
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			//The special property (lowercase) id may be used to reference the unique identifier of an object. (You may also use its property name.) 
			string hqlString = "from s in class Stuff where s.Foo.id = ? and s.id.Id = ? and s.MoreStuff.id.IntId = ? and s.MoreStuff.id.StringId = ?";
			object[] values = new object[] {bar, (long)1234, (int)12, "id" };
			Type.IType[] types = new Type.IType[]
				{
					NHibernate.Entity(typeof(Foo)),
					NHibernate.Int64,
					NHibernate.Int32,
					NHibernate.String 
				};
		

			IList results = s.Find(hqlString, values, types);
			Assert.AreEqual(1, results.Count);

			hqlString = "from s in class Stuff where s.Foo.id = ? and s.id.Id = ? and s.MoreStuff.Name = ?";
			values = new object[] {bar, (long)1234, "More Stuff"};
			types = new Type.IType[] 
				{
					NHibernate.Entity(typeof(Foo)),
					NHibernate.Int64,
					NHibernate.String
				};
			
			results = s.Find(hqlString, values, types);
			Assert.AreEqual(1, results.Count);


			hqlString = "from s in class Stuff where s.Foo.String is not null";
			s.Find(hqlString);

			hqlString = "from s in class Stuff where s.Foo > '0' order by s.Foo";
			results = s.Find(hqlString);
			Assert.AreEqual(1, results.Count);

			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			FooProxy foo = (FooProxy)s.Load(typeof(Foo), id);
			s.Load(more, more);
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			Stuff stuff = new Stuff();
			stuff.Foo = foo;
			stuff.Id = 1234;
			stuff.MoreStuff = more;
			s.Load(stuff, stuff);

			// TODO: figure out what to do with TimeZone
			//Assert.IsTrue( stuff.getProperty().equals( TimeZone.getDefault() ) );
			Assert.AreEqual("More Stuff", stuff.MoreStuff.Name);
			s.Delete("from ms in class MoreStuff");
			s.Delete("from foo in class Foo");
			
			t.Commit();
			s.Close();

		}

		[Test]
		public void CascadeSave() 
		{
			ISession s = sessions.OpenSession();
			ITransaction t = s.BeginTransaction();
			Baz baz = new Baz();
			IList list = new ArrayList();
			list.Add( new Fee() );
			list.Add( new Fee() );
			baz.Fees = list;
			s.Save(baz);
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();

			baz = (Baz)s.Load( typeof(Baz), baz.Code );
			Assert.AreEqual(2, baz.Fees.Count);
			s.Delete(baz);

			IEnumerable enumerable = s.Enumerable("from fee in class Fee");
			Assert.IsFalse( enumerable.GetEnumerator().MoveNext() );
			t.Commit();
			s.Close();
		}

		[Test]
		public void CompositeKeyPathExpressions() 
		{
			ISession s = sessions.OpenSession();

			string hql = "select fum1.Fo from fum1 in class Fum where fum1.Fo.FumString is not null";
			s.Find(hql);
			
			hql = "from fum1 in class Fum where fum1.Fo.FumString is not null order by fum1.Fo.FumString";
			s.Find(hql);
			// TODO: in h2.0.3 there is also HQSLDialect, MckoiDialect, and PointbaseDialect
			if(!(dialect is Dialect.MySQLDialect)) 
			{
				hql = "from fum1 in class Fum where exists elements(fum1.Friends)";
				s.Find(hql);

				hql = "from fum1 in class Fum where size(fum1.Friends) = 0";
				s.Find(hql);
			}

			hql = "select fum1.Friends.elements from fum1 in class Fum";
			s.Find(hql);

			hql = "from fum1 in class Fum, fr in elements( fum1.Friends )";
			s.Find(hql);

			s.Close();

		}

		[Test]
		[Ignore("Test is failing becuase items in Arrays are being deleted.")]
		public void CollectionsInSelect() 
		{
			ISession s = sessions.OpenSession();
			ITransaction t = s.BeginTransaction();
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
			if( !(dialect is Dialect.MySQLDialect) 
				//&& !(dialect is Dialect.HSQLDialect) - don't have in nh yet
				//&& !(dialect is Dialect.MckoiDialect) - commented out in h2.0.3 also
				//&& !(dialect is Dialect.SAPDBDialect) - don't have in nh yet
				//&& !(dialect is Dialect.PointbaseDialect) - don't have in nh yet
				) 
			{
				s.Find("select count(*) from Baz as baz where 1 in indices(baz.FooArray)");
				s.Find("select count(*) from Bar as bar where 'abc' in elements(bar.Baz.FooArray)");
				s.Find("select count(*) from Bar as bar where 1 in indices(bar.Baz.FooArray)");
				s.Find("select count(*) from Bar as bar, bar.Component.Glarch.ProxyArray as g where g.id in indices(bar.Baz.FooArray)");
				s.Find("select max( elements(bar.Baz.FooArray) ) from Bar as bar, bar.Component.Glarch.ProxyArray as g where g.id in indices(bar.Baz.FooArray)");
				//s.Find("select count(*) from Bar as bar where 1 in (from bar.Component.Glarch.ProxyArray g where g.Name='foo')");
				//s.Find("select count(*) from Bar as bar where 1 in (from g in bar.Component.Glarch.ProxyArray.elements where g.Name='foo')");
				//s.Find("select count(*) from Bar as bar left outer join bar.Component.Glarch.ProxyArray as pg where 1 in (from g in bar.Component.Glarch.ProxyArray)");
			}
            
			list = s.Find("from Baz baz left join baz.FooToGlarch join fetch baz.FooArray foo left join fetch foo.TheFoo");
			Assert.AreEqual( 1, list.Count );
			Assert.AreEqual( 2, ((object[])list[0]).Length );

			//TODO: flush is causing array delete
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
			
			//Assert.AreEqual( 0, s.Find("from bar in class Bar, foo in bar.Baz.FooSet.elements").Count );
			//Assert.AreEqual( 1, s.Find("from bar in class Bar, foo in elements( bar.Baz.FooArray )").Count );

			s.Delete(bar);



			s.Delete(baz);
			s.Delete(baz2);
			s.Delete(foos[1]);
			t.Commit();
			s.Close();

		}

		[Test]
		public void NewFlushing() 
		{
			ISession s = sessions.OpenSession();
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
		[Ignore("Test not written yet.")]
		public void PersistCollections() 
		{
		}

		[Test]
		public void SaveFlush() 
		{
			ISession s = sessions.OpenSession();
			Fee fee = new Fee();
			s.Save( fee, "key" );
			fee.Fi = "blah";
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
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
			ISession s = sessions.OpenSession();
			Foo foo = new Foo();
			s.Save(foo);
			foo.String = "dirty";
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			Foo foo2 = new Foo();
			s.Load( foo2, foo.Key );
			Assert.IsTrue( foo.EqualsFoo(foo2), "create-update" );
			s.Delete(foo2);
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			foo = new Foo();
			s.Save(foo, "assignedid");
			foo.String = "dirty";
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			s.Load(foo2, "assignedid");
			Assert.IsTrue( foo.EqualsFoo(foo2), "create-update" );
			s.Delete(foo2);
			s.Flush();
			s.Close();


		}

		[Test]
		public void Update() 
		{
			ISession s = sessions.OpenSession();
			Foo foo = new Foo();
			s.Save(foo);
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			FooProxy foo2 = (FooProxy)s.Load( typeof(Foo), foo.Key );
			foo2.String = "dirty";
			foo2.Boolean = false;
			foo2.Bytes = new byte[] {1,2,3};
			foo2.Date = DateTime.Today;
			foo2.Short = 69;
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			Foo foo3 = new Foo();
			s.Load( foo3, foo.Key );
			Assert.IsTrue( foo2.EqualsFoo(foo3), "update" );
			s.Delete(foo3);
			s.Flush();
			s.Close();

		}

		[Test]
		public void UpdateCollections() 
		{
			ISession s = sessions.OpenSession();
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
			baz.Foos = new Hashtable();
			baz.Foos.Add( f1, new object() );
			s.Save(f1);
			s.Save(f2);
			s.Save(f3);
			s.Save(o);
			s.Save(baz);
			s.Flush();
			s.Close();

			baz.Ones[0] = null;
			baz.Ones.Add(o);
			baz.Foos.Add( f2, new object() );
			foos[0] = f3;
			foos[1] = f1;

			s = sessions.OpenSession();
			s.SaveOrUpdate(baz);
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
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
			
			s = sessions.OpenSession();
			s.SaveOrUpdate(baz);
			s.Delete("from f in class NHibernate.DomainModel.Foo");
			baz.Ones.Remove(o);
			s.Delete("from o in class NHibernate.DomainModel.One");
			s.Delete(baz);
			s.Flush();
			s.Close();
		}

		[Test]
		[Ignore("Test fails because Proxy not implemented yet.")]
		public void Load() 
		{
			ISession s = sessions.OpenSession();
			Qux q = new Qux();
			s.Save(q);
			BarProxy b = new Bar();
			s.Save(b);
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			q = (Qux)s.Load( typeof(Qux), q.Key );
			b = (BarProxy)s.Load( typeof(Foo), b.Key );
			string tempKey = b.Key;
			Assert.IsFalse( NHibernate.IsInitialized(b), "b should have been an unitialized Proxy" );
			string tempBarString = b.BarString;
			Assert.IsTrue( NHibernate.IsInitialized(b), "b should have been an initialized Proxy" );
			BarProxy b2 = (BarProxy)s.Load( typeof(Bar), b.Key );
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
			ISession s = sessions.OpenSession();
			Foo foo = new Foo();
			s.Save(foo);
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
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
			ISession s = sessions.OpenSession();
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

			s = sessions.OpenSession();
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
			ISession s = sessions.OpenSession();
			Bar bar = new Bar();
			s.Save(bar);
			bar.BarString = "bar bar";
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
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
			ISession s = sessions.OpenSession();
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
			ISession s = sessions.OpenSession();
			Bar bar = new Bar();
			s.Save(bar);
			// h2.0.3 was a set
			bar.Abstracts = new Hashtable();
			bar.Abstracts.Add( bar, new object() );
			Bar bar2 = new Bar();
			bar.Abstracts.Add( bar2, new object() );
			bar.TheFoo = bar;
			s.Save(bar2);
			s.Flush();
			s.Close();

			bar.Abstracts = null;
			s = sessions.OpenSession();
			s.Load( bar, bar.Key );

			Assert.AreEqual( 2, bar.Abstracts.Count);
			Assert.IsTrue(bar.Abstracts.Contains(bar), "collection contains self");
			Assert.AreSame(bar, bar.TheFoo, "association to self");

			foreach(object obj in bar.Abstracts.Keys) 
			{
				s.Delete(obj);
			}

			s.Flush();
			s.Close();

		}

		[Test]
		[Ignore("Test not written yet.")]
		public void Find() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void Query() 
		{
		}
	
		[Test]
		public void DeleteRecursive() 
		{
			ISession s = sessions.OpenSession();
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
			ISession s = sessions.OpenSession();
			Baz baz1 = new Baz();
			s.Save(baz1);
			Baz baz2 = new Baz();
			s.Save(baz2);
			baz1.IntArray = new int[] {1, 2, 3, 4};
			baz1.FooSet = new Hashtable();
			Foo foo = new Foo();
			s.Save(foo);
			baz1.FooSet.Add( foo, new object() );
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			baz2 = (Baz) s.Load( typeof(Baz), baz2.Code );
			baz1 = (Baz) s.Load( typeof(Baz), baz1.Code );
			baz2.FooSet = baz1.FooSet;
			baz1.FooSet = null;
			baz2.IntArray = baz1.IntArray;
			baz1.IntArray = null;
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			baz2 = (Baz)s.Load( typeof(Baz), baz2.Code );
			baz1 = (Baz)s.Load( typeof(Baz), baz1.Code );
			Assert.AreEqual(4, baz2.IntArray.Length, "unkeyed reachability - baz2.IntArray");
			Assert.AreEqual(1, baz2.FooSet.Count, "unkeyed reachability - baz2.FooSet");
			Assert.AreEqual(0, baz1.IntArray.Length, "unkeyed reachability - baz1.IntArray");
			Assert.AreEqual(0, baz1.FooSet.Count, "unkeyed reachability - baz1.FooSet");

			foreach(object obj in baz2.FooSet.Keys) 
			{
				s.Delete( (FooProxy)obj );
			}

			s.Delete(baz1);
			s.Delete(baz2);
			s.Flush();
			s.Close();

			// now for collections of collections
			s = sessions.OpenSession();
			baz1 = new Baz();
			s.Save(baz1);
			baz2 = new Baz();
			s.Save(baz2);
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			baz2 = (Baz)s.Load( typeof(Baz), baz2.Code );
			baz1 = (Baz)s.Load( typeof(Baz), baz1.Code );
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			baz2 = (Baz)s.Load( typeof(Baz), baz2.Code );
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			baz2 = (Baz)s.Load( typeof(Baz), baz2.Code );
			baz1 = (Baz)s.Load( typeof(Baz), baz1.Code );
			s.Delete(baz1);
			s.Delete(baz2);
			s.Flush();
			s.Close();

			// now for keyed collections
			s = sessions.OpenSession();
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

			s = sessions.OpenSession();
			baz2 = (Baz)s.Load( typeof(Baz), baz2.Code );
			baz1 = (Baz)s.Load( typeof(Baz), baz1.Code );
			baz2.FooArray = baz1.FooArray;
			baz1.FooArray = null;
			baz2.StringDateMap = baz1.StringDateMap;
			baz1.StringDateMap = null;
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
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
			ISession s = sessions.OpenSession();
			Qux q = new Qux();
			s.Save(q);
			q.Stuff = "foo bar baz qux";
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			q = (Qux)s.Load( typeof(Qux), q.Key );
			Assert.IsTrue( q.Created, "lifecycle create" );
			Assert.IsTrue( q.Loaded, "lifecycle load" );
			Assert.IsNotNull( q.Foo, "lifecycle subobject" );
			s.Delete(q);
			Assert.IsTrue( q.Deleted, "lifecyle delete" );
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			Assert.AreEqual(0, s.Find("from foo in class NHibernate.DomainModel.Foo").Count, "subdeletion");
			s.Flush();
			s.Close();

		}

		[Test]
		//[Ignore("Test not written yet.")]
		public void Iterators() 
		{
			ISession s = sessions.OpenSession();
			for( int i=0; i<10; i++ ) 
			{
				Qux q = new Qux();
				object qid = s.Save(q);
				Assert.IsNotNull(q, "q is not null");
				Assert.IsNotNull(qid, "qid is not null");
			}
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			IEnumerator enumer = s.Enumerable("from q in class NHibernate.DomainModel.Qux where q.Stuff is null").GetEnumerator();
			int count = 0;
			while( enumer.MoveNext() ) 
			{
				Qux q = (Qux)enumer.Current;
				q.Stuff = "foo";
				// can't remove item from IEnumerator in .net 
				//if (count==0 || count==5) enumer.Remove();
				count++;
			}

			Assert.AreEqual(10, count, "found 10 items");
			s.Flush();
			s.Close();
			
			s = sessions.OpenSession();

			Assert.AreEqual(10, 
				s.Delete("from q in class NHibernate.DomainModel.Qux where q.Stuff=?", "foo", NHibernate.String),
				"delete by query");
			
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			enumer = s.Enumerable("from q in class NHibernate.DomainModel.Qux").GetEnumerator();
			Assert.IsFalse( enumer.MoveNext() , "no items in enumerator" );
			s.Flush();
			s.Close();


		}

		[Test]
		public void Versioning() 
		{
			ISession s = sessions.OpenSession();
			GlarchProxy g = new Glarch();
			s.Save(g);
			GlarchProxy g2 = new Glarch();
			s.Save(g2);
			object gid = s.GetIdentifier(g);
			object g2id = s.GetIdentifier(g2);
			g.Name = "glarch";
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
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
			s.Close();

			s = sessions.OpenSession();
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
			s.Close();
		}

		[Test]
		public void VersionedCollections() 
		{
			ISession s = sessions.OpenSession();
			GlarchProxy g = new Glarch();
			s.Save(g);
			g.ProxyArray = new GlarchProxy[] {g};
			string gid = (string) s.GetIdentifier(g);
			ArrayList list = new ArrayList();
			list.Add("foo");
			g.Strings = list;
			// <sets> in h2.0.3
			IDictionary hashset = new Hashtable();
			hashset[g] = new object();
			g.ProxySet = hashset;
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			g = (GlarchProxy)s.Load( typeof(Glarch), gid );
			Assert.AreEqual( 1, g.Strings.Count );
			Assert.AreEqual( 1, g.ProxyArray.Length );
			Assert.AreEqual( 1, g.ProxySet.Count );
			Assert.AreEqual( 1, g.Version, "version collection before");
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			g = (GlarchProxy)s.Load( typeof(Glarch), gid );
			Assert.AreEqual( "foo", g.Strings[0] );
			Assert.AreSame( g, g.ProxyArray[0] );
			IEnumerator enumer = g.ProxySet.Keys.GetEnumerator();
			enumer.MoveNext();
			Assert.AreSame( g, enumer.Current );
			Assert.AreEqual(1, g.Version, "versioned collection before");
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			g = (GlarchProxy)s.Load( typeof(Glarch), gid );
			Assert.AreEqual( 1, g.Version, "versioned collection before");
			g.Strings.Add("bar");
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			g = (GlarchProxy)s.Load( typeof(Glarch), gid );
			Assert.AreEqual( 2, g.Version, "versioned collection after" );
			Assert.AreEqual( 2, g.Strings.Count, "versioned collection after" );
			g.ProxyArray = null;
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			g = (GlarchProxy)s.Load( typeof(Glarch), gid );
			Assert.AreEqual( 3, g.Version, "versioned collection after" );
			Assert.AreEqual( 0, g.ProxyArray.Length, "version collection after" );
			g.FooComponents = new ArrayList();
			g.ProxyArray = null;
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
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
			ISession s = sessions.OpenSession();
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
				object obj = enumer.Current;
			}

			IList list = s.Find("from g in class NHibernate.DomainModel.Glarch");
			Assert.AreEqual( 6, list.Count, "recursive find" );
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			list = s.Find("from g in class NHibernate.DomainModel.Glarch");
			Assert.AreEqual( 6, list.Count, "recursive iter" );
			list = s.Find("from g in class NHibernate.DomainModel.Glarch where g.Next is not null");
			Assert.AreEqual( 5, list.Count, "exclude the null next" );
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
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
			s = sessions.OpenSession();
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
				object obj = enumer.Current;
			}

			list = s.Find("from foo in class NHibernate.DomainModel.Foo");
			Assert.AreEqual( 6, list.Count, "recursive find");
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			list = s.Find("from foo in class NHibernate.DomainModel.Foo");
			Assert.AreEqual( 6, list.Count, "recursive iter" );
			enumer = list.GetEnumerator();
			while( enumer.MoveNext() ) 
			{
				Assert.IsTrue( enumer.Current is BarProxy, "polymorphic recursive load" );
			}
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
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
		[Ignore("Test not written yet.")]
		public void ScrollableIterator() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void MultiColumnQueries() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void DeleteTransient() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void UpdateFromTransient() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void Databinder() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void ArrayOfTimes() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void Components() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void Enum() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void NoForeignKeyViolations() 
		{
		}

		[Test]
		public void LazyCollections() 
		{
			ISession s = sessions.OpenSession();
			Qux q = new Qux();
			s.Save(q);
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			q = (Qux)s.Load( typeof(Qux), q.Key );
			s.Flush();
			s.Close();

			// two exceptions are supposed to occur:")
			bool ok = false;
			try 
			{
				int i = q.MoreFums.Count;
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
				int j = q.Fums.Count;
			}
			catch (LazyInitializationException lie) 
			{
				System.Diagnostics.Debug.WriteLine("caught expected " + lie.ToString());
				ok = true;
			}

			Assert.IsTrue( ok, "lazy collection with many-to-many" );

			s = sessions.OpenSession();
			q = (Qux)s.Load( typeof(Qux), q.Key );
			s.Delete(q);
			s.Flush();
			s.Close();
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void NewSessionLifecycle() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void Disconnect() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void OrderBy() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void ManyToOne() 
		{
		}

		[Test]
		[Ignore("won't work without proxy")]
		public void SaveDelete()
		{
			ISession s = sessions.OpenSession();
			Foo f = new Foo();
			s.Save(f);
			s.Flush();
			s.Close();
		
			s = sessions.OpenSession();
			s.Delete( s.Load( typeof(Foo), f.Key ) );
			s.Flush();
			s.Close();
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void ProxyArray() 
		{
		}

		[Test]
		public void Cache() 
		{
			ISession s = sessions.OpenSession();
			Immutable im = new Immutable();
			s.Save(im);
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			s.Load( im, im.Id);
			s.Close();

			s = sessions.OpenSession();
			s.Load( im, im.Id);
	
			Immutable imFromFind = (Immutable)s.Find("from im in class Immutable where im = ?", im, NHibernate.Entity(typeof(Immutable)))[0];
			Immutable imFromLoad = (Immutable)s.Load(typeof(Immutable), im.Id);
			
			Assert.IsTrue(im==imFromFind, "cached object identity from Find ");
			Assert.IsTrue(im==imFromLoad, "cached object identity from Load ");
			
			s.Close();

		}

		[Test]
		[Ignore("Test not written yet.")]
		public void FindLoad() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void Refresh() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void AutoFlush() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void Veto() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void SerializableType() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void AuotFlushCollections() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void UserProvidedConnection() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void CachedCollection() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void ComplicatedQuery() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void LoadAfterDelete() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void ObjectType() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void Any() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void EmbeddedCompositeID() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void AutosaveChildren() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void OrphanDelete() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void TransientOrphanDelete() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void ProxiesInCollections() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void Service() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void PSCache() 
		{
		}


	}
}
