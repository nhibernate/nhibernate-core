using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Test.Linq.Entities;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
	public class LinqTestCase : ReadonlyTestCase
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
				       		"Linq.Mappings.Territory.hbm.xml",
				       		"Linq.Mappings.AnotherEntity.hbm.xml",
				       		"Linq.Mappings.Role.hbm.xml",
				       		"Linq.Mappings.User.hbm.xml",
				       		"Linq.Mappings.TimeSheet.hbm.xml",
				       		"Linq.Mappings.Animal.hbm.xml",
				       		"Linq.Mappings.Patient.hbm.xml"
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

		public void AssertByIds<T, K>(IEnumerable<T> q, K[] ids, Converter<T, K> getId)
		{
			int current = 0;
			foreach (T customer in q)
			{
				Assert.AreEqual(ids[current], getId(customer));
				current += 1;
			}
			Assert.AreEqual(current, ids.Length);
		}
	}
}