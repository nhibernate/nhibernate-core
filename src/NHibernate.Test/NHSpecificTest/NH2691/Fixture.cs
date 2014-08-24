using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;

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
				Executing.This(() => baseQuery.Count()).Should().NotThrow();
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
				Executing.This(() => baseQuery.LongCount()).Should().NotThrow();
				session.Transaction.Commit();
			}
		}
	}
}