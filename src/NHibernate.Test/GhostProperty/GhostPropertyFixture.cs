using System.Collections;
using NHibernate.ByteCode.Castle;
using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.GhostProperty
{
	[TestFixture]
	public class GhostPropertyFixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new[] { "GhostProperty.Mappings.hbm.xml" }; }
		}

		protected override void Configure(NHibernate.Cfg.Configuration configuration)
		{
			configuration.SetProperty(Environment.ProxyFactoryFactoryClass,
									  typeof(ProxyFactoryFactory).AssemblyQualifiedName);
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
		public void GhostPropertyMaintainIdentityMap()
		{
			using (ISession s = OpenSession())
			{
				var order = s.Get<Order>(1);

				Assert.AreSame(order.Payment, s.Load<Payment>(1));
			}
		}

	}
}