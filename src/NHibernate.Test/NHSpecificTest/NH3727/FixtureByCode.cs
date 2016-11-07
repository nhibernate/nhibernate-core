using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Criterion;
using NHibernate.Mapping.ByCode;
using NHibernate.Transform;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3727
{
	public class ByCodeFixture : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Entity>(rc =>
			{
				rc.Id(x => x.Id);
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		[Test]
		public void QueryOverWithSubqueryProjectionCanBeExecutedMoreThanOnce()
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				const int parameter1 = 111;

				var countSubquery = QueryOver.Of<Entity>()
						.Where(x => x.Id == parameter1) //any condition which makes output SQL has parameter
						.Select(Projections.RowCount())
						;

				var originalQueryOver = session.QueryOver<Entity>()
											   .SelectList(l => l
																	.Select(x => x.Id)
																	.SelectSubQuery(countSubquery)
												)
											   .TransformUsing(Transformers.ToList);

				var objects = originalQueryOver.List<object>();

				Assert.DoesNotThrow(() => originalQueryOver.List<object>(), "Second try to execute QueryOver thrown exception.");
			}
		}

		[Test]
		public void ClonedQueryOverExecutionMakesOriginalQueryOverNotWorking()
		{
			// Projections are copied by clone operation. 
			// SubqueryProjection use SubqueryExpression which holds CriteriaQueryTranslator (class SubqueryExpression { private CriteriaQueryTranslator innerQuery; })
			// So given CriteriaQueryTranslator is used twice. 
			// Since CriteriaQueryTranslator has CollectedParameters collection, second execution of the Criteria does not fit SqlCommand parameters.

			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				const int parameter1 = 111;

				var countSubquery = QueryOver.Of<Entity>()
						.Where(x => x.Id == parameter1) //any condition which makes output SQL has parameter
						.Select(Projections.RowCount())
						;

				var originalQueryOver = session.QueryOver<Entity>()
											   //.Where(x => x.Id == parameter2)
											   .SelectList(l => l
																	.Select(x => x.Id)
																	.SelectSubQuery(countSubquery)
												)
											   .TransformUsing(Transformers.ToList);

				var clonedQueryOver = originalQueryOver.Clone();
				clonedQueryOver.List<object>();

				Assert.DoesNotThrow(() => originalQueryOver.List<object>(), "Cloned QueryOver execution caused source QueryOver throw exception when executed.");
			}
		}
	}
}