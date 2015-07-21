using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Test.NHSpecificTest.NH3489
{
	[TestFixture]
	[Ignore("Only run to test performance.")]
	public class Fixture : TestCaseMappingByCode
	{
		private const int batchSize = 900;

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Order>(cm =>
			{
				cm.Id(x => x.Id, m => m.Generator(Generators.HighLow));
				cm.Table("Orders");
				cm.Property(x => x.Name);
				cm.Set(x => x.Departments, m =>
				{
					m.Table("Orders_Departments");
					m.Key(k =>
					{
						k.Column("OrderId");
						k.NotNullable(true);
					});
					m.BatchSize(batchSize);
					m.Cascade(Mapping.ByCode.Cascade.All);
				}, r => r.ManyToMany(c => c.Column("DepartmentId")));
			});

			mapper.Class<Department>(cm =>
			{
				cm.Id(x => x.Id, m => m.Generator(Generators.HighLow));
				cm.Table("Departments");
				cm.Set(x => x.Orders, m =>
				{
					m.Table("Orders_Departments");
					m.Key(k =>
					{
						k.Column("DepartmentId");
						k.NotNullable(true);
					});
					m.BatchSize(batchSize);
					m.Cascade(Mapping.ByCode.Cascade.All);
					m.Inverse(true);
				}, r => r.ManyToMany(c => c.Column("OrderId")));
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void Configure(Configuration configuration)
		{
			base.Configure(configuration);
			configuration.SetProperty(Environment.BatchSize, batchSize.ToString(CultureInfo.InvariantCulture));
		}

		protected override void OnSetUp()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var department1 = new Department {Name = "Dep 1"};
				session.Save(department1);
				var department2 = new Department {Name = "Dep 2"};
				session.Save(department2);

				for (int i = 0; i < 10000; i++)
				{
					var order = new Order {Name = "Order " + (i + 1)};
					order.Departments.Add(department1);
					order.Departments.Add(department2);
					session.Save(order);
				}

				session.Flush();
				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				session.Flush();
				transaction.Commit();
			}
		}

		protected override string CacheConcurrencyStrategy
		{
			get { return null; }
		}

		[Test]
		public void PerformanceTest()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				IList<Order> orders = session.QueryOver<Order>().List();
				foreach (Order order in orders)
					order.Departments.ToList();
			}
			stopwatch.Stop();
			Console.WriteLine(stopwatch.Elapsed);
		}
	}
}