using System;

using NHibernate.DomainModel;

using NUnit.Framework;

namespace NHibernate.Test 
{

	/// <summary>
	/// Summary description for FumTest.
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
					//"Vetoer.hbm.xml",
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
		[Ignore("Test not yet written")]
		public void CompositeID() 
		{
		}

		[Test]
		[Ignore("Test not yet written")]
		public void CompositeIDCollections() 
		{
		}

		[Test]
		public void CompositeIDOneToOne()
		{
			ISession s = sessions.OpenSession();
			Fum fum = new Fum( FumKey("fum") );
			fum.fum = "fee fi fo";
			//s.Save(fum);
			Fumm fumm = new Fumm();
			fumm.Fum = fum;
			s.Save(fumm);
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			fumm = (Fumm) s.Load( typeof(Fumm), FumKey("fum") );
			//s.delete(fumm.Fum);
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
		public void CompositeIDs() 
		{
		}

		[Test]
		[Ignore("Test not yet written")]
		public void DeleteOwner() 
		{
		}

		[Test]
		[Ignore("Test not yet written")]
		public void KeyManyToOne() 
		{
		}

		[Test]
		[Ignore("Test not yet written")]
		public void ListIdentifiers() 
		{
		}
	}
}
