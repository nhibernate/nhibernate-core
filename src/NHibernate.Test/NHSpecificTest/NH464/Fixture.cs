using System;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH464
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH464"; }
		}

		// Could not reproduce the bug neither with nor without the reflection optimizer.
		[Test]
		public void WriteAndRead()
		{
			DateTime start = new DateTime(2000, 1, 1, 10, 30, 0);
			DateTime end = new DateTime(2000, 1, 2, 12, 30, 0);

			Container container = new Container();
			container.Id = 1;
			container.Component = new Component();
			container.Component.Dates.Add( new DateRange( start, end ) );

			using( ISession s = OpenSession() )
			using( ITransaction t = s.BeginTransaction() )
			{
				s.Save( container );
				t.Commit();
			}

			using( ISession s = OpenSession() )
			using( ITransaction t = s.BeginTransaction() )
			{
				container = (Container) s.Load( typeof(Container), container.Id );

				Assert.AreEqual( 1, container.Component.Dates.Count );

				DateRange dr = (DateRange) container.Component.Dates[0];
				Assert.AreEqual( start, dr.Start );
				Assert.AreEqual( end, dr.End );

				s.Delete( container );
				t.Commit();
			}
		}
	}
}
