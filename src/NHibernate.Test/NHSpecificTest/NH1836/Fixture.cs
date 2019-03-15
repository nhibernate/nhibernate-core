using System;
using System.Collections;
using System.Linq;
using NHibernate.Multi;
using NHibernate.Transform;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1836
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override bool AppliesTo(Engine.ISessionFactoryImplementor factory)
		{
			return factory.ConnectionProvider.Driver.SupportsMultipleQueries;
		}

		protected override void OnSetUp()
		{
			base.OnSetUp();

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Save(new Entity {Id = 1});
				t.Commit();
			}
		}

		[Test, Obsolete]
		public void AliasToBeanTransformerShouldApplyCorrectlyToMultiQuery()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				IMultiQuery multiQuery = s.CreateMultiQuery()
					.Add(s.CreateQuery("select entity.Id as EntityId from Entity entity")
						.SetResultTransformer(Transformers.AliasToBean(typeof(EntityDTO)))
					);

				IList results = null;
				Assert.That(() => results = multiQuery.List(), Throws.Nothing);
				var elementOfFirstResult = ((IList)results[0])[0];
				Assert.That(elementOfFirstResult, Is.TypeOf<EntityDTO>().And.Property("EntityId").EqualTo(1));
			}
		}

		[Test]
		public void AliasToBeanTransformerShouldApplyCorrectlyToQueryBatch()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var multiQuery = s
				                 .CreateQueryBatch()
				                 .Add<EntityDTO>(s
				                                 .CreateQuery("select entity.Id as EntityId from Entity entity")
				                                 .SetResultTransformer(Transformers.AliasToBean(typeof(EntityDTO))));

				Assert.That(multiQuery.Execute, Throws.Nothing);
				var results = multiQuery.GetResult<EntityDTO>(0);
				Assert.That(results.First(), Is.TypeOf<EntityDTO>().And.Property("EntityId").EqualTo(1));
				t.Commit();
			}
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.CreateQuery("delete from System.Object").ExecuteUpdate();
				t.Commit();
			}
		}
	}
}
