using System;
using NUnit.Framework;
using NHibernate;
using NHibernate.DomainModel;
using System.Collections;

namespace NHibernate.Test
{
	[TestFixture]
	public class FooBarTest : TestCase
	{
		[SetUp]
		public void SetUp()
		{
			ExportSchema(new string[] {   "FooBar.hbm.xml",
										  "Glarch.hbm.xml",
										  "Fee.hbm.xml",
										  "Qux.hbm.xml",
										  "Fum.hbm.xml",
										  "Baz.hbm.xml",
										  //										  "Simple.hbm.xml",
										  //										  "Fumm.hbm.xml",
										  //										  "Fo.hbm.xml",
										  //										  "One.hbm.xml",
										  //										  "Many.hbm.xml",
										  "Immutable.hbm.xml"
										  //										  "Vetoer.hbm.xml",
										  //										  "Holder.hbm.xml",
										  //										  "Location.hbm.xml",
										  //										  "Stuff.hbm.xml",
										  //										  "Container.hbm.xml",
										  //										  "XY.hbm.xml"});
									  }, true);
		}

		[TearDown]
		public void TearDown() 
		{
			DropSchema();
		}

		[Test]
		[Ignore("Fails because Proxies are not working.")]
		public void FetchInitializedCollection()
		{
			ISession s = sessions.OpenSession();
			Baz baz = new Baz();
			IList fooBag = new ArrayList();
			fooBag.Add( new Foo() );
			fooBag.Add( new Foo() );
			baz.fooBag=fooBag;
			s.Save(baz);
			fooBag = baz.fooBag;
			s.Find("from Baz baz left join fetch fooBag");
			Assert.IsTrue( NHibernate.IsInitialized(fooBag) );
			Assert.IsTrue( fooBag==baz.fooBag );
			Assert.IsTrue( baz.fooBag.Count==2 );
			s.Close();
			
			s = sessions.OpenSession();
			baz = (Baz) s.Load( typeof(Baz), baz.code );
			Object bag = baz.fooBag;
			Assert.IsFalse( NHibernate.IsInitialized(bag) );
			s.Find("from Baz baz left join fetch fooBag");
			Assert.IsFalse( NHibernate.IsInitialized(bag) );
			Assert.IsTrue( bag==baz.fooBag );
			Assert.IsTrue( baz.fooBag.Count==2 );
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
			b.sortablez = ss;
			s.Save(b);
			s.Flush();
			t.Commit();
			s.Close();
		
			s = sessions.OpenSession();
			t = s.BeginTransaction();
			IList result = s.CreateCriteria(typeof(Baz))
				.AddOrder( Expression.Order.Asc("name") )
				.List();
			b = (Baz) result[0];
			Assert.IsTrue( b.sortablez.Count==3 );
			
			// compare the first item in the "Set" sortablez - can't reference
			// the first item using b.sortablez[0] because it thinks 0 is the
			// DictionaryEntry key - not the index.
			foreach(DictionaryEntry de in b.sortablez) 
			{
				Assert.AreEqual( ((Sortable)de.Key).name, "bar");
				break;
			}
		
			s.Flush();
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			result = s.CreateQuery("from Baz baz left join fetch sortablez order by baz.name asc")
				.List();
			b = (Baz) result[0];
			Assert.IsTrue( b.sortablez.Count==3 );
			foreach(DictionaryEntry de in b.sortablez) 
			{
				Assert.AreEqual( ((Sortable)de.Key).name, "bar");
				break;
			}
			s.Flush();
			t.Commit();
			s.Close();
		
			s = sessions.OpenSession();
			t = s.BeginTransaction();
			result = s.CreateQuery("from Baz baz order by baz.name asc")
				.List();
			b = (Baz) result[0];
			Assert.IsTrue( b.sortablez.Count==3 );
			foreach(DictionaryEntry de in b.sortablez) 
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
			baz.fees = list;
			list = s.Find("from Foo foo, Baz baz left join fetch fees");
			Assert.IsTrue( NHibernate.IsInitialized( ( (Baz) ( (object[]) list[0] )[1] ).fees ) );
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
			baz.bazez =list;
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
		[Ignore("won't work without proxy")]
		public void SaveDelete()
		{
			ISession s = sessions.OpenSession();
			Foo f = new Foo();
			s.Save(f);
			s.Flush();
			s.Close();
		
			s = sessions.OpenSession();
			s.Delete( s.Load( typeof(Foo), f.key ) );
			s.Flush();
			s.Close();
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
			IDictionary foos = baz.fooSet;
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
	}
}
