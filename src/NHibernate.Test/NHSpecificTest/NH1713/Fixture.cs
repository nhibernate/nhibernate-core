using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1713
{
	[TestFixture, Ignore("Should be fixed in some way.")]
	public class Fixture : BugTestCase
	{
		/* NOTE
		 * This test should be fixed in some way at least to support Money.
		 * So far it is only a demostration that using 
		 * <property name="prepare_sql">false</property>
		 * we should do some additional work for INSERT+UPDATE
		 */
		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(Environment.PrepareSql, "true");
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is MsSql2000Dialect;
		}

		[Test]
		public void Can_Save_Money_Column()
		{
			Assert.That(PropertiesHelper.GetBoolean(Environment.PrepareSql, cfg.Properties, false));
			var item = new A {Amount = 2600};

			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					s.Save(item);
					tx.Commit();
				}
			}

			Assert.IsTrue(item.Id > 0);

			// cleanup
			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					s.Delete("from A");
					tx.Commit();
				}
			}
		}

		[Test]
		public void Can_Update_Money_Column()
		{
			Assert.That(PropertiesHelper.GetBoolean(Environment.PrepareSql, cfg.Properties, false));
			object savedId;
			var item = new A {Amount = (decimal?) 2600.55};

			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					savedId= s.Save(item);
					tx.Commit();
				}
			}

			Assert.That(item.Id, Is.GreaterThan(0));

			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					var item2 = s.Load<A>(savedId);
					item2.Amount = item2.Amount - 1.5m;
					s.SaveOrUpdate(item2);
					tx.Commit();
				}
			}

			// cleanup
			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					s.Delete("from A");
					tx.Commit();
				}
			}
		}
	}
}