using System;
using System.IO;
using System.Reflection;
using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH3539
{
	[TestFixture]
	public class ImmutableComponentTest : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override string[] Mappings
		{
			get { return Array.Empty<string>(); }
		}

		protected override void Configure(Configuration configuration)
		{
			using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(
					   "NHibernate.Test.NHSpecificTest.GH3539.Person.hbm.xml"))
			{
				using (StreamReader reader = new StreamReader(stream))
				{
					string mapping = reader.ReadToEnd();

					configuration.AddXml(mapping);
					configuration.SetProperty(Cfg.Environment.GenerateStatistics, "true");
				}
			}
		}

		protected override void OnTearDown()
		{
			using (ISession s = Sfi.OpenSession())
			using (ITransaction t = s.BeginTransaction())
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
			using (ISession s = Sfi.OpenSession())
			using (ITransaction t = s.BeginTransaction())
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

			using (ISession s = Sfi.OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				var restored = s.Get<Person>(personId);

				var oldCardInfo = restored.CardInfo;

				Assert.That(oldCardInfo.GetCardsCopy().Contains("card1"));
				Assert.That(oldCardInfo.GetCardsCopy().Contains("card2"));
				Assert.That(oldCardInfo.GetHashCode(), Is.EqualTo(newCardInfo.GetHashCode()));

				var newCardInfo = new CardInfo("card1", "card2");

				Assert.That(oldCardInfo.Equals(newCardInfo), Is.True);

				restored.CardInfo = newCardInfo;

				// Expected behaviour:
				//
				// At this point there should be no DML statements because newCardInfo
				// is the same as the old one. But NHibernate will generate DELETE
				// followed by 2 INSERT into OldCards table. I'm not sure how to fail
				// an assertion when an unexpected DML is issued.

				t.Commit();
			}
		}
	}
}
