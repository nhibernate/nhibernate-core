using System;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Dialect;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2858
{
	public class Fixture : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<DomainClass>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.Assigned));
				rc.Property(x => x.TheGuid);
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var entity = new DomainClass {Id = 1, TheGuid = Guid.Empty};
				session.Save(entity);
				session.Flush();
				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");
				session.Flush();
				transaction.Commit();
			}
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect as MsSql2008Dialect != null;
		}

		[Test]
		public void GuidToStringShouldBeRetrievedCorrectlyInLinqProjection()
		{
			using (ISession session = OpenSession())
			{
				var guidToString = session.Query<DomainClass>().Select(x => x.TheGuid.ToString()).First();
				Assert.That(guidToString, Is.EqualTo(Guid.Empty.ToString()));
			}
		}
	}
}