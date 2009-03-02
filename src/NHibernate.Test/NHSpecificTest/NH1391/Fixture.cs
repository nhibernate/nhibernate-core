using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
namespace NHibernate.Test.NHSpecificTest.NH1391
{
	[TestFixture]
	public class Fixture:BugTestCase
	{
		protected override void OnSetUp()
		{
			using(var session=OpenSession())
			using(var tran=session.BeginTransaction())
			{
				var producta = new ProductA {Name = "producta"};
				var productb = new ProductB {Name = "productb"};
				var company = new Company { Products = new List<ProductA>() };
				company.Products.Add(producta);
				producta.Company = company;
				productb.Company = company;
				session.Save(company);
				session.Save(productb);
				session.Save(producta);
				tran.Commit();
			}
		}
		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var tran = session.BeginTransaction())
			{
				session.Delete("from Product");
				session.Delete("from Company");
				tran.Commit();
			}
		}


		[Test]
		public void Can_discriminate_subclass_on_list_with_lazy_loading()
		{
			using (var session = OpenSession())
			using (var tran = session.BeginTransaction())
			{
				var company = session.Get<Company>(1);
				Assert.That(company, Is.Not.Null);
				Assert.That(company.Products, Has.Count(1));
				Assert.That(company.Products[0], Is.AssignableFrom(typeof (ProductA)));
				tran.Commit();
			}
		}
	}
}
