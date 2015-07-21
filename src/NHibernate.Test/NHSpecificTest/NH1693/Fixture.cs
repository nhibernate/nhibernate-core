using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1693
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			{
				session.Delete("from Invoice");
				session.Flush();
			}
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				session.Save(new Invoice { Mode = "a", Num = 1, Category = 10 });
				session.Save(new Invoice { Mode = "a", Num = 2, Category = 10 });
				session.Save(new Invoice { Mode = "a", Num = 3, Category = 20 });
				session.Save(new Invoice { Mode = "a", Num = 4, Category = 10 });
				session.Save(new Invoice { Mode = "b", Num = 2, Category = 10 });
				session.Save(new Invoice { Mode = "b", Num = 3, Category = 10 });
				session.Save(new Invoice { Mode = "b", Num = 5, Category = 10 });

				tx.Commit();
			}
		}

		[Test]
		public void without_filter()
		{
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				var q1 =
					"from Invoice i where i.Mode='a' and i.Category=:cat and not exists (from Invoice i2 where i2.Mode='a' and i2.Category=:cat and i2.Num=i.Num+1)";
				var list = session.CreateQuery(q1)
					.SetParameter("cat", 10)
					.List<Invoice>();
				Assert.That(list.Count, Is.EqualTo(2));
				Assert.That(list[0].Num == 2 && list[0].Mode == "a");
				Assert.That(list[1].Num == 4 && list[1].Mode == "a");

				tx.Commit();
			}
		}

		[Test]
		public void with_filter()
		{
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				session.EnableFilter("modeFilter").SetParameter("currentMode", "a");

				var q1 =
					"from Invoice i where i.Category=:cat and not exists (from Invoice i2 where i2.Category=:cat and i2.Num=i.Num+1)";
				var list = session.CreateQuery(q1)
					.SetParameter("cat", 10)
					.List<Invoice>();
				Assert.That(list.Count, Is.EqualTo(2));
				Assert.That(list[0].Num == 2 && list[0].Mode == "a");
				Assert.That(list[1].Num == 4 && list[1].Mode == "a");

				tx.Commit();
			}
		}

	}
}
