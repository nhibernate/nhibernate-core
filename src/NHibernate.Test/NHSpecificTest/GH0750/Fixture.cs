using System.Collections.Generic;
using NHibernate.Criterion;
using NHibernate.Transform;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH0750
{
	public class ProductSummary
	{
		public int ProductId { get; set; }

		public string Name { get; set; }

		public int TotalQuantity { get; set; }

		public decimal TotalPrice { get; set; }
	}

	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				var greenTea = new Product()
				{
					ProductId = 1,
					Name = "Green Tea",
					UnitPrice = 5
				};

				session.Save(greenTea);

				var blackTea = new Product()
				{
					ProductId = 2,
					Name = "Black Tea",
					UnitPrice = 10
				};

				session.Save(blackTea);

				var greenTeaOrder = new Order()
				{
					OrderId = 1,
					OrderDate = System.DateTime.Now
				};

				session.Save(greenTeaOrder);

				greenTeaOrder.OrderLines.Add(new OrderLine() { Order = greenTeaOrder, Product = greenTea, Quantity = 2, UnitPrice = greenTea.UnitPrice ?? 0 });

				session.Save(greenTeaOrder);

				var blackTeaOrder = new Order()
				{
					OrderId = 2,
					OrderDate = System.DateTime.Now
				};

				session.Save(blackTeaOrder);

				blackTeaOrder.OrderLines.Add(new OrderLine() { Order = blackTeaOrder, Product = blackTea, Quantity = 5, UnitPrice = blackTea.UnitPrice ?? 0 });

				session.Save(blackTeaOrder);
				tx.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.CreateQuery("delete from OrderLine").ExecuteUpdate();
				s.CreateQuery("delete from System.Object").ExecuteUpdate();
				tx.Commit();
			}
		}

		[Test]
		public void MapQueryResultWithAliasToBeanTransformer()
		{
			Assert.DoesNotThrow(() => GetSaleSummaries());
		}

		public IList<ProductSummary> GetSaleSummaries()
		{
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				var criteria = session
					.CreateCriteria<Order>("O")
					.CreateCriteria("O.OrderLines", "OI", SqlCommand.JoinType.InnerJoin)
					.CreateCriteria("OI.Product", "P", SqlCommand.JoinType.InnerJoin);

				var summaeries = criteria
					.SetProjection(Projections.ProjectionList().Add(Projections.Property("P.ProductId"), "ProductId")
					.Add(Projections.Property("P.Name"), "Name")
					.Add(Projections.Sum(Projections.Cast(NHibernateUtil.Int32, Projections.Property("OI.Quantity"))), "TotalQuantity")
					.Add(Projections.Sum("OI.UnitPrice"), "TotalPrice")
					.Add(Projections.GroupProperty("P.ProductId"))
					.Add(Projections.GroupProperty("P.Name")))
					.SetResultTransformer(Transformers.AliasToBean(typeof(ProductSummary)))
					.List<ProductSummary>();
				tx.Commit();
				return summaeries;
			}
		}
	}
}
