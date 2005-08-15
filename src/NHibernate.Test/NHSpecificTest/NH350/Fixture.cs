using System;
using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH350
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH350"; }
		}

		// We pass an Int32 id to Load and expect an exception, since the class
		// uses Int64 ids.
		[Test, ExpectedException( typeof( ArgumentException ) )]
		public void Loading()
		{
			object parentId;
			using( ISession session = OpenSession() )
			{
				SecurityDomain parent = new SecurityDomain();
				parent.Name = "Name";
				parent.ChildDomains.Add(new SecurityDomain());

				parentId = session.Save( parent );
				session.Flush();
			}

			try
			{
				using( ISession session = OpenSession() )
				{
					SecurityDomain aDomain = (SecurityDomain)session.Load( typeof(SecurityDomain), 1 );
					Assert.AreEqual( "Name", aDomain.Name );
					Assert.IsTrue( aDomain.ChildDomains.Count > 0 );
				}
			}
			finally
			{
				using( ISession session = OpenSession() )
				{
					session.Delete( "from SecurityDomain" );
					session.Flush();
				}
			}
		}
	}
}
