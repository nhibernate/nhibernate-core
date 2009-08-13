using System.Collections;
using NHibernate.Test.Linq.Entities;

namespace NHibernate.Test.Linq
{
    public class LinqTestCase : TestCase
    {
        private Northwind _northwind;
        private ISession _session;

        protected override string MappingsAssembly
        {
            get { return "NHibernate.Test"; }
        }

        protected override IList Mappings
        {
            get
            {
                return new[]
                           {
                               "Linq.Mappings.Customer.hbm.xml",
                               "Linq.Mappings.Employee.hbm.xml",
                               "Linq.Mappings.Order.hbm.xml",
                               "Linq.Mappings.OrderLine.hbm.xml",
                               "Linq.Mappings.Product.hbm.xml",
                               "Linq.Mappings.ProductCategory.hbm.xml",
                               "Linq.Mappings.Region.hbm.xml",
                               "Linq.Mappings.Shipper.hbm.xml",
                               "Linq.Mappings.Supplier.hbm.xml",
                               "Linq.Mappings.Territory.hbm.xml"
                           };
            }
        }

        protected override void OnSetUp()
        {
            base.OnSetUp();

            _session = OpenSession();
            _northwind = new Northwind(_session);
        }

        protected override void OnTearDown()
        {
            using (ISession s = OpenSession())
            using (ITransaction tx = s.BeginTransaction())
            {
                s.Delete("from Order");
                s.Delete("from Product");
                s.Delete("from ProductCategory");
                s.Delete("from Supplier");
                s.Delete("from Customer");
                s.Delete("from Employee");
                s.Delete("from Shipper");
                s.Delete("from Region");
                s.Delete("from Territory");

                tx.Commit();
            }
        }

        protected Northwind db
        {
            get { return _northwind; }
        }

        protected ISession session
        {
            get { return _session; }
        }
    }
}