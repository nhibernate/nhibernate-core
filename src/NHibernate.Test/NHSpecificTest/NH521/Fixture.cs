using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH521
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH521"; }
		}

		[Test]
		public void AttachUninitProxyCausesInit()
		{
			// First, save a ReferringEntity (which will have a ReferenceToLazyEntity)
			int id = 0;

			using( ISession session = OpenSession() )
			using( ITransaction transaction = session.BeginTransaction() )
			{
				id = ( int ) session.Save( new ReferringEntity() );
				transaction.Commit();
			}

			// Then, load the ReferringEntity (its ReferenceToLazyEntity will be uninit)
			LazyEntity uninitEntity = null;

			using( ISession session = OpenSession() )
			{
				uninitEntity = ( session.Load( typeof( ReferringEntity ), id ) as ReferringEntity ).ReferenceToLazyEntity;
			}

			Assert.IsFalse(
				NHibernateUtil.IsInitialized( uninitEntity ),
				"The reference to a lazy entity is not unitialized at the loading of the referring entity." );

			// Finally, lock the uninitEntity and test if it gets initialized
			using( ISession session = OpenSession() )
			using( ITransaction transaction = session.BeginTransaction() )
			{
				session.Lock( uninitEntity, LockMode.None );

				Assert.IsFalse(
					NHibernateUtil.IsInitialized( uninitEntity ),
					"session.Lock() causes initialization of an unitialized entity." );

				Assert.AreEqual( LockMode.None, session.GetCurrentLockMode( uninitEntity ) );

				Assert.IsFalse(
					NHibernateUtil.IsInitialized( uninitEntity ),
					"session.GetCurrentLockMode() causes initialization of an unitialized entity." );


				session.Delete( "from System.Object" );
				transaction.Commit();
			}
		}
	}
}