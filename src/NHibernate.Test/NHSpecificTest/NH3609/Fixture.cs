using System.Linq;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.Transform;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3609
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var e1 = new Entity {Name = "Bob"};
				session.Save(e1);

				var e2 = new Entity {Name = "Sally"};
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

		[Test]
		public void CountWithConditionalDoesNotThrow()
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
			    MappingEntity mappingEntity = null;
			    Assert.DoesNotThrow(() =>
			        session.QueryOver<Entity>().SelectList(
			            builder =>
			                builder.Select(Projections.Count(Projections.Conditional(
			                    Restrictions.Eq(Projections.Property<Entity>(x => x.Name), "FOO"),
			                    Projections.Constant("", NHibernateUtil.String),
                                Projections.Constant(null, NHibernateUtil.String))).WithAlias(() => mappingEntity.Count))
			            )
			            .TransformUsing(Transformers.AliasToBean<MappingEntity>()).List<MappingEntity>()
			        );
			}
		}
	}
}