using System.Collections;
using NHibernate.Cfg;
using NHibernate.Cfg.Loquacious;
using NHibernate.Tuple.Entity;
using NUnit.Framework;

namespace NHibernate.Test.GhostProperty
{
	[TestFixture]
	public class GhostPropertyFixture : TestCase
	{
		private string log;

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new[] { "GhostProperty.Mappings.hbm.xml" }; }
		}

		protected override void Configure(Cfg.Configuration configuration)
		{
			configuration.DataBaseIntegration(x=> x.LogFormattedSql = false);
		}

		protected override void OnSetUp()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var wireTransfer = new WireTransfer
				{
					Id = 1
				};
				s.Persist(wireTransfer);
				s.Persist(new Order
				{
					Id = 1,
					Payment = wireTransfer
				});
				tx.Commit();
			}

		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.Delete("from Order");
				s.Delete("from Payment");
				tx.Commit();
			}
		}

		protected override void BuildSessionFactory()
		{
			using (var logSpy = new LogSpy(typeof(EntityMetamodel)))
			{
				base.BuildSessionFactory();
				log = logSpy.GetWholeLog();
			}
		}

		[Test]
		public void ShouldGenerateErrorForNonAutoPropGhostProp()
		{
			Assert.IsTrue(log.Contains("NHibernate.Test.GhostProperty.Order.Payment is not an auto property, which may result in uninitialized property access"));
		}

		[Test]
		public void CanGetActualValueFromLazyManyToOne()
		{
			using (ISession s = OpenSession())
			{
				var order = s.Get<Order>(1);

				Assert.IsTrue(order.Payment is WireTransfer);
			}
		}

		[Test]
		public void WillNotLoadGhostPropertyByDefault()
		{
			using (ISession s = OpenSession())
			{
				var order = s.Get<Order>(1);
				Assert.IsFalse(NHibernateUtil.IsPropertyInitialized(order, "Payment"));
			}
		}

		[Test]
		public void GhostPropertyMaintainIdentityMap()
		{
			using (ISession s = OpenSession())
			{
				var order = s.Get<Order>(1);

				Assert.AreSame(order.Payment, s.Load<Payment>(1));
			}
		}

		[Test, Ignore("This shows an expected edge case")]
		public void GhostPropertyMaintainIdentityMapUsingGet()
		{
			using (ISession s = OpenSession())
			{
				var payment = s.Load<Payment>(1);
				var order = s.Get<Order>(1);

				Assert.AreSame(order.Payment, payment);
			}
		}

		[Test]
		public void WillLoadGhostAssociationOnAccess()
		{
			// NH-2498
			using (ISession s = OpenSession())
			{
				Order order;
				using (var ls = new SqlLogSpy())
				{
					order = s.Get<Order>(1);
					var logMessage = ls.GetWholeLog();
					Assert.That(logMessage, Is.Not.StringContaining("FROM Payment"));
				}
				Assert.That(NHibernateUtil.IsPropertyInitialized(order, "Payment"), Is.False);

				// trigger on-access lazy load 
				var x = order.Payment;
				Assert.That(NHibernateUtil.IsPropertyInitialized(order, "Payment"), Is.True);
			}
		}

		[Test]
		public void WhenGetThenLoadOnlyNoLazyPlainProperties()
		{
			using (ISession s = OpenSession())
			{
				Order order;
				using (var ls = new SqlLogSpy())
				{
					order = s.Get<Order>(1);
					var logMessage = ls.GetWholeLog();
					Assert.That(logMessage, Is.Not.StringContaining("ALazyProperty"));
					Assert.That(logMessage, Is.StringContaining("NoLazyProperty"));
				}
				Assert.That(NHibernateUtil.IsPropertyInitialized(order, "NoLazyProperty"), Is.True);
				Assert.That(NHibernateUtil.IsPropertyInitialized(order, "ALazyProperty"), Is.False);

				using (var ls = new SqlLogSpy())
				{
					var x = order.ALazyProperty;
					var logMessage = ls.GetWholeLog();
					Assert.That(logMessage, Is.StringContaining("ALazyProperty"));
				}
				Assert.That(NHibernateUtil.IsPropertyInitialized(order, "ALazyProperty"), Is.True);
			}
		} 
	}
}