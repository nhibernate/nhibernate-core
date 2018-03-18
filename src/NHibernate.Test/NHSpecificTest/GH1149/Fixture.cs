using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1149
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		private int _companyId = 0;

		protected override void OnSetUp()
		{
			base.OnSetUp();
			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					var company = new Company { Name = "Test Company" };

					company.Address = new Address(company) { AddressLine1 = "Company Address" };

					_companyId = (int) session.Save(company);

					tx.Commit();
				}
			}
		}

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Delete("from Address");
				session.Delete("from Company");
				session.Flush();
				transaction.Commit();
			}
		}

		[Test]
		public void StatelessSessionLoadsOneToOneRelatedObject()
		{
			using (var stateless = Sfi.OpenStatelessSession())
			{
				var loadedCompany = stateless.Get<Company>(_companyId);

				Assert.That(loadedCompany, Is.Not.Null);
				Assert.That(loadedCompany.Name, Is.Not.Null);
				Assert.That(loadedCompany.Address, Is.Not.Null);
				Assert.That(loadedCompany.Address.AddressLine1, Is.Not.Null);
			}
		}

	}
}
