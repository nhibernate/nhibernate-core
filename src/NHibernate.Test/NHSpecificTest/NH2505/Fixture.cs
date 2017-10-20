using System;
using System.Linq;
using System.Text.RegularExpressions;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2505
{
	public class MyClass
	{
		public virtual Guid Id { get; set; }
		public virtual bool Alive { get; set; }
		public virtual bool? MayBeAlive { get; set; }
		public virtual int Something { get; set; }
	}

	[TestFixture]
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
					session.Save(new MyClass { Alive = false, MayBeAlive = false });
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
		public void WhenQueryConstantEqualToMemberThenDoNotUseCaseStatement()
		{
			using (new Scenario(Sfi))
			{
				using (var session = OpenSession())
				{
					using (var sqls = new SqlLogSpy())
					{
						var list = session.Query<MyClass>().Where(x => x.Alive == false).ToList();
						Assert.That(list, Has.Count.EqualTo(2));
						Assert.That(caseClause.Matches(sqls.GetWholeLog()).Count, Is.EqualTo(0));
					}
					using (var sqls = new SqlLogSpy())
					{
						var list = session.Query<MyClass>().Where(x => true == x.Alive).ToList();
						Assert.That(list, Has.Count.EqualTo(1));
						Assert.That(caseClause.Matches(sqls.GetWholeLog()).Count, Is.EqualTo(0));
					}
				}
			}
		}

		[Test]
		public void WhenQueryConstantNotEqualToMemberThenDoNotUseCaseStatement()
		{
			using (new Scenario(Sfi))
			{
				using (var session = OpenSession())
				{
					using (var sqls = new SqlLogSpy())
					{
						var list = session.Query<MyClass>().Where(x => x.Alive != false).ToList();
						Assert.That(list, Has.Count.EqualTo(1));
						Assert.That(caseClause.Matches(sqls.GetWholeLog()).Count, Is.EqualTo(0));
					}
					using (var sqls = new SqlLogSpy())
					{
						var list = session.Query<MyClass>().Where(x => true != x.Alive).ToList();
						Assert.That(list, Has.Count.EqualTo(2));
						Assert.That(caseClause.Matches(sqls.GetWholeLog()).Count, Is.EqualTo(0));
					}
				}
			}
		}

		[Test]
		public void WhenQueryComplexEqualToComplexThentUseTheCaseStatementForBoth()
		{
			using (new Scenario(Sfi))
			{
				using (var session = OpenSession())
				{
					using (var sqls = new SqlLogSpy())
					{
						session.Query<MyClass>().Where(x => (5 > x.Something) == (x.Something < 10)).ToList();
						Assert.That(caseClause.Matches(sqls.GetWholeLog()).Count, Is.EqualTo(2));
					}
				}
			}
		}

		[Test]
		public void WhenQueryConstantEqualToNullableMemberThenDoNotUseCaseStatement()
		{
			using (new Scenario(Sfi))
			{
				using (var session = OpenSession())
				{
					using (var sqls = new SqlLogSpy())
					{
						var list = session.Query<MyClass>().Where(x => x.MayBeAlive == false).ToList();
						Assert.That(list, Has.Count.EqualTo(1));
						Assert.That(caseClause.Matches(sqls.GetWholeLog()).Count, Is.EqualTo(0));
					}
					using (var sqls = new SqlLogSpy())
					{
						var list = session.Query<MyClass>().Where(x => true == x.MayBeAlive).ToList();
						Assert.That(list, Has.Count.EqualTo(1));
						Assert.That(caseClause.Matches(sqls.GetWholeLog()).Count, Is.EqualTo(0));
					}
				}
			}
		}

		[Test]
		public void WhenQueryConstantEqualToNullableMemberValueThenDoNotUseCaseStatement()
		{
			using (new Scenario(Sfi))
			{
				using (var session = OpenSession())
				{
					using (var sqls = new SqlLogSpy())
					{
						var list = session.Query<MyClass>().Where(x => x.MayBeAlive.Value == false).ToList();
						Assert.That(list, Has.Count.EqualTo(1));
						Assert.That(caseClause.Matches(sqls.GetWholeLog()).Count, Is.EqualTo(0));
					}
					using (var sqls = new SqlLogSpy())
					{
						var list = session.Query<MyClass>().Where(x => true == x.MayBeAlive.Value).ToList();
						Assert.That(list, Has.Count.EqualTo(1));
						Assert.That(caseClause.Matches(sqls.GetWholeLog()).Count, Is.EqualTo(0));
					}
				}
			}
		}

		[Test]
		public void WhenQueryConstantNotEqualToNullableMemberThenDoNotUseCaseStatement()
		{
			using (new Scenario(Sfi))
			{
				using (var session = OpenSession())
				{
					using (var sqls = new SqlLogSpy())
					{
						var list = session.Query<MyClass>().Where(x => x.MayBeAlive != false).ToList();
						Assert.That(list, Has.Count.EqualTo(2));
						Assert.That(caseClause.Matches(sqls.GetWholeLog()).Count, Is.EqualTo(0));
					}
					using (var sqls = new SqlLogSpy())
					{
						var list = session.Query<MyClass>().Where(x => true != x.MayBeAlive).ToList();
						Assert.That(list, Has.Count.EqualTo(2));
						Assert.That(caseClause.Matches(sqls.GetWholeLog()).Count, Is.EqualTo(0));
					}
				}
			}
		}
	}
}
