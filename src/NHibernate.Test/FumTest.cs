using System;
using System.Collections;

using NHibernate.DomainModel;

using NUnit.Framework;

namespace NHibernate.Test 
{

	/// <summary>
	/// FumTest handles testing Composite Ids.
	/// </summary>
	[TestFixture]
	public class FumTest : TestCase
	{
		protected static short fumKeyShort = 1;
	
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
					"Immutable.hbm.xml",
					"Fee.hbm.xml",
					"Vetoer.hbm.xml",
					"Holder.hbm.xml",
					"Location.hbm.xml",
					"Stuff.hbm.xml",
					"Container.hbm.xml",
					"Simple.hbm.xml",
					"Middle.hbm.xml"
				}, true);
		}

		public static FumCompositeID FumKey(String str) 
		{
		
			return FumKey(str,false);
		}
	
		public static FumCompositeID FumKey(String str, bool aCompositeQueryTest) 
		{
			FumCompositeID id = new FumCompositeID();
//			if( dialect is Dialect.MckoiDialect ) 
//												{
//													  GregorianCalendar now = new GregorianCalendar();
//													  GregorianCalendar cal = new GregorianCalendar( 
//														  now.get(java.util.Calendar.YEAR),
//														  now.get(java.util.Calendar.MONTH),
//														  now.get(java.util.Calendar.DATE) 
//														  );
//													  id.setDate( cal.getTime() );
//												  }
//			else 
//			{
			id.Date = new DateTime(2004, 4, 29, 9, 0, 0, 0);
//				 }
			id.String = str;
		
			if (aCompositeQueryTest) 
			{
				id.Short = fumKeyShort++ ;
			}
			else 
			{
				id.Short = (short)12 ;
			}
		
			return id;
		}

		[Test]
		public void ListIdentifiers() 
		{
			ISession s = sessions.OpenSession();
			Fum fum = new Fum( FumTest.FumKey("fum") );
			fum.FumString = "fo fee fi";
			s.Save(fum);

			fum = new Fum( FumTest.FumKey("fi") );
			fum.FumString = "fee fi fo";
			s.Save(fum);

			// not doing a flush because the Find will do an auto flush unless we tell the session a 
			// different FlushMode
			IList list = s.Find("select fum.Id from fum in class NHibernate.DomainModel.Fum where not fum.FumString = 'FRIEND'");
			
			Assert.AreEqual(2, list.Count, "List Identifiers");

			IEnumerator enumerator = s.Enumerable("select fum.Id from fum in class NHibernate.DomainModel.Fum where not fum.FumString='FRIEND'").GetEnumerator();
			int i = 0;
			while(enumerator.MoveNext()) 
			{
				Assert.IsTrue(enumerator.Current is FumCompositeID, "Iterating Identifiers");
				i++;
			}

			Assert.AreEqual(2, i, "Number of Ids found.");

			// clean up by deleting the 2 Fum objects that were added.
			s.Delete( s.Load( typeof(Fum), list[0] ) );
			s.Delete( s.Load( typeof(Fum), list[1] ) );
			s.Flush();
			s.Close();


		}
		
		[Test]
		public void CompositeID() 
		{
			ISession s = sessions.OpenSession();
			ITransaction t = s.BeginTransaction();
			Fum fum = new Fum( FumTest.FumKey("fum") );
			fum.FumString = "fee fi fo";
			s.Save(fum);

			Assert.AreSame( fum, s.Load( typeof(Fum), FumTest.FumKey("fum"), LockMode.Upgrade ) );
			//s.Flush();
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			fum = (Fum) s.Load( typeof(Fum), FumTest.FumKey("fum"), LockMode.Upgrade );
			Assert.IsNotNull(fum, "Load by composite key");

			Fum fum2 = new Fum( FumTest.FumKey("fi") );
			fum2.FumString = "fee fo fi";
			fum.Fo = fum2;
			s.Save(fum2);

			IList list = s.Find("from fum in class NHibernate.DomainModel.Fum where not fum.FumString='FRIEND'");
			Assert.AreEqual(2, list.Count, "Find a List of Composite Keyed objects");

			IList list2 = s.Find("select fum from fum in class NHibernate.DomainModel.Fum where fum.FumString='fee fi fo'");
			Assert.AreEqual(fum, (Fum)list2[0], "Find one Composite Keyed object");

			fum.Fo = null;
			//s.Flush();
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			IEnumerator enumerator = s.Enumerable("from fum in class NHibernate.DomainModel.Fum where not fum.FumString='FRIEND'").GetEnumerator();
			int i = 0;
			while(enumerator.MoveNext()) 
			{
				fum = (Fum) enumerator.Current;
				s.Delete(fum);
				i++;
			}

			Assert.AreEqual(2, i, "Iterate on Composite Key");
			//s.Flush();
			t.Commit();
			s.Close();

		}

		[Test]
		public void CompositeIDOneToOne()
		{
			ISession s = sessions.OpenSession();
			Fum fum = new Fum( FumKey("fum") );
			fum.FumString = "fee fi fo";
			//s.Save(fum); commented out in h2.0.3
			Fumm fumm = new Fumm();
			fumm.Fum = fum;
			s.Save(fumm);
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			fumm = (Fumm) s.Load( typeof(Fumm), FumKey("fum") );
			//s.delete(fumm.Fum); commented out in h2.0.3
			s.Delete(fumm);
			s.Flush();
			s.Close();
					   
		}

		[Test]
		public void CompositeIDQuery() 
		{
			ISession s = sessions.OpenSession();
			Fum fee = new Fum( FumTest.FumKey("fee", true) );
			fee.FumString = "fee";
			s.Save(fee);
			Fum fi = new Fum( FumTest.FumKey("fi", true) );
			fi.FumString = "fi";
			short fiShort = fi.Id.Short;
			s.Save(fi);
			Fum fo = new Fum( FumTest.FumKey("fo", true) );
			fo.FumString = "fo";
			s.Save(fo);
			Fum fum = new Fum( FumTest.FumKey("fum", true) );
			fum.FumString = "fum";
			s.Save(fum);
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			// Try to find the Fum object "fo" that we inserted searching by the string in the id
			IList vList = s.Find("from fum in class NHibernate.DomainModel.Fum where fum.Id.String='fo'");
			Assert.AreEqual( 1, vList.Count, "find by composite key query (find fo object)" );
			fum = (Fum)vList[0];
			Assert.AreEqual( "fo", fum.Id.String, "find by composite key query (check fo object)" );

			// Try to fnd the Fum object "fi" that we inserted by searching the date in the id
			vList = s.Find("from fum in class NHibernate.DomainModel.Fum where fum.Id.Short = ?", fiShort, NHibernate.Int16 );
			Assert.AreEqual( 1, vList.Count, "find by composite key query (find fi object)" );
			fi = (Fum)vList[0];
			Assert.AreEqual( "fi", fi.Id.String, "find by composite key query (check fi object)" );
			
			// make sure we can return all of the objects by searching by the date id
			vList = s.Find("from fum in class NHibernate.DomainModel.Fum where fum.Id.Date <= ? and not fum.FumString='FRIEND'", DateTime.Now, NHibernate.Date);
			Assert.AreEqual( 4, vList.Count, "find by composite key query with arguments" );
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			Assert.IsTrue( s.Enumerable("select fum.Id.Short, fum.Id.Date, fum.Id.String from fum in class NHibernate.DomainModel.Fum").GetEnumerator().MoveNext() );
			Assert.IsTrue( s.Enumerable("select fum.Id from fum in class NHibernate.DomainModel.Fum").GetEnumerator().MoveNext() );

			IQuery qu = s.CreateQuery("select fum.FumString, fum, fum.FumString, fum.Id.Date from fum in class NHibernate.DomainModel.Fum");
			Type.IType[] types = qu.ReturnTypes;
			Assert.AreEqual( 4, types.Length );
			for( int k=0; k<types.Length; k++) 
			{
				Assert.IsNotNull( types[k] );
			}
			Assert.IsTrue( types[0] is Type.StringType );
			Assert.IsTrue( types[1] is Type.EntityType );
			Assert.IsTrue( types[2] is Type.StringType );
			Assert.IsTrue( types[3] is Type.DateTimeType );
			IEnumerator enumer = qu.Enumerable().GetEnumerator();
			int j = 0;
			while( enumer.MoveNext() ) 
			{
				j++;
				Assert.IsTrue( ((object[])enumer.Current)[1] is Fum );
			}
			Assert.AreEqual( 8, j, "iterate on composite key" );

			fum = (Fum)s.Load( typeof(Fum), fum.Id );
			s.Filter( fum.QuxArray, "where this.Foo is null" );
			s.Filter( fum.QuxArray, "where this.Foo.id = ?", "fooid", NHibernate.String );
			IQuery f = s.CreateFilter( fum.QuxArray, "where this.Foo.id = :fooId" );
			f.SetString("fooId", "abc");
			Assert.IsFalse( f.Enumerable().GetEnumerator().MoveNext() );

			enumer = s.Enumerable("from fum in class NHibernate.DomainModel.Fum where not fum.FumString='FRIEND'").GetEnumerator();
			int i = 0;
			while( enumer.MoveNext() ) 
			{
				fum = (Fum)enumer.Current;
				s.Delete(fum);
				i++;
			}
			Assert.AreEqual( 4, i, "iterate on composite key" );
			s.Flush();

			s.Enumerable("from fu in class Fum, fo in class Fum where fu.Fo.Id.String = fo.Id.String and fo.FumString is not null");
			s.Find("from Fumm f1 inner join f1.Fum f2");
			s.Close();
		}

		[Test]
		public void CompositeIDCollections() 
		{
			ISession s = sessions.OpenSession();
			Fum fum1 = new Fum( FumTest.FumKey("fum1") );
			Fum fum2 = new Fum( FumTest.FumKey("fum2") );
			fum1.FumString = "fee fo fi";
			fum2.FumString = "fee fo fi";
			s.Save(fum1);
			s.Save(fum2);
			Qux q = new Qux();
			s.Save(q);
			IDictionary dict = new Hashtable();
			IList list = new ArrayList();
			dict.Add(fum1, new object() );
			dict.Add(fum2, new object() );
			list.Add(fum1);
			q.Fums = dict;
			q.MoreFums = list;
			fum1.QuxArray = new Qux[] {q};
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			q = (Qux)s.Load( typeof(Qux), q.Key );
			Assert.AreEqual( 2, q.Fums.Count, "collection of fums" );
			Assert.AreEqual( 1, q.MoreFums.Count, "collection of fums" );
			Assert.AreSame( q, ((Fum)q.MoreFums[0]).QuxArray[0], "unkeyed composite id collection" );
			IEnumerator enumer = q.Fums.Keys.GetEnumerator();
			enumer.MoveNext();
			s.Delete( (Fum)enumer.Current );
			enumer.MoveNext();
			s.Delete( (Fum)enumer.Current );
			s.Delete(q);
			s.Flush();
			s.Close();
		}

		[Test]
		public void DeleteOwner() 
		{
			ISession s = sessions.OpenSession();
			Qux q = new Qux();
			s.Save(q);
			Fum f1 = new Fum( FumTest.FumKey("f1") );
			Fum f2 = new Fum( FumTest.FumKey("f2") );
			IDictionary dict = new Hashtable();
			dict.Add( f1, new object() );
			dict.Add( f2, new object() );
			IList list = new ArrayList();
			list.Add(f1);
			list.Add(f2);
			f1.FumString = "f1";
			f2.FumString = "f2";
			q.Fums = dict;
			q.MoreFums = list;
			s.Save(f1);
			s.Save(f2);
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			ITransaction t = s.BeginTransaction();
			q = (Qux)s.Load( typeof(Qux), q.Key, LockMode.Upgrade );
			s.Lock( q, LockMode.Upgrade );
			s.Delete(q);
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			list = s.Find("from fum in class NHibernate.DomainModel.Fum where not fum.FumString='FRIEND'");
			Assert.AreEqual( 2, list.Count, "deleted owner" );
			s.Lock( list[0], LockMode.Upgrade );
			s.Lock( list[1], LockMode.Upgrade );
			foreach( object obj in list ) 
			{
				s.Delete(obj);
			}
			t.Commit();
			s.Close();


		}

		[Test]
		public void CompositeIDs() 
		{
			ISession s = sessions.OpenSession();
			Fo fo = Fo.NewFo();
			fo.Buf = System.Text.Encoding.ASCII.GetBytes("abcdefghij1`23%$*^*$*\n\t");
			s.Save( fo, FumTest.FumKey("an instance of fo") );
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			fo = (Fo)s.Load( typeof(Fo), FumTest.FumKey("an instance of fo") );
			fo.Buf[1] = (byte)126;
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			fo = (Fo)s.Load( typeof(Fo), FumTest.FumKey("an instance of fo") );
			Assert.AreEqual( 126, fo.Buf[1] );
			IEnumerator enumer = s.Enumerable("from fo in class NHibernate.DomainModel.Fo where fo.id.String like 'an instance of fo'").GetEnumerator();
			Assert.IsTrue( enumer.MoveNext() );
			Assert.AreSame( fo, enumer.Current );
			s.Delete(fo);
			s.Flush();
			try 
			{
				s.Save( Fo.NewFo() );
				Assert.Fail("should not get here"); 
			}
			catch(Exception e) 
			{
				Assert.IsNotNull(e);
			}
			s.Close();
		}

		

		[Test]
		[Ignore("HQL can't parse a class named 'Order' - http://jira.nhibernate.org:8080/browse/NH-81, this test passes when changed to NHibernate.DomainModel")]
		public void KeyManyToOne() 
		{
			ISession s = sessions.OpenSession();
			Inner sup = new Inner();
			InnerKey sid = new InnerKey();
			sup.Dudu = "dudu";
			sid.AKey = "a";
			sid.BKey = "b";
			sup.Id = sid;
			Middle m = new Middle();
			MiddleKey mid = new MiddleKey();
			mid.One = "one";
			mid.Two = "two";
			mid.Sup = sup;
			m.Id = mid;
			m.Bla = "bla";
			Outer d = new Outer();
			OuterKey did = new OuterKey();
			did.Master = m;
			did.DetailId = "detail";
			d.Id = did;
			d.Bubu = "bubu";
			s.Save(sup);
			s.Save(m);
			s.Save(d);
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			d = (Outer)s.Load( typeof(Outer), did );
			Assert.AreEqual( "dudu", d.Id.Master.Id.Sup.Dudu );
			s.Delete(d);
			s.Delete( d.Id.Master );
			s.Save( d.Id.Master );
			s.Save(d);
			s.Flush();
			s.Close();
			
			s = sessions.OpenSession();
			d = (Outer)s.Find("from Outer o where o.id.DetailId=?", d.Id.DetailId, NHibernate.String)[0];
			s.Find("from Outer o where o.Id.Master.Id.Sup.Dudu is not null");
			s.Find("from Outer o where o.Id.Master.Bla = ''");
			s.Find("from Outer o where o.Id.Master.Id.One = ''");
			s.Delete(d);
			s.Delete(d.Id.Master);
			s.Delete(d.Id.Master.Id.Sup);
			s.Flush();
			s.Close();
		}
	}
}
