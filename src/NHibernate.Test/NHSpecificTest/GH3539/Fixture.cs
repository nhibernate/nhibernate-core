using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH3539
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(Cfg.Environment.GenerateStatistics, "true");
		}

		protected override void OnTearDown()
		{
			using (var s = Sfi.OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Delete("from Person");
				t.Commit();
			}

			base.OnTearDown();
		}

		[Test]
		public void TestComponent()
		{
			int personId;
			using (var s = Sfi.OpenSession())
			using (var t = s.BeginTransaction())
			{
				var person = new Person(age: 20)
				{
					CardInfo = new("card1", "card2")
				};
				s.Save(person);
				t.Commit();
				s.Flush();
				personId = person.Id;
			}

			using (var s = Sfi.OpenSession())
			using (var t = s.BeginTransaction())
			{
				var restored = s.Get<Person>(personId);

				var oldCardInfo = restored.CardInfo;

				Assert.That(oldCardInfo.GetCardsCopy().Contains("card1"));
				Assert.That(oldCardInfo.GetCardsCopy().Contains("card2"));

				var newCardInfo = new CardInfo("card1", "card2");

				Assert.That(oldCardInfo.GetHashCode(), Is.EqualTo(newCardInfo.GetHashCode()));
				Assert.That(oldCardInfo.Equals(newCardInfo));

				restored.CardInfo = newCardInfo;

				// Expected behaviour:
				//
				// At this point there should be no DML statements because newCardInfo
				// is the same as the old one. But NHibernate will generate DELETE
				// followed by 2 INSERT into OldCards table.

				using (var x = new SqlLogSpy())
				{
					t.Commit();
					Assert.That(x.GetWholeLog(), Is.Empty);
				}
			}
		}
	}
}
