using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NHibernate.Dialect;
using NUnit.Framework;
using NHibernate.Test.NHSpecificTest;
using Iesi.Collections.Generic;
using NHibernate;
using System.Data;
using NHibernate.Criterion;

namespace NHibernate.Test.NHSpecificTest.NH3898
{
	/// <summary>
	/// <para>
	/// </para>
	/// </remarks>
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void Configure(NHibernate.Cfg.Configuration configuration)
		{
			#region removing possible second-level-cache configs
			configuration.Properties.Remove(NHibernate.Cfg.Environment.CacheProvider);
			configuration.Properties.Remove(NHibernate.Cfg.Environment.UseQueryCache);
			configuration.Properties.Add(NHibernate.Cfg.Environment.UseQueryCache, "true");
			configuration.Properties.Remove(NHibernate.Cfg.Environment.UseSecondLevelCache);
			#endregion

			base.Configure(configuration);
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();
			using (ISession s = OpenSession())
			{
				int countUpdate = 0;

				countUpdate =
					s
						.CreateSQLQuery("DELETE FROM T_EMPLOYEE")
						.ExecuteUpdate();
				Assert.AreEqual(1, countUpdate);

				s.Flush();
			}
		}

		protected override void OnSetUp()
		{
			base.OnSetUp();
		}

		protected override bool AppliesTo(global::NHibernate.Dialect.Dialect dialect)
		{
			//return dialect as MsSql2005Dialect != null;
			return base.AppliesTo(dialect);
		}

		/// <summary>
		/// Test that reproduces the problem.
		/// </summary>
		[Test]
		public void GeneratedInsertUpdateTrue()
		{
			using (ISession session = this.OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					Employee employee = new Employee();
					employee.Id = 1;
					employee.Name = "Employee 1";
					employee.PromotionCount = 9999999;
					session.Save(employee);
					Assert.AreEqual(0, employee.PromotionCount);
					tx.Commit();
				}
			}

			using (ISession session = this.OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					Employee employee = session.Get<Employee>(1);
					employee.Name = "Employee 1 changed";
					employee.PromotionCount++;
					Assert.AreEqual(1, employee.PromotionCount);
					tx.Commit();
				}
			}

			using (ISession session = this.OpenSession())
			{
				Employee employee = session.Get<Employee>(1);
				Assert.AreEqual("Employee 1 changed", employee.Name);
				Assert.AreEqual(1, employee.PromotionCount);
			}
		}
	}
}