using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.NHSpecificTest.NH3074
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		private const int id =123;

		protected override void OnSetUp()
		{
			using (var s = OpenSession())
			{
				using (var tx = s.BeginTransaction())
				{
					var cat = new Cat {Id = id, NumberOfLegs = 2, Weight = 100};
					s.Save(cat);
					tx.Commit();
				}
			}
		}

		[Test]
		public void CanSetLockMode()
		{
			using (var s = OpenSession())
			{
				using (s.BeginTransaction())
				{
					s.CreateQuery("select c from Animal c where c.Id=:id")
						.SetInt32("id", id)
						.SetLockMode("c", LockMode.Upgrade)
						.List<Cat>()
						.Should().Not.Be.Empty();
				}
			}
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			{
				using (var tx = s.BeginTransaction())
				{
					s.Delete(s.Get<Cat>(id));
					tx.Commit();
				}
			}
		}
	}
}