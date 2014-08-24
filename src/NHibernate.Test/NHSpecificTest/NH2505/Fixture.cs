using System;
using System.Linq;
using System.Text.RegularExpressions;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.NHSpecificTest.NH2505
{
	public class MyClass
	{
		public virtual Guid Id { get; set; }
		public virtual bool Alive { get; set; }
		public virtual bool? MayBeAlive { get; set; }
		public virtual int Something { get; set; }
	}
	public class Fixture: TestCaseMappingByCode
	{
		private Regex caseClause = new Regex("case",RegexOptions.IgnoreCase);
		protected override HbmMapping GetMappings()
		{
			var mapper = new ConventionModelMapper();
			mapper.BeforeMapClass += (mi, t, x) => x.Id(map=> map.Generator(Generators.Guid));
			return mapper.CompileMappingFor(new[] { typeof(MyClass) });
		}

		private class Scenario: IDisposable
		{
			private readonly ISessionFactory factory;

			public Scenario(ISessionFactory factory)
			{
				this.factory = factory;
				using (var session= factory.OpenSession())
				{
					session.Save(new MyClass { Alive = true });
					session.Save(new MyClass { Alive = false, MayBeAlive = true });
					session.Flush();
				}
			}

			public void Dispose()
			{
				using (var session = factory.OpenSession())
				{
					session.CreateQuery("delete from MyClass").ExecuteUpdate();
					session.Flush();
				}
			}
		}

		[Test]
		public void WhenQueryConstantEqualToMemberThenDoesNotUseTheCaseConstructor()
		{
			using (new Scenario(Sfi))
			{
				using (var session = OpenSession())
				{
					using (var sqls = new SqlLogSpy())
					{
						session.Query<MyClass>().Where(x => x.Alive == false).Should().Have.Count.EqualTo(1);
						caseClause.Matches(sqls.GetWholeLog()).Count.Should().Be(0);
					}
					using (var sqls = new SqlLogSpy())
					{
						session.Query<MyClass>().Where(x => true == x.Alive).Should().Have.Count.EqualTo(1);
						caseClause.Matches(sqls.GetWholeLog()).Count.Should().Be(0);
					}
				}
			}
		}

		[Test]
		public void WhenQueryConstantNotEqualToMemberThenDoesNotUseTheCaseConstructor()
		{
			using (new Scenario(Sfi))
			{
				using (var session = OpenSession())
				{
					using (var sqls = new SqlLogSpy())
					{
						session.Query<MyClass>().Where(x => x.Alive != false).Should().Have.Count.EqualTo(1);
						caseClause.Matches(sqls.GetWholeLog()).Count.Should().Be(0);
					}
					using (var sqls = new SqlLogSpy())
					{
						session.Query<MyClass>().Where(x => true != x.Alive).Should().Have.Count.EqualTo(1);
						caseClause.Matches(sqls.GetWholeLog()).Count.Should().Be(0);
					}
				}
			}
		}

		[Test]
		public void WhenQueryComplexEqualToComplexThentUseTheCaseConstructorForBoth()
		{
			using (new Scenario(Sfi))
			{
				using (var session = OpenSession())
				{
					using (var sqls = new SqlLogSpy())
					{
						session.Query<MyClass>().Where(x => (5 > x.Something) == (x.Something < 10)).ToList();
						caseClause.Matches(sqls.GetWholeLog()).Count.Should().Be(2);
					}
				}
			}
		}

		[Test]
		public void WhenQueryConstantEqualToNullableMemberThenUseTheCaseConstructorForMember()
		{
			using (new Scenario(Sfi))
			{
				using (var session = OpenSession())
				{
					using (var sqls = new SqlLogSpy())
					{
						session.Query<MyClass>().Where(x => x.MayBeAlive == false).Should().Have.Count.EqualTo(1);
						caseClause.Matches(sqls.GetWholeLog()).Count.Should().Be(1);
					}
					using (var sqls = new SqlLogSpy())
					{
						session.Query<MyClass>().Where(x => true == x.MayBeAlive).Should().Have.Count.EqualTo(1);
						caseClause.Matches(sqls.GetWholeLog()).Count.Should().Be(1);
					}
				}
			}
		}

		[Test]
		public void WhenQueryConstantEqualToNullableMemberValueThenDoesNotUseTheCaseConstructorForMember()
		{
			using (new Scenario(Sfi))
			{
				using (var session = OpenSession())
				{
					using (var sqls = new SqlLogSpy())
					{
						session.Query<MyClass>().Where(x => x.MayBeAlive.Value == false).ToList();
						caseClause.Matches(sqls.GetWholeLog()).Count.Should().Be(1);
					}
					using (var sqls = new SqlLogSpy())
					{
						session.Query<MyClass>().Where(x => true == x.MayBeAlive.Value).ToList();
						caseClause.Matches(sqls.GetWholeLog()).Count.Should().Be(1);
					}
				}
			}
		}

		[Test]
		public void WhenQueryConstantNotEqualToNullableMemberThenUseTheCaseConstructorForMember()
		{
			using (new Scenario(Sfi))
			{
				using (var session = OpenSession())
				{
					using (var sqls = new SqlLogSpy())
					{
						session.Query<MyClass>().Where(x => x.MayBeAlive != false).Should().Have.Count.EqualTo(1);
						caseClause.Matches(sqls.GetWholeLog()).Count.Should().Be(1);
					}
					using (var sqls = new SqlLogSpy())
					{
						session.Query<MyClass>().Where(x => true != x.MayBeAlive).Should().Have.Count.EqualTo(1);
						caseClause.Matches(sqls.GetWholeLog()).Count.Should().Be(1);
					}
				}
			}
		}
	}
}