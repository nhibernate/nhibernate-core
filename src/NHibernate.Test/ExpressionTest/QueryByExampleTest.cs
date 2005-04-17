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
		[SetUp]
		public void SetUp() 
		{
			ExportSchema( new string[] { "Componentizable.hbm.xml"});
			InitData();
		}
		
		
		[Test]
		public void TestSimpleQBE()
		{
			using( ISession s = sessions.OpenSession() )
			using( ITransaction t = s.BeginTransaction() )
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
		//[Ignore("Test Fails with Exception - do to with Criteria expression/parameter handling")]
		public void TestJunctionNotExpressionQBE()
		{
			using( ISession s = sessions.OpenSession() )
			using( ITransaction t = s.BeginTransaction() )
			{
				Componentizable master = GetMaster("hibernate", null, "ope%");
				ICriteria crit = s.CreateCriteria(typeof(Componentizable));
				Example ex = Example.Create(master).EnableLike();

			
				crit.Add(Expression.Expression.Or(Expression.Expression.Not(ex), ex));

				IList result = crit.List();
				Assert.IsNotNull(result);
//				if (!(GetDialect()
//					instanceof HSQLDialect) )
//					assertEquals(2, result.size());
				t.Commit();
			}
		}

		[Test]
		public void TestExcludingQBE()
		{
			using( ISession s = sessions.OpenSession() )
			using( ITransaction t = s.BeginTransaction() )
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
			using( ISession s = sessions.OpenSession() )
			{
				Componentizable master = GetMaster("hibernate", "ORM tool", "ORM tool1");
				s.Save(master);
				s.Flush();
			}

			using( ISession s = sessions.OpenSession() )
			{
				Componentizable master = GetMaster("hibernate", "open source", "open source1");
				s.Save(master);
				s.Flush();
			}

			using( ISession s = sessions.OpenSession() )
			{
				Componentizable master = GetMaster("hibernate", null, null);
				s.Save(master);
				s.Flush();
			}
		}

		private void DeleteData()
		{
			using( ISession s = sessions.OpenSession() )
			using( ITransaction t = s.BeginTransaction() )
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