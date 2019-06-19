using System;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3426
{
	[TestFixture]
	public class Fixture : TestCaseMappingByCode
	{

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Entity>(rc =>
			{
				rc.Id(x => x.Id);
				rc.Property(x => x.Name);
			});
			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		private const string id = "9FF2D288-56E6-F349-9CFC-48902132D65B";

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			{
				session.Save(new Entity { Id = Guid.Parse(id), Name = "Name 1" });

				session.Flush();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					session.Delete("from System.Object");

					session.Flush();
					transaction.Commit();
				}
			}
		}

		[Test]
		public void SelectGuidToString()
		{
			using (var session = OpenSession())
			{
				var list = session.Query<Entity>()
					.Select(x => new { Id = x.Id.ToString() })
					.ToList();

				Assert.AreEqual(id.ToUpper(), list[0].Id.ToUpper());
			}
		}
		
		[Test]
		public void WhereGuidToString()
		{
			using (var session = OpenSession())
			{
				var list = session.Query<Entity>()
				                  .Where(x => x.Id.ToString().ToUpper() == id)
				                  .ToList();

				Assert.That(list, Has.Count.EqualTo(1));
			}
		}

		[Test]
		public void CompareStringColumnWithGuidToString()
		{
			using (var session = OpenSession())
			{
				var list = session.Query<Entity>()
				                  .Where(x => x.Id.ToString() == x.Name)
				                  .ToList();

				Assert.That(list, Has.Count.EqualTo(0));
			}
		}

		[Test]
		public void CompareStringColumnWithNullableGuidToString()
		{
			using (var session = OpenSession())
			{
				var list = session.Query<Entity>()
				                  .Where(x => ((Guid?) x.Id).ToString() == x.Id.ToString())
				                  .ToList();

				Assert.That(list, Has.Count.EqualTo(1));
			}
		}
	}
}
