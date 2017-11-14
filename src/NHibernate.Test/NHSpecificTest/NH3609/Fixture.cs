﻿using NHibernate.Criterion;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Transform;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3609
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
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
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				session.Flush();
				transaction.Commit();
			}
		}

		[Test]
		public void AvgWithConditionalDoesNotThrow()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				MappingEntity mappingEntity = null;
				Assert.DoesNotThrow(
					() =>
						session.QueryOver<Entity>().SelectList(
							builder =>
								builder.Select(
									Projections.Avg(
										Projections.Conditional(
											Restrictions.Eq(Projections.Property<Entity>(x => x.Name), "FOO"),
											Projections.Constant("", NHibernateUtil.String),
											Projections.Constant(null, NHibernateUtil.String))).WithAlias(() => mappingEntity.Count))
						).TransformUsing(Transformers.AliasToBean<MappingEntity>()).List<MappingEntity>()
					);
			}
		}

		[Test]
		public void CountWithConditionalDoesNotThrow()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				MappingEntity mappingEntity = null;
				Assert.DoesNotThrow(
					() =>
						session.QueryOver<Entity>().SelectList(
							builder =>
								builder.Select(
									Projections.Count(
										Projections.Conditional(
											Restrictions.Eq(Projections.Property<Entity>(x => x.Name), "FOO"),
											Projections.Constant("", NHibernateUtil.String),
											Projections.Constant(null, NHibernateUtil.String))).WithAlias(() => mappingEntity.Count))
						).TransformUsing(Transformers.AliasToBean<MappingEntity>()).List<MappingEntity>()
					);
			}
		}

		[Test]
		public void GroupByClauseHasParameterSet()
		{
			if (!TestDialect.SupportsComplexExpressionInGroupBy)
				Assert.Ignore(Dialect.GetType().Name + " does not support complex group by expressions");
			// When not using a named prefix, the driver use positional parameters, causing parameterized
			// expression used in group by and select to be not be considered as the same expression.
			if (!((DriverBase)Sfi.ConnectionProvider.Driver).UseNamedPrefixInParameter)
				Assert.Ignore("Cannot group by and select a parameterized expression with positional parameters");

			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				MappingEntity mappingEntity = null;
				Assert.DoesNotThrow(
					() =>
						session.QueryOver<Entity>().SelectList(
							builder =>
								builder.Select(
									Projections.GroupProperty(
										Projections.Conditional(
											Restrictions.Eq(Projections.Property<Entity>(x => x.Name), ""),
											Projections.Constant(1),
											Projections.Constant(2)))
											   .WithAlias(() => mappingEntity.Count))
						).TransformUsing(Transformers.AliasToBean<MappingEntity>()).List<MappingEntity>()
					);
			}
		}
	}
}
