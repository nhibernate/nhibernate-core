using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2691
{
	public class Fixture: TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ConventionModelMapper();
			mapper.IsTablePerClass((type, declared) => false);
			mapper.IsTablePerClassHierarchy((type, declared) => true);
			var mappings = mapper.CompileMappingFor(new[] {typeof (Animal), typeof (Reptile), typeof (Mammal), typeof (Lizard), typeof (Dog), typeof (Cat)});
			return mappings;
		}

		[Test]
		public void WhenUseCountWithOrderThenCutTheOrder()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var baseQuery = from cat in session.Query<Cat>() orderby cat.BirthDate select cat;
				Assert.That(() => baseQuery.Count(), Throws.Nothing);
				session.Transaction.Commit();
			}
		}

		[Test]
		public void WhenUseLongCountWithOrderThenCutTheOrder()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var baseQuery = from cat in session.Query<Cat>() orderby cat.BirthDate select cat;
				Assert.That(() => baseQuery.LongCount(), Throws.Nothing);
				session.Transaction.Commit();
			}
		}
	}
}