using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.EmbIdBidirectional
{
	public class Fixture : BugTestCase
	{
		[Test]
		[KnownBug("Regression - worked in 5.1.x. Now also Teardown fails because entity cannot be loaded....")]
		public void CanReadBidirectionalEntitiesWithEmbeddedId()
		{
			const string ingData = "data_ing";
			var edId = new EmbId {X = 1, Y = 2};
			var ingId = new EmbId { X = 5, Y = 6 };
			var ed = new BiEmbIdRefEdEntity {Id = edId, Data = "data_ed"};
			var ing = new BiEmbIdRefIngEntity {Id = ingId, Data = ingData, Reference = ed};
			using (var s = OpenSession())
			{
				using (var tx = s.BeginTransaction())
				{
					s.Save(ed);
					s.Save(ing);
					tx.Commit();
				}
			}

			using (var s = OpenSession())
			{
				var ingEntity = s.Get<BiEmbIdRefIngEntity>(ingId);
				Assert.That(ingEntity.Data, Is.EqualTo(ingData));
			}
		}
		
		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			{
				using (var tx = session.BeginTransaction())
				{
					session.Delete("from BiEmbIdRefIngEntity");
					session.Delete("from BiEmbIdRefEdEntity");
					tx.Commit();
				}
			}
		}
	}
}
