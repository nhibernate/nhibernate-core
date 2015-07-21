using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2390
{
	[Ignore("Not fixed yet")]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			base.OnSetUp();
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				var class1 = new Class1();
				s.Save(class1);
				t.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Delete("from Class1");
				t.Commit();
			}
			base.OnTearDown();
		}

		[Test]
		public void Test()
		{
			var rowsUpdated = 0;
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				rowsUpdated = s.CreateQuery("UPDATE VERSIONED Class1 c SET c.Property1 = :value1, c.Property2 = :value2, c.Property3 = :value3, c.Property4 = :value4, c.Property5 = :value5")
									.SetParameter("value1", 1)
									.SetParameter("value2", 2)
									.SetParameter("value3", 3)
									.SetParameter("value4", 4)
									.SetParameter("value5", 5)
						.ExecuteUpdate();
				t.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				var class1 = (Class1)(s.CreateQuery("FROM Class1").UniqueResult());

				Assert.That(rowsUpdated, Is.EqualTo(1), "UPDATE did not alter the expected number of rows");
				Assert.That(class1.Property1, Is.EqualTo(1), "UPDATE did not alter Property1");
				Assert.That(class1.Property2, Is.EqualTo(2), "UPDATE did not alter Property2");
				Assert.That(class1.Property3, Is.EqualTo(3), "UPDATE did not alter Property3");
				Assert.That(class1.Property4, Is.EqualTo(4), "UPDATE did not alter Property4");
				Assert.That(class1.Property5, Is.EqualTo(5), "UPDATE did not alter Property5");
				Assert.That(class1.Version, Is.EqualTo(2), "UPDATE did not increment the version");
			}
		}
	}
}
