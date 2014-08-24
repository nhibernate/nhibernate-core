using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.DomainModel.Northwind.Entities;
using SharpTestsEx;

namespace NHibernate.Test.Linq
{
	public class LinqTestCase : ReadonlyTestCase
	{
		private Northwind _northwind;
		private ISession _session;

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

		protected Northwind db
		{
			get { return _northwind; }
		}

		protected ISession session
		{
			get { return _session; }
		}

		protected override void OnSetUp()
		{
			base.OnSetUp();

			_session = OpenSession();
			_northwind = new Northwind(_session);
		}

		protected override void OnTearDown()
		{
			if (_session.IsOpen)
			{
				_session.Close();
			}
		}

		public void AssertByIds<TEntity, TId>(IEnumerable<TEntity> entities, TId[] expectedIds, Converter<TEntity, TId> entityIdGetter)
		{
			entities.Select(x => entityIdGetter(x)).Should().Have.SameValuesAs(expectedIds);
		}
	}
}