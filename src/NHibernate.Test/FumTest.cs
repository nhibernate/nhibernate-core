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

		static FumCompositeID FumKey(String str) 
		{
		
			return FumKey(str,false);
		}
	
		static FumCompositeID FumKey(String str, bool aCompositeQueryTest) 
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
			Fum fum = new Fum( FumTest.FumKey("fum") );
			fum.FumString = "fee fi fo";
			s.Save(fum);

			Assert.AreSame( fum, s.Load( typeof(Fum), FumTest.FumKey("fum"), LockMode.Upgrade ) );
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
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
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			IEnumerator enumerator = s.Enumerable("from fum in class NHibernate.DomainModel.Fum where not fum.FumString='FRIEND'").GetEnumerator();
			int i = 0;
			while(enumerator.MoveNext()) 
			{
				fum = (Fum) enumerator.Current;
				s.Delete(fum);
				i++;
			}

			Assert.AreEqual(2, i, "Iterate on Composite Key");
			s.Flush();
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
		[Ignore("Test not yet written")]
		public void CompositeIDQuery() 
		{
		}

		[Test]
		[Ignore("Test not yet written")]
		public void CompositeIDCollections() 
		{
		}

		[Test]
		[Ignore("Test not yet written")]
		public void DeleteOwner() 
		{
		}

		[Test]
		[Ignore("Test not yet written")]
		public void CompositeIDs() 
		{
		}

		

		[Test]
		[Ignore("Test not yet written")]
		public void KeyManyToOne() 
		{
		}

		
	}
}
