using System;
using System.Collections;
using NHibernate.DomainModel;
using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.ExpressionTest
{
	[TestFixture]
	public class QueryByExampleTest : TestCase
	{
		protected override IList Mappings
		{
			get { return new string[] {"Componentizable.hbm.xml"}; }
		}

		protected override void OnSetUp()
		{
			InitData();
		}

		protected override void OnTearDown()
		{
			DeleteData();
		}

		[Test]
		public void TestSimpleQBE()
		{
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				Componentizable master = GetMaster("hibernate", null, "ope%");
				ICriteria crit = s.CreateCriteria(typeof(Componentizable));
				Example ex = Example.Create(master).EnableLike();
				crit.Add(ex);
				IList result = crit.List();
				Assert.IsNotNull(result);
				Assert.AreEqual(1, result.Count);
				t.Commit();
			}
		}

		[Test]
		public void TestJunctionNotExpressionQBE()
		{
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				Componentizable master = GetMaster("hibernate", null, "ope%");
				ICriteria crit = s.CreateCriteria(typeof(Componentizable));
				Example ex = Example.Create(master).EnableLike();


				crit.Add(Expression.Or(Expression.Not(ex), ex));

				IList result = crit.List();
				Assert.IsNotNull(result);
				//if ( !(dialect is HSQLDialect - h2.1 test

				Assert.AreEqual(2, result.Count, "expected 2 objects");
				t.Commit();
			}
		}

		[Test]
		public void TestExcludingQBE()
		{
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				Componentizable master = GetMaster("hibernate", null, "ope%");
				ICriteria crit = s.CreateCriteria(typeof(Componentizable));
				Example ex = Example.Create(master).EnableLike()
					.ExcludeProperty("Component.SubComponent");
				crit.Add(ex);
				IList result = crit.List();
				Assert.IsNotNull(result);
				Assert.AreEqual(3, result.Count);

				master = GetMaster("hibernate", "ORM tool", "fake stuff");
				crit = s.CreateCriteria(typeof(Componentizable));
				ex = Example.Create(master).EnableLike()
					.ExcludeProperty("Component.SubComponent.SubName1");
				crit.Add(ex);
				result = crit.List();
				Assert.IsNotNull(result);
				Assert.AreEqual(1, result.Count);
				t.Commit();
			}
		}

		private void InitData()
		{
			using (ISession s = OpenSession())
			{
				Componentizable master = GetMaster("hibernate", "ORM tool", "ORM tool1");
				s.Save(master);
				s.Flush();
			}

			using (ISession s = OpenSession())
			{
				Componentizable master = GetMaster("hibernate", "open source", "open source1");
				s.Save(master);
				s.Flush();
			}

			using (ISession s = OpenSession())
			{
				Componentizable master = GetMaster("hibernate", null, null);
				s.Save(master);
				s.Flush();
			}
		}

		private void DeleteData()
		{
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Delete("from Componentizable");
				t.Commit();
			}
		}

		private Componentizable GetMaster(String name, String subName, String subName1)
		{
			Componentizable master = new Componentizable();
			if (name != null)
			{
				Component masterComp = new Component();
				masterComp.Name = name;
				if (subName != null || subName1 != null)
				{
					SubComponent subComponent = new SubComponent();
					subComponent.SubName = subName;
					subComponent.SubName1 = subName1;
					masterComp.SubComponent = subComponent;
				}
				master.Component = masterComp;
			}
			return master;
		}
	}
}