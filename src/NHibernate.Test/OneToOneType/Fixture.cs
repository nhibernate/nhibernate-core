using NHibernate.Test.NHSpecificTest;
using NUnit.Framework;

namespace NHibernate.Test.OneToOneType
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnTearDown()
		{
			using (var s = Sfi.OpenSession())
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

			using (var s = Sfi.OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var owner = new Owner()
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

				owner.Details = new Details()
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
	}
}
