using NHibernate;
using NHibernate.DomainModel;

using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest
{
	[TestFixture]
	public class GetTest : TestCase
	{
		[SetUp]
		public void SetUp()
		{
			// A class with a proxy is needed to actually test Get vs Load.
			ExportSchema( new string[] { "ABCProxy.hbm.xml" }, true );
		}

		[Test]
		public void GetVsLoad()
		{
			A a = new A( "name" );

			using( ISession s = sessions.OpenSession() )
			{
				s.Save( a );
			}

			using( ISession s = sessions.OpenSession() )
			{
				A loadedA = ( A )s.Load( typeof( A ), a.Id );
				Assert.IsFalse( NHibernateUtil.IsInitialized( loadedA ),
					"Load should not initialize the object" );

				Assert.IsNotNull( s.Load( typeof( A ), 2 ),
					"Loading non-existent object should not return null" );
			}

			using( ISession s = sessions.OpenSession() )
			{
				A gotA = (A)s.Get( typeof( A ), a.Id );
				Assert.IsTrue(NHibernateUtil.IsInitialized( gotA ),
					"Get should initialize the object" );

				Assert.IsNull( s.Get( typeof( A ), 2 ),
					"Getting non-existent object should return null" );
			}
		}

		[Test]
		public void GetAndModify() 
		{
			A a = new A( "name" );
			ISession s = sessions.OpenSession();
			s.Save( a );
			s.Close();

			s = sessions.OpenSession();
			a = s.Get( typeof(A), a.Id ) as A;
			a.Name = "modified";
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			a = s.Get( typeof(A), a.Id ) as A;
			Assert.AreEqual( "modified", a.Name, "the name was modified" );
			s.Close();
		}
	}
}
