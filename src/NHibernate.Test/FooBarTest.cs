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

		[Test]
		[Ignore("Fails because Proxies are not working.")]
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
		[Ignore("Test not written yet.")]
		public void QueryLockMode() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void ManyToManyBag() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void IdBag() 
		{
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
		[Ignore("Test not written yet.")]
		public void OneToOneGenerator() 
		{
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
		[Ignore("Test not written yet.")]
		public void SaveAddDelete() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void NamedParams() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void Dyna() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void FindByCriteria() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void AfterDelete() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void CollectionWhere() 
		{
		}
		
		[Test]
		[Ignore("Test not written yet.")]
		public void ComponentParent()
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void CollectionCache() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void AssociationId() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void CascadeSave() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void CompositeKeyPathExpressions() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void CollectionsInSelect() 
		{
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
			s.Delete( s.Load( typeof(Foo), f.key ) );
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
