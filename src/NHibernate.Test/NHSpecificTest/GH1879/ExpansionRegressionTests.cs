using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1879
{
	[TestFixture]
	public class ExpansionRegressionTests : GH1879BaseFixture<Invoice>
	{
		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Save(new Invoice { InvoiceNumber = 1, Amount = 10, SpecialAmount = 100, Paid = false });
				session.Save(new Invoice { InvoiceNumber = 2, Amount = 10, SpecialAmount = 100, Paid = true });
				session.Save(new Invoice { InvoiceNumber = 2, Amount = 10, SpecialAmount = 110, Paid = false });
				session.Save(new Invoice { InvoiceNumber = 2, Amount = 10, SpecialAmount = 110, Paid = true });

				session.Flush();
				transaction.Commit();
			}
		}

		[Test]
		public void MethodShouldNotExpandForNonConditionalOrCoalesce()
		{
			using (var session = OpenSession())
			{
				Assert.That(session.Query<Invoice>().Count(e => ((object)(e.Amount + e.SpecialAmount)).Equals(110)), Is.EqualTo(2));
			}
		}

		[Test]
		public void MethodShouldNotExpandForConditionalWithPropertyAccessor()
		{
			using (var session = OpenSession())
			{
				Assert.That(session.Query<Invoice>().Count(e => ((object)(e.Paid ? e.Amount : e.SpecialAmount)).Equals(10)), Is.EqualTo(2));
			}
		}

		[Test]
		public void MethodShouldNotExpandForCoalesceWithPropertyAccessor()
		{
			using (var session = OpenSession())
			{
				Assert.That(session.Query<Invoice>().Count(e => ((object)(e.SpecialAmount ?? e.Amount)).Equals(100)), Is.EqualTo(2));
			}
		}
	}
}
