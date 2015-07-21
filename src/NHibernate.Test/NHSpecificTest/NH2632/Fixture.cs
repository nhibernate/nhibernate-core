using System;
using NHibernate.Cfg;
using NHibernate.Cfg.Loquacious;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2632
{
	public class Fixture: TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			// The impl/mapping of the bidirectional one-to-many sucks but was provided as is
			var mapper = new ModelMapper();
			mapper.BeforeMapClass += (i, t, cm) => cm.Id(map =>
																									 {
																										 map.Column((t.Name + "Id").ToUpperInvariant());
																										 map.Generator(Generators.HighLow, g => g.Params(new { max_lo = "1000" }));
																									 });
			mapper.Class<Customer>(ca =>
			{
				ca.Lazy(false);
				ca.Id(x => x.Id, m => { });
				ca.NaturalId(x => x.Property(c => c.Name, p => p.NotNullable(true)));
				ca.Property(x => x.Address, p => p.Lazy(true));
				ca.Set(c => c.Orders, c =>
				{
					c.Key(x => x.Column("CUSTOMERID"));
					c.Inverse(true);
					c.Cascade(Mapping.ByCode.Cascade.All);
				}, c => c.OneToMany());
			});
			mapper.Class<Order>(cm =>
								{
														cm.Id(x => x.Id, m => { });
														cm.Property(x => x.Date);
														cm.ManyToOne(x => x.Customer, map => map.Column("CUSTOMERID"));
								});
			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void Configure(Cfg.Configuration configuration)
		{
			configuration.DataBaseIntegration(di => di.KeywordsAutoImport = Hbm2DDLKeyWords.AutoQuote);
		}

		private class Scenario : IDisposable
		{
			private readonly ISessionFactory factory;
			private object customerId;

			public Scenario(ISessionFactory factory)
			{
				this.factory = factory;
				using (ISession s = factory.OpenSession())
				{
					using (ITransaction t = s.BeginTransaction())
					{
						var customer = new Customer { Name="Zombi", Address = "Bah?!??"};
						var order = new Order { Date = DateTime.Today, Customer = customer };
						customerId = s.Save(customer);
						s.Save(order);
						t.Commit();
					}
				}
			}

			public object CustomerId
			{
				get { return customerId; }
			}

			public void Dispose()
			{
				using (ISession s = factory.OpenSession())
				{
					using (ITransaction t = s.BeginTransaction())
					{
						s.Delete("from Order");
						s.Delete("from Customer");
						t.Commit();
					}
				}
			}
		}

		[Test]
		public void GettingCustomerDoesNotThrow()
		{
			using (var scenario = new Scenario(Sfi))
			{
				using (var session = OpenSession())
				{
					Customer customer = null;
					Assert.That(() => customer = session.Get<Customer>(scenario.CustomerId), Throws.Nothing);
					// An entity defined with lazy=false can't have lazy properties (as reported by the WARNING; see EntityMetamodel class)
					Assert.That(NHibernateUtil.IsInitialized(customer.Address), Is.True);
					Assert.That(customer.Address, Is.EqualTo("Bah?!??"));
				}
			}
		}
	}
}