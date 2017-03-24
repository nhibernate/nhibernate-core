using System.Linq;
using NHibernate.DomainModel.Northwind.Entities;
using NUnit.Framework;
using NHibernate.Linq;

namespace NHibernate.Test.Linq
{
	[TestFixture]
	public class DeleteTests : LinqTestCase
	{
		[Test]
		public void CanDeleteSimpleExpression()
		{
			//NH-3659
			using (this.session.BeginTransaction())
			{
				var beforeDeleteCount = this.session.Query<User>().Count(u => u.Id > 0);

				var deletedCount = this.session.Delete<User>(u => u.Id > 0);

				var afterDeleteCount = this.session.Query<User>().Count(u => u.Id > 0);

				Assert.AreEqual(beforeDeleteCount, deletedCount);

				Assert.AreEqual(0, afterDeleteCount);
			}
		}

		[Test]
		public void CanDeleteComplexExpression()
		{
			//NH-3659
			using (this.session.BeginTransaction())
			{
				var cities = new string[] { "Paris", "Madrid" };

				var beforeDeleteCount = this.session.Query<Customer>().Count(c => c.Orders.Count() == 0 && cities.Contains(c.Address.City));

				var deletedCount = this.session.Delete<Customer>(c => c.Orders.Count() == 0 && cities.Contains(c.Address.City));

				var afterDeleteCount = this.session.Query<Customer>().Count(c => c.Orders.Count() == 0 && cities.Contains(c.Address.City));

				Assert.AreEqual(beforeDeleteCount, deletedCount);

				Assert.AreEqual(0, afterDeleteCount);
			}
		}
	}
}
