using NHibernate.Cfg;
using NHibernate.DomainModel.Northwind.Entities;

namespace NHibernate.Benchmark
{
	public class NorthwindBenchmarkCase : BenchmarkCase
	{
		protected override string[] Mappings => new[]
		{
			"NHibernate.DomainModel.Northwind.Mappings.Animal.hbm.xml",
			"NHibernate.DomainModel.Northwind.Mappings.AnotherEntity.hbm.xml",
			"NHibernate.DomainModel.Northwind.Mappings.Customer.hbm.xml",
			"NHibernate.DomainModel.Northwind.Mappings.Employee.hbm.xml",
			"NHibernate.DomainModel.Northwind.Mappings.Order.hbm.xml",
			"NHibernate.DomainModel.Northwind.Mappings.OrderLine.hbm.xml",
			"NHibernate.DomainModel.Northwind.Mappings.Patient.hbm.xml",
			"NHibernate.DomainModel.Northwind.Mappings.Product.hbm.xml",
			"NHibernate.DomainModel.Northwind.Mappings.Region.hbm.xml",
			"NHibernate.DomainModel.Northwind.Mappings.ProductCategory.hbm.xml",
			"NHibernate.DomainModel.Northwind.Mappings.Role.hbm.xml",
			"NHibernate.DomainModel.Northwind.Mappings.Shipper.hbm.xml",
			"NHibernate.DomainModel.Northwind.Mappings.Supplier.hbm.xml",
			"NHibernate.DomainModel.Northwind.Mappings.Territory.hbm.xml",
			"NHibernate.DomainModel.Northwind.Mappings.TimeSheet.hbm.xml",
			"NHibernate.DomainModel.Northwind.Mappings.User.hbm.xml",
		};

		protected override void AddConfigurationResource(Configuration configuration, string file)
		{
			configuration.AddResource(file, typeof(Animal).Assembly);
		}
	}
}
