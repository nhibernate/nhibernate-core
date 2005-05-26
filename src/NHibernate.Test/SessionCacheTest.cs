using System;
using System.Collections;
using NHibernate.DomainModel;
using NUnit.Framework;

namespace NHibernate.Test
{
	/// <summary>
	/// Summary description for SessionCacheTest.
	/// </summary>
	[TestFixture]
	public class SessionCacheTest : TestCase
	{
		protected override IList Mappings
		{
			get { return new string[] { "Simple.hbm.xml" }; }
		}

		[Test]
		public void MakeCollectionTransient()
		{
			ISession fixture = OpenSession();

			for( int i = 1; i < 6; i++ )
			{
				Simple s = new Simple( i );
				s.Address = "dummy collection address " + i;
				s.Date = DateTime.Now;
				s.Name = "dummy collection name " + i;
				s.Pay = i * 1279;
				fixture.Save( s, i );
			}
			
			fixture.Flush();

			IList list = fixture.CreateCriteria( typeof( Simple ) ).List();

			Assert.IsNotNull( list );
			Assert.IsTrue( list.Count == 5 );
			Assert.IsTrue( fixture.Contains( list[2] ) );

			fixture.Clear();

			Assert.IsTrue( list.Count == 5 );
			Assert.IsFalse( fixture.Contains( list[2] ) );

			fixture.Flush();
			
			Assert.IsTrue( list.Count == 5 );

			fixture.Delete( "from System.Object o" );
			fixture.Flush();
			fixture.Close();
		}

		[Test]
		[Ignore( "test like h2.1.x test, but not all h2.1.x features exist yet.")]
		public void LoadAfterNotExists()
		{
			ISession fixture = OpenSession();

			// First, prime the fixture session to think the entity does not exist
			try
			{
				fixture.Load( typeof( Simple ), -1 );
			}
			catch( ObjectNotFoundException )
			{
				// this is expected
			}

			// Next, lets create that entity under the covers
			ISession anotherSession = null;
			try
			{
				anotherSession = OpenSession();

				Simple oneSimple = new Simple( 1 );
				oneSimple.Name = "hidden entity";
				oneSimple.Address = "SessionCacheTest.LoadAfterNotExists";
				oneSimple.Date = DateTime.Now;
				oneSimple.Pay = 1000000f;
				
				anotherSession.Save( oneSimple, -1 );
				anotherSession.Flush();
			}
			finally
			{
				QuietlyClose( anotherSession );
			}

			// Verify that the original session is still unable to see the new entry...
			try
			{
				fixture.Load( typeof( Simple ), -1 );
			}
			catch( ObjectNotFoundException )
			{
			}

			// Now, lets clear the original session at which point it should be able to see the new entity
			fixture.Clear();

			string failedMessage = "Unable to load entity with id = -1.";
			try
			{
				Simple dummy = fixture.Load( typeof( Simple ), -1 ) as Simple;
				Assert.IsNotNull( dummy, failedMessage );
			}
			catch( ObjectNotFoundException )
			{
				Assert.Fail( failedMessage );
			}
			finally
			{
				QuietlyClose( fixture );
			}
		}

		private void QuietlyClose( ISession session )
		{
			if( session != null )
			{
				try
				{
					session.Delete( "from System.Object o" );
					session.Flush();
					session.Close();
				}
				catch
				{}
			}
		}
	}
}
