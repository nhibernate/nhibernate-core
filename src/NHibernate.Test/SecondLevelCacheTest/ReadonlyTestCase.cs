using System;
using System.Collections;
using System.Reflection;
using log4net;
using log4net.Config;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.Hql.Ast.ANTLR;
using NHibernate.Hql.Classic;
using NUnit.Framework;
using NHibernate.DomainModel.Northwind.Entities;

namespace NHibernate.Test.SecondLevelCacheTests
{
    public class ReadonlyTestCase : TestCase
    {

        protected override IList Mappings
        {
            get
            {
                return new[]
				{
					"Northwind.Mappings.Customer.hbm.xml",
					"Northwind.Mappings.Employee.hbm.xml",
					"Northwind.Mappings.Order.hbm.xml",
					"Northwind.Mappings.OrderLine.hbm.xml",
					"Northwind.Mappings.Product.hbm.xml",
					"Northwind.Mappings.ProductCategory.hbm.xml",
					"Northwind.Mappings.Region.hbm.xml",
					"Northwind.Mappings.Shipper.hbm.xml",
					"Northwind.Mappings.Supplier.hbm.xml",
					"Northwind.Mappings.Territory.hbm.xml",
					"Northwind.Mappings.AnotherEntity.hbm.xml",
					"Northwind.Mappings.Role.hbm.xml",
					"Northwind.Mappings.User.hbm.xml",
					"Northwind.Mappings.TimeSheet.hbm.xml",
					"Northwind.Mappings.Animal.hbm.xml",
					"Northwind.Mappings.Patient.hbm.xml"
				};
            }
        }

        protected override void OnSetUp()
        {
            base.OnSetUp();
        }

        protected override void OnTearDown()
        {
        }

        protected override void CreateSchema()
        {
        }

        protected override void DropSchema()
        {
        }

        protected override bool CheckDatabaseWasCleaned()
        {
            // We are read-only, so we're theoretically always clean.
            return true;
        }

        protected override void ApplyCacheSettings(Configuration configuration)
        {
            // Patrick Earl: I wasn't sure if making this do nothing was important, but I left it here since it wasn't running in the code when I changed it.
            base.ApplyCacheSettings(configuration);
        }
    }
}
