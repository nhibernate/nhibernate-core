using System;
using System.Linq;
using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.Criteria
{
	[TestFixture]
	public class ConditionalProjectionTest : TestCase
	{
		protected override string MappingsAssembly => "NHibernate.Test";

		protected override string[] Mappings => new [] {"Criteria.Enrolment.hbm.xml"};

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Save(new Student {StudentNumber = 6L, Name = "testa"});
				session.Save(new Student {StudentNumber = 5L, Name = "testz"});
				session.Save(new Student {StudentNumber = 4L, Name = "test1"});
				session.Save(new Student {StudentNumber = 3L, Name = "test2"});
				session.Save(new Student {StudentNumber = 2L, Name = "test998"});
				session.Save(new Student {StudentNumber = 1L, Name = "test999"});
				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = Sfi.OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.CreateQuery("delete from System.Object").ExecuteUpdate();
				transaction.Commit();
			}
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return !TestDialect.HasBrokenTypeInferenceOnSelectedParameters;
		}

		[Test]
		public void UsingMultiConditionals()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				// when Name = "testa" then 1 ...
				var orderOfNames = new Tuple<string, string>[]
				{
					System.Tuple.Create("test1", "1"),
					System.Tuple.Create("testz", "2"),
					System.Tuple.Create("test2", "3"),
					System.Tuple.Create("testa", "4")
				};

				var criterionProjections =
					orderOfNames
						.Select(
							x => new ConditionalCriterionProjectionPair(
								Restrictions.Eq(nameof(Student.Name), x.Item1),
								Projections.Constant(x.Item2)))
						.ToArray();

				// ... else 99
				var elseProjection = Projections.Constant("99");

				var conditionalsProjection = Projections.Conditionals(criterionProjections, elseProjection);

				var order = Order.Asc(conditionalsProjection);

				var criteria = session.CreateCriteria(typeof(Student)).AddOrder(order);

				var actuals = criteria.List<Student>();

				Assert.That(actuals.Count, Is.GreaterThanOrEqualTo(orderOfNames.Length));
				for (var i = 0; i < orderOfNames.Length; i++)
				{
					var expected = orderOfNames[i];
					var actual = actuals[i];

					Assert.That(actual.Name, Is.EqualTo(expected.Item1));
				}
			}
		}
	}
}
