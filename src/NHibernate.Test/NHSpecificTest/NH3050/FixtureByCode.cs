using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using System.Collections.Generic;
using System;
using NHibernate.Cfg;

namespace NHibernate.Test.NHSpecificTest.NH3050
{
	/// <summary>
	/// Fixture using 'by code' mappings
	/// </summary>
	/// <remarks>
	/// This fixture is identical to <see cref="Fixture" /> except the <see cref="Entity" /> mapping is performed 
	/// by code in the GetMappings method, and does not require the <c>Mappings.hbm.xml</c> file. Use this approach
	/// if you prefer.
	/// </remarks>
	[TestFixture]
	public class FixtureByCode : TestCaseMappingByCode
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
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var e1 = new Entity { Name = "Bob" };
				session.Save(e1);

				var e2 = new Entity { Name = "Sally" };
				session.Save(e2);

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

		protected override void Configure(Configuration configuration)
		{
			//firstly to make things simpler, we set the query plan cache size to 1
			configuration.Properties[Cfg.Environment.QueryPlanCacheMaxSize] = "1";
		}

		[Test]
		public void NH3050_Reproduction()
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				var names = new List<string>() { "Bob" };
				var query = from e in session.Query<Entity>()
							where names.Contains(e.Name)
							select e;

				//create a future, which will prepare a linq query plan and add it to the cache (NhLinqExpression)
				var future = query.ToFuture();

				//we need enough unique queries (different to our main query here) to fill the plan cache so that our previous plan is evicted
				//in this case we only need one as we have limited the cache size to 1
				(from e in session.Query<Entity>()
				 where e.Name == ""
				 select e).ToList();

				//garbage collection runs so that the query plan for our future which is a weak reference now in the plan cache is collected.
				GC.Collect();

				//execute future which creates an ExpandedQueryExpression and adds it to the plan cache (generates the same cache plan key as the NhLinqExpression)
				future.GetEnumerable().ToList();

				//execute original query again which will look for a NhLinqExpression in the plan cache but because it has already been evicted
				//and because the ExpandedQueryExpression generates the same cache key, the ExpandedQueryExpression is returned and 
				//an exception is thrown as it tries to cast to a NhLinqExpression.
				query.ToList();
			}
		}
	}
}
