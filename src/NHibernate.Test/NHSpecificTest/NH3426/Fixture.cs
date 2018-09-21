using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			{
				session.Save(new Entity { Id = Guid.NewGuid(), Name = "Name 1" });
				session.Save(new Entity { Id = Guid.NewGuid(), Name = "Name 2" });
				session.Save(new Entity { Id = Guid.NewGuid(), Name = "Name 3" });

				session.Flush();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				session.Flush();
				transaction.Commit();
			}
		}

		[Test]
		public void SelectAll()
		{
			using (var session = OpenSession())
			{
				var list = session.Query<Entity>()
					.Select(x => new { Id = x.Id.ToString() })
					.ToList();

				Assert.Equals(3, list.Count);
				foreach (var x in list)
				{
					Assert.AreEqual(36, x.Id.Length);
				}

			}
		}
	}
}
