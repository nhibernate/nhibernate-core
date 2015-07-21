using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1289
{
	[TestFixture,Ignore]
	public class Fixture:BugTestCase
	{
		protected override void OnSetUp()
		{
			using(var ses=OpenSession())
			using(var tran=ses.BeginTransaction())
			{
				var purchaseOrder = new Cons_PurchaseOrder
				                    	{
				                    		PurchaseItems = new HashSet<PurchaseItem>(),
				                    	};
				var product = new Cons_Product
				              	{
				              		ProductName = "abc",
				              		Units = 5,
				              		Price = "123",
				              		Description = "desc",
				              		ImageName = "abc"
				              	};


				var purchaseItem = new Cons_PurchaseItem
				                   	{
				                   		Product = product,
				                   		PurchaseOrder = purchaseOrder
				                   	};
				purchaseOrder.PurchaseItems.Add(purchaseItem);
				ses.Save(product);
				ses.Save(purchaseOrder);
				ses.Save(purchaseItem);

				tran.Commit();
			}
				
			
		}
		protected override void OnTearDown()
		{
			using (var ses = OpenSession())
			using (var tran = ses.BeginTransaction())
			{
				ses.Delete("from Cons_PurchaseOrder");
				ses.Delete("from Cons_PurchaseItem");
				ses.Delete("from Cons_Product");
				tran.Commit();
			}
		}

		[Test]
		public void ManyToOne_gets_implicit_polymorphism_correctly()
		{
			using (var ses = OpenSession())
			using (var tran = ses.BeginTransaction())
			{
				var purchaseItem = ses.Get<PurchaseItem>(1);
				Assert.That(purchaseItem, Is.AssignableFrom(typeof(Cons_PurchaseItem)));
				Assert.That(purchaseItem.Product, Is.AssignableFrom(typeof(Cons_Product)));
				tran.Commit();
			}
		}
	}
}
