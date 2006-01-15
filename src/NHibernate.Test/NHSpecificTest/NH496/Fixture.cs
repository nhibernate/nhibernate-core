using System;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH496
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH496"; }
		}

		[Test]
		[ExpectedException( typeof( MappingException ) )]
		public void CRUD()
		{
			WronglyMappedClass obj = new WronglyMappedClass();
			obj.SomeInt = 10;

			using( ISession s = OpenSession() )
			using( ITransaction t = s.BeginTransaction() )
			{
				s.Save( obj );
				t.Commit();
			}

			try
			{
				using( ISession s = OpenSession() )
				using( ITransaction t = s.BeginTransaction() )
				{
					obj = ( WronglyMappedClass ) s.Get( typeof( WronglyMappedClass ), obj.Id );
					Assert.AreEqual( 10, obj.SomeInt );
					t.Commit();
				}
			}
			finally
			{
				using( ISession s = OpenSession() )
				using( ITransaction t = s.BeginTransaction() )
				{
					s.Delete( obj );
					t.Commit();
				}
			}
		}
	}
}
