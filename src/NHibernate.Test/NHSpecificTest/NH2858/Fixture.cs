using System;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Dialect;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2858
{
	public class Entity
	{
		public virtual int Id { get; set; }
		public virtual Guid TheGuid { get; set; }
	}
	
	public class Fixture : TestCaseMappingByCode
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect as MsSql2005Dialect != null;
		}

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Entity>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.Assigned));
				rc.Property(x => x.TheGuid);
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var entity = new Entity {Id = 1, TheGuid = Guid.Empty};
				session.Save(entity);
				session.Flush();
				transaction.Commit();
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
		public void GuidToStringShouldBeRetrievedCorrectlyInLinqProjection()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var guidToString = session.Query<Entity>().Select(x => x.TheGuid.ToString()).First();
				Assert.That(guidToString, Is.EqualTo(Guid.Empty.ToString()));
			}
		}
	}
}