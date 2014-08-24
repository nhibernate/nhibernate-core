using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3401
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				session.Flush();
				transaction.Commit();
			}
		}

		[Test(Description = "NH-3401")]
		[Ignore("Test not implemented - this can be used a base for a proper test case for NH-3401.")]
		public void YesNoParameterLengthShouldBe1()
		{
			// MISSING PART: Asserts for the SQL parameter sizes in the generated commands.

			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var e1 = new Entity {Name = "Bob"};
				session.Save(e1);

				var e2 = new Entity {Name = "Sally", YesNo = true};
				session.Save(e2);

				session.Flush();
				transaction.Commit();
			}


			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = from e in session.Query<Entity>()
					where e.YesNo
					select e;

				Assert.AreEqual(1, result.ToList().Count);
			}
		}
	}
}