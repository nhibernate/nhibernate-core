using System;
using System.Collections;
using NHibernate.DomainModel;
using NHibernate.Expression;
using NUnit.Framework;


namespace NHibernate.Test.ExpressionTest
{
	/// <summary>
	/// Summary description for QueryByExampleTest.
	/// </summary>
	[TestFixture]
	public class QueryByExampleTest : TestCase
	{
		public QueryByExampleTest()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		[SetUp]
		public void SetUp() 
		{
			ExportSchema( new string[] { "Componentizable.hbm.xml"});
			initData();
		}
		
		
		[Test]
		public void TestSimpleQBE()
		{
			ISession s = sessions.OpenSession();
			ITransaction t = s.BeginTransaction();
			Componentizable master = GetMaster("hibernate", null, "ope%");
			ICriteria crit = s.CreateCriteria(typeof(Componentizable));
			Example ex = Example.create(master).EnableLike();
			crit.Add(ex);
			IList result = crit.List();
			Assertion.AssertNotNull(result);
			Assertion.AssertEquals(1, result.Count);
			t.Commit();
			s.Close();
		}

		[Test]
		[Ignore("Test Fails with Exception - do to with Criteria expression/parameter handling")]
		public void TestJunctionNotExpressionQBE()
		{
			ISession s = sessions.OpenSession();
			ITransaction t = s.BeginTransaction();
			Componentizable master = GetMaster("hibernate", null, "ope%");
			ICriteria crit = s.CreateCriteria(typeof(Componentizable));
			Example ex = Example.create(master).EnableLike();

			
			crit.Add(Expression.Expression.Or(Expression.Expression.Not(ex), ex));

			IList result = crit.List();
			Assertion.AssertNotNull(result);
//			if (!(GetDialect()
//			instanceof HSQLDialect) )
//			assertEquals(2, result.size());
			t.Commit();
			s.Close();
		}

		[Test]
		public void TestExcludingQBE()
		{
			ISession s = sessions.OpenSession();
			ITransaction t = s.BeginTransaction();
			Componentizable master = GetMaster("hibernate", null, "ope%");
			ICriteria crit = s.CreateCriteria(typeof(Componentizable));
			Example ex = Example.create(master).EnableLike()
				.ExcludeProperty("Component.SubComponent");
			crit.Add(ex);
			IList result = crit.List();
			Assertion.AssertNotNull(result);
			Assertion.AssertEquals(3, result.Count);

			master = GetMaster("hibernate", "ORM tool", "fake stuff");
			crit = s.CreateCriteria(typeof(Componentizable));
			ex = Example.create(master).EnableLike()
				.ExcludeProperty("Component.SubComponent.SubName1");
			crit.Add(ex);
			result = crit.List();
			Assertion.AssertNotNull(result);
			Assertion.AssertEquals(1, result.Count);
			t.Commit();
			s.Close();
		}

		private void initData()
		{
			ISession s = sessions.OpenSession();
			Componentizable master = GetMaster("hibernate", "ORM tool", "ORM tool1");
			s.Save(master);
			s.Close();
			s = sessions.OpenSession();
			master = GetMaster("hibernate", "open source", "open source1");
			s.Save(master);
			s.Close();
			s = sessions.OpenSession();
			master = GetMaster("hibernate", null, null);
			s.Save(master);
			s.Close();
		}

		private void deleteData()
		{
			ISession s = sessions.OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Delete("from Componentizable");
			t.Commit();
			s.Close();
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