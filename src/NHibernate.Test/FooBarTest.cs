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
										  "Baz.hbm.xml"
										  //										  "Simple.hbm.xml",
										  //										  "Fumm.hbm.xml",
										  //										  "Fo.hbm.xml",
										  //										  "One.hbm.xml",
										  //										  "Many.hbm.xml",
										  //										  "Immutable.hbm.xml",
										  //										  "Vetoer.hbm.xml",
										  //										  "Holder.hbm.xml",
										  //										  "Location.hbm.xml",
										  //										  "Stuff.hbm.xml",
										  //										  "Container.hbm.xml",
										  //										  "XY.hbm.xml"});
									  }, true);
		}

		[Test]
			//[Ignore("Generated SQL contains bad quoting")]
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
			s.Find("from Baz baz left join fetch baz.fooBag");
			Assert.IsTrue( NHibernate.IsInitialized(fooBag) );
			Assert.IsTrue( fooBag==baz.fooBag );
			Assert.IsTrue( baz.fooBag.Count==2 );
			s.Close();
			
			s = sessions.OpenSession();
			baz = (Baz) s.Load( typeof(Baz), baz.code );
			Object bag = baz.fooBag;
			Assert.IsFalse( NHibernate.IsInitialized(bag) );
			s.Find("from Baz baz left join fetch baz.fooBag");
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
			Baz b = new Baz();
			IDictionary ss = new Hashtable();
			ss.Add(b, new Sortable[] {new Sortable("foo") });
//			ss.Add(b, new Sortable("bar") );
//			ss.Add(b, new Sortable("baz") );
			b.sortablez = ss;
			s.Save(b);
			s.Flush();
			s.Close();
		
			s = sessions.OpenSession();
			IList result = s.CreateCriteria(typeof(Baz))
				.AddOrder( Expression.Order.Asc("name") )
				.List();
			b = (Baz) result[0];
			Assert.IsTrue( b.sortablez.Count==3 );
			Assert.AreEqual( ( (Sortable) b.sortablez[0] ).name, "bar" );
			s.Flush();
			s.Close();
		}

	}
}
