using System.Collections;
using NHibernate.Cfg;
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

		protected override string[] Mappings
		{
			get { return new[] { "GhostProperty.Mappings.hbm.xml" }; }
		}

		protected override void Configure(Cfg.Configuration configuration)
		{
			configuration.ByCode().DataBaseIntegration(x=> x.LogFormattedSql = false);
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
				var creditCard = new CreditCard
				{
					Id = 2
				};
				s.Persist(creditCard);
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

		protected override DebugSessionFactory BuildSessionFactory()
		{
			using (var logSpy = new LogSpy(typeof(EntityMetamodel)))
			{
				var factory = base.BuildSessionFactory();
				log = logSpy.GetWholeLog();
				return factory;
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
		public void CanGetInitializedLazyManyToOneAfterClosedSession()
		{
			Order order;
			Payment payment;

			using (var s = OpenSession())
			{
				order = s.Get<Order>(1);
				payment = order.Payment; // Initialize Payment
			}

			Assert.That(order.Payment, Is.EqualTo(payment));
			Assert.That(order.Payment is WireTransfer, Is.True);
		}

		[Test]
		public void InitializedLazyManyToOneBeforeParentShouldNotBeAProxy()
		{
			Order order;
			Payment payment;

			using (var s = OpenSession())
			{
				payment = s.Load<Payment>(1);
				NHibernateUtil.Initialize(payment);
				order = s.Get<Order>(1);
				// Here the Payment property should be unwrapped
				payment = order.Payment;
			}

			Assert.That(order.Payment, Is.EqualTo(payment));
			Assert.That(order.Payment is WireTransfer, Is.True);
		}

		[Test]
		public void SetUninitializedProxyShouldNotTriggerPropertyInitialization()
		{
			using (var s = OpenSession())
			{
				var order = s.Get<Order>(1);
				Assert.That(order.Payment is WireTransfer, Is.True); // Load property
				Assert.That(NHibernateUtil.IsPropertyInitialized(order, "Payment"), Is.True);
				order.Payment = s.Load<Payment>(2);
				Assert.That(NHibernateUtil.IsPropertyInitialized(order, "Payment"), Is.True);
				Assert.That(NHibernateUtil.IsInitialized(order.Payment), Is.False);
				Assert.That(order.Payment is WireTransfer, Is.False);
			}
		}

		[Test]
		public void SetInitializedProxyShouldNotResetPropertyInitialization()
		{
			using (var s = OpenSession())
			{
				var order = s.Get<Order>(1);
				var payment = s.Load<Payment>(2);
				Assert.That(order.Payment is WireTransfer, Is.True); // Load property
				Assert.That(NHibernateUtil.IsPropertyInitialized(order, "Payment"), Is.True);
				NHibernateUtil.Initialize(payment);
				order.Payment = payment;
				Assert.That(NHibernateUtil.IsPropertyInitialized(order, "Payment"), Is.True);
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
					Assert.That(logMessage, Does.Not.Contain("FROM Payment"));
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
					Assert.That(logMessage, Does.Not.Contain("ALazyProperty"));
					Assert.That(logMessage, Does.Contain("NoLazyProperty"));
				}
				Assert.That(NHibernateUtil.IsPropertyInitialized(order, "NoLazyProperty"), Is.True);
				Assert.That(NHibernateUtil.IsPropertyInitialized(order, "ALazyProperty"), Is.False);

				using (var ls = new SqlLogSpy())
				{
					var x = order.ALazyProperty;
					var logMessage = ls.GetWholeLog();
					Assert.That(logMessage, Does.Contain("ALazyProperty"));
				}
				Assert.That(NHibernateUtil.IsPropertyInitialized(order, "ALazyProperty"), Is.True);
			}
		}

		[Test]
		public void AcceptPropertySetWithTransientObject()
		{
			Order order;
			using (var s = OpenSession())
			{
				order = s.Get<Order>(1);
			}

			var newPayment = new WireTransfer();
			order.Payment = newPayment;

			Assert.That(order.Payment, Is.EqualTo(newPayment));
		}

		[Test]
		public void WillFetchJoinInSingleHqlQuery()
		{
			Order order = null;

			using (ISession s = OpenSession())
			{
				order = s.CreateQuery("from Order o left join fetch o.Payment where o.Id = 1").List<Order>()[0];
			}

			Assert.DoesNotThrow(() => { var x = order.Payment; });
		}
	}
}
