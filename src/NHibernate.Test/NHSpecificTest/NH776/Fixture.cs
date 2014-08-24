using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH776
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH776"; }
		}

		[Test]
		public void ProxiedOneToOneTest()
		{
			//Instantiate and setup associations (all needed to generate the error);
			A a = new A(1, "aaa");

			try
			{
				using (ISession session = sessions.OpenSession())
				{
					session.Save(a);

					session.Flush();
				}

				using (ISession session = sessions.OpenSession())
				{
					A loadedA = (A) session.Load(typeof(A), 1);
					Assert.IsNull(loadedA.NotProxied);
					Assert.IsNull(loadedA.Proxied,
					              "one-to-one to proxied types not handling missing associated classes correctly (as null)");
				}
			}
			finally
			{
				using (ISession session = OpenSession())
				{
					session.Delete(a);
					session.Flush();
				}
			}
		}
	}
}