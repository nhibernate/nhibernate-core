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
		//[Ignore("Test not written yet.")]
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

			//TODO: don't have an s.iterate() equivalent built
			IList fees = s.Find("from fee in class Fee");
			Assert.AreEqual(0, fees.Count);
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
		[Ignore("Test is failing becuase of Arrays in hql and a reference to them.")]
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
			Assert.AreEqual( 696969696969696969L, r.Amount );

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
            // TODO: it looks like we are having problems with arrays - FooArray to foo and then getting a property of the item
			// in the array.
			list = s.Find("from Baz baz left join baz.FooToGlarch join fetch baz.FooArray foo left join fetch foo.TheFoo");
			Assert.AreEqual( 1, list.Count );
			Assert.AreEqual( 2, ((object[])list[0]).Length );


			s.Delete(bar);



			s.Delete(baz);
			s.Delete(baz2);
			s.Delete(foos[1]);
			t.Commit();
			s.Close();

		}

		[Test]
		[Ignore("Test not written yet.")]
		public void NewFlushing() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void PersistCollections() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void SaveFlush() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void CreateUpdate() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void Update() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void UpdateCollections() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void Load() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void Create() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void Callback() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void Polymorphism() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void RemoveContains() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void CollectionOfSelf() 
		{
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
		[Ignore("Test not written yet.")]
		public void DeleteRecursive() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void Reachability() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void PersistentLifecycle() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void Iterators() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void Versioning() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void VersionedCollections() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void RecursiveLoad() 
		{
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
		[Ignore("Test not written yet.")]
		public void LazyCollections() 
		{
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
