using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.DomainModel.Northwind.Entities;
using NUnit.Framework;

namespace NHibernate.Test.Criteria.ReadonlyTests
{
	public abstract class CriteriaNorthwindReadonlyTestCase : NHibernate.Test.Linq.ReadonlyTestCase
	{
		private ISession _session = null;
		protected NorthwindQueryOver db;

		protected override string[] Mappings
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

		public ISession Session
		{
			get { return _session; }
		}

		protected override void OnSetUp()
		{
			_session = OpenSession();
			db = new NorthwindQueryOver(_session);
			base.OnSetUp();
		}

		protected override void OnTearDown()
		{
			if (_session.IsOpen)
			{
				_session.Close();
			}
		}

		public static void AssertByIds<TEntity, TId>(IEnumerable<TEntity> entities, TId[] expectedIds, Converter<TEntity, TId> entityIdGetter)
		{
			Assert.That(entities.Select(x => entityIdGetter(x)), Is.EquivalentTo(expectedIds));
		}

		protected IQueryOver<Customer, Customer> Customers => Session.QueryOver<Customer>();
	}
}
