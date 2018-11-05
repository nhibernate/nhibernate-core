using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1439
{
	public class Fixture : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<CompositeEntity>(rc =>
			{
				rc.ComposedId(
					c =>
					{
						c.Property(t => t.Id);
						c.Property(t => t.Name);
					});

				rc.Property(x => x.LazyProperty, x => x.Lazy(true));
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var e1 = new CompositeEntity { Id = 1, Name = "Bob", LazyProperty = "LazyProperty"};
				session.Save(e1);

				session.Flush();
				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.CreateQuery("delete from System.Object").ExecuteUpdate();

				session.Flush();
				transaction.Commit();
			}
		}

		[Test]
		public void LazyPropertyShouldNotBeNullified()
		{
			using (var session = OpenSession())
			using (var tran = session.BeginTransaction())
			{
				var result = (
					from e in session.Query<CompositeEntity>()
					where e.Name == "Bob"
					select e
				).ToList();
				session.Flush();
				tran.Commit();
				Assert.That(result, Has.Count.EqualTo(1));
			}
		}
	}
}
