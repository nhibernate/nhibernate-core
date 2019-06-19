using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2714
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		private const int ExtraId = 500;

		protected override void OnTearDown()
		{
			using (ISession s = OpenSession())
			{
				s.Delete($"from {nameof(Item)}");
				s.Flush();
				s.Delete($"from {nameof(Information)}");
				s.Flush();
			}
		}

		[Test]
		public void PropertyRefUsesOtherColumns()
		{
			var information = new Information {Name = "First", ExtraId = ExtraId};

			var item = new Item {Id = 1, Name = information.Name, ExtraId = information.ExtraId};

			using (ISession session = OpenSession())
			{
				session.Save(information);
				session.Save(item);
				session.Flush();
			}

			using (ISession session = OpenSession())
			{
				var otherInformation = session.Get<Information>(information.Id);
				Assert.That(otherInformation.Items.Count, Is.EqualTo(1));
			}
		}

		[Test]
		public void ChildKeyPropertiesOfParentAreRetrieved()
		{
			var information = new Information {Name = "First", ExtraId = ExtraId};

			var item = new Item {Id = 1, Name = information.Name, ExtraId = information.ExtraId};

			using (ISession session = OpenSession())
			{
				session.Save(information);
				session.Save(item);
				session.Flush();
			}

			using (ISession session = OpenSession())
			{
				var otherInformation = session.Get<Information>(information.Id);

				Assert.That(otherInformation.Name, Is.EqualTo(information.Name));
				Assert.That(otherInformation.ExtraId, Is.EqualTo(information.ExtraId));
			}
		}
	}
}
