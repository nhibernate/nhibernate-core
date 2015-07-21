using System.Linq;
using NHibernate.Driver;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2296
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override bool AppliesTo(Engine.ISessionFactoryImplementor factory)
		{
			return !(factory.ConnectionProvider.Driver is OracleManagedDataClientDriver);
		}

		protected override void OnSetUp()
		{
			base.OnSetUp();

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var o = new Order() { AccountName = "Acct1" };
				o.Products.Add(new Product() { StatusReason = "Success", Order = o });
				o.Products.Add(new Product() { StatusReason = "Failure", Order = o });
				s.Save(o);

				o = new Order() { AccountName = "Acct2" };
				s.Save(o);

				o = new Order() { AccountName = "Acct3" };
				o.Products.Add(new Product() { StatusReason = "Success", Order = o });
				o.Products.Add(new Product() { StatusReason = "Failure", Order = o });
				s.Save(o);

				tx.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.Delete("from Product");
				s.Delete("from Order");
				tx.Commit();
			}

			base.OnTearDown();
		}

		[Test]
		public void Test()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var orders = s.CreateQuery("select o from Order o") 
					.SetMaxResults(2)
					.List<Order>();

				// trigger lazy-loading of products, using subselect fetch. 
				string sr = orders[0].Products[0].StatusReason;

				// count of entities we want:
				int ourEntities = orders.Count + orders.Sum(o => o.Products.Count);

				Assert.That(s.Statistics.EntityCount, Is.EqualTo(ourEntities));
			}
		}
	}
}