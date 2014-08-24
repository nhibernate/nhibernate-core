using System.Linq;
using NUnit.Framework;
using NHibernate.Linq;

namespace NHibernate.Test.NHSpecificTest.NH2362
{
	public class Fixture : BugTestCase
	{
		[Test]
		public void CanParseMultipleGroupByAndSelect()
		{
			using (var session = OpenSession())
			{
				(from p
						in session.Query<Product>()
				 group p by new { CategoryId = p.Category.Id, SupplierId = p.Supplier.Id }
					 into g
					 let totalPrice = g.Sum(p => p.Price)
					 select new { g.Key.CategoryId, g.Key.SupplierId, TotalPrice = totalPrice }).ToList();
				
			}
		}

		[Test]
		public void CanParseMultipleGroupBy()
		{
			using (var session = OpenSession())
			{
				(from p
				 	in session.Query<Product>()
				 group p by new {CategoryId = p.Category.Id, SupplierId = p.Supplier.Id}
				 into g
				 let totalPrice = g.Sum(p => p.Price)
				 select totalPrice).ToList();
			}
		}
	}
}