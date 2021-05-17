using NHibernate.Test.NHSpecificTest;
using NUnit.Framework;

namespace NHibernate.Test.OneToOneType
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.CreateQuery("delete from Details").ExecuteUpdate();
				s.CreateQuery("delete from Owner").ExecuteUpdate();

				tx.Commit();
			}
		}

		[Test]
		public void OneToOnePersistedOnOwnerUpdate()
		{
			object ownerId;

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var owner = new Owner
				{
					Name = "Owner",
				};

				ownerId = s.Save(owner);

				tx.Commit();
			}

			using (var s = Sfi.OpenSession())
			using (var tx = s.BeginTransaction())
			{
				Owner owner = s.Load<Owner>(ownerId);

				owner.Details = new Details
				{
					Data = "Owner Details"
				};

				tx.Commit();
			}

			using (var s = Sfi.OpenSession())
			using (var tx = s.BeginTransaction())
			{
				Owner owner = s.Get<Owner>(ownerId);

				Assert.NotNull(owner.Details);
			}
		}

		[Test]
		public void OneToOnePersistedOnOwnerUpdateForSessionUpdate()
		{
			Owner owner;

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				owner = new Owner
				{
					Name = "Owner",
				};

				s.Save(owner);
				tx.Commit();
			}

			using (var s = Sfi.OpenSession())
			{
				owner = s.Get<Owner>(owner.Id);
			}

			using (var s = Sfi.OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.SaveOrUpdate(owner);
				owner.Details = new Details
				{
					Data = "Owner Details"
				};

				tx.Commit();
			}

			using (var s = Sfi.OpenSession())
			{
				owner = s.Get<Owner>(owner.Id);

				Assert.IsNotNull(owner.Details);
			}
		}
		
		[Test]
		public void CanInsertByStatelessSession()
		{
			object id;

			using (var s = Sfi.OpenStatelessSession())
			using (var tx = s.BeginTransaction())
			{
				var details = new Details
				{
					Owner = new Owner
					{
						Name = "Owner"
					},
					Data = "Owner Details"
				};

				id = s.Insert(details);

				tx.Commit();
			}

			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var owner = s.Get<Owner>(id);

				Assert.That(owner.Details, Is.Not.Null);
			}
		}
	}
}
