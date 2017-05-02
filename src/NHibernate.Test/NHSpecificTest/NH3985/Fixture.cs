using NUnit.Framework;
using System;

namespace NHibernate.Test.NHSpecificTest.NH3985
{
	/// <summary>
	/// The test verifies that subsequent child sessions are not issued in already-disposed state.
	/// </summary>
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		public void GetChildSession_ShouldReturnNonDisposedInstance()
		{
			using (var rootSession = OpenSession())
			{
				using (var childSession1 = rootSession.SessionWithOptions().Connection().OpenSession())
				{
				}

				using (var childSession2 = rootSession.SessionWithOptions().Connection().OpenSession())
				{
					Assert.DoesNotThrow(() => { childSession2.Get<Process>(Guid.NewGuid()); });
				}
			}
		}

		[Test]
		public void GetChildSession_ShouldReturnNonClosedInstance()
		{
			using (var rootSession = OpenSession())
			{
				var childSession1 = rootSession.SessionWithOptions().Connection().OpenSession();
				childSession1.Close();

				using (var childSession2 = rootSession.SessionWithOptions().Connection().OpenSession())
				{
					Assert.DoesNotThrow(() => { childSession2.Get<Process>(Guid.NewGuid()); });
				}
			}
		}
	}
}
