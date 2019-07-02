using NHibernate.Benchmark.Database.Northwind.Entities;
using NHibernate.Cfg;

namespace NHibernate.Benchmark
{
	public class NorthwindBenchmarkCase : BenchmarkCase
	{
		protected override string[] Mappings => new[]
		{
			"Database.Northwind.Mappings.Animal.hbm.xml",
			"Database.Northwind.Mappings.AnotherEntity.hbm.xml",
			"Database.Northwind.Mappings.Customer.hbm.xml",
			"Database.Northwind.Mappings.Employee.hbm.xml",
			"Database.Northwind.Mappings.Order.hbm.xml",
			"Database.Northwind.Mappings.OrderLine.hbm.xml",
			"Database.Northwind.Mappings.Patient.hbm.xml",
			"Database.Northwind.Mappings.Product.hbm.xml",
			"Database.Northwind.Mappings.Region.hbm.xml",
			"Database.Northwind.Mappings.ProductCategory.hbm.xml",
			"Database.Northwind.Mappings.Role.hbm.xml",
			"Database.Northwind.Mappings.Shipper.hbm.xml",
			"Database.Northwind.Mappings.Supplier.hbm.xml",
			"Database.Northwind.Mappings.Territory.hbm.xml",
			"Database.Northwind.Mappings.TimeSheet.hbm.xml",
			"Database.Northwind.Mappings.User.hbm.xml",
		};

		protected override void AddConfigurationResource(Configuration configuration, string file)
		{
			configuration.AddResource(typeof(Program).Namespace + "." + file, typeof(Animal).Assembly);
		}
	}
}
