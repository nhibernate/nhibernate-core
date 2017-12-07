using System.Linq;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.Transform;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3787
{
	[TestFixture]
	public class TestFixture : BugTestCase
	{
		private const decimal _testRate = 12345.1234567890123M;

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return !TestDialect.HasBrokenDecimalType;
		}

		protected override void OnSetUp()
		{
			base.OnSetUp();

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var testEntity = new TestEntity
				{
					UsePreviousRate = true,
					PreviousRate = _testRate,
					Rate = 54321.1234567890123M
				};
				s.Save(testEntity);
				t.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.CreateQuery("delete from TestEntity").ExecuteUpdate();
				t.Commit();
			}
		}

		[Test]
		public void TestLinqQuery()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var queryResult = s
					.Query<TestEntity>()
					.Where(e => e.PreviousRate == _testRate)
					.ToList();

				Assert.That(queryResult.Count, Is.EqualTo(1));
				Assert.That(queryResult[0].PreviousRate, Is.EqualTo(_testRate));
				t.Commit();
			}
		}

		[Test]
		public void TestLinqProjection()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var queryResult = (from test in s.Query<TestEntity>()
				                   select new RateDto { Rate = test.UsePreviousRate ? test.PreviousRate : test.Rate }).ToList();

				// Check it has not been truncated to the default scale (10) of NHibernate.
				Assert.That(queryResult[0].Rate, Is.EqualTo(_testRate));
				t.Commit();
			}
		}

		[Test]
		public void TestLinqQueryOnExpression()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var queryResult = s
					.Query<TestEntity>()
					.Where(
						// Without MappedAs, the test fails for SQL Server because it would restrict its parameter to the dialect's default scale.
						e => (e.UsePreviousRate ? e.PreviousRate : e.Rate) == _testRate.MappedAs(TypeFactory.Basic("decimal(18,13)")))
					.ToList();

				Assert.That(queryResult.Count, Is.EqualTo(1));
				Assert.That(queryResult[0].PreviousRate, Is.EqualTo(_testRate));
				t.Commit();
			}
		}

		[Test]
		public void TestQueryOverProjection()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				TestEntity testEntity = null;

				var rateDto = new RateDto();
				//Generated sql
				//exec sp_executesql N'SELECT (case when this_.UsePreviousRate = @p0 then this_.PreviousRate else this_.Rate end) as y0_ FROM [TestEntity] this_',N'@p0 bit',@p0=1
				var query = s
					.QueryOver(() => testEntity)
					.Select(
						Projections
							.Alias(
								Projections.Conditional(
									Restrictions.Eq(Projections.Property(() => testEntity.UsePreviousRate), true),
									Projections.Property(() => testEntity.PreviousRate),
									Projections.Property(() => testEntity.Rate)),
								"Rate")
							.WithAlias(() => rateDto.Rate));

				var queryResult = query.TransformUsing(Transformers.AliasToBean<RateDto>()).List<RateDto>();

				Assert.That(queryResult[0].Rate, Is.EqualTo(_testRate));
				t.Commit();
			}
		}
	}
}
