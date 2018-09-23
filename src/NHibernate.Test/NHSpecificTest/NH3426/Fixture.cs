using System;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Dialect;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3426
{
	[TestFixture]
	public class Fixture : TestCaseMappingByCode
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is SQLiteDialect;
		}

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

		private static readonly string _id = "9FF2D288-56E6-F349-9CFC-48902132D65B";

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			{
				session.Save(new Entity { Id = Guid.Parse(_id), Name = "Name 1" });

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

				Assert.AreEqual(list[0].Id.ToUpper(), _id.ToUpper());
			}
		}
	}
}
