using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Criterion;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3594
{
	[TestFixture]
	public class Fixture : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Entity>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			{
				session.Save(new Entity { Name = "Name 1" });
				session.Save(new Entity { Name = "Name 2" });
				session.Save(new Entity { Name = "Name 3" });
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
		public void Test()
		{
			using (var session = OpenSession())
			{
				const int page = 2;
				const int rows = 2;

				var criteria = DetachedCriteria
					.For<Entity>("e")
					.SetProjection(Projections.Distinct(
						Projections.ProjectionList()
							.Add(Projections.Property("Id"))
							.Add(Projections.Property("Name"))
							))
							.SetFirstResult((page - 1) * rows)
					.SetMaxResults(rows)
					.SetResultTransformer(new Transform.AliasToBeanResultTransformer(typeof(Entity)));

				var query = criteria.GetExecutableCriteria(session);
				var result = query.List();

				Assert.That(result[0], Is.EqualTo("Name2"));
				Assert.That(result[1], Is.EqualTo("Name3"));

			}
		}
	}
}
