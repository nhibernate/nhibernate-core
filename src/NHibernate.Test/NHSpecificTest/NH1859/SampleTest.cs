using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1859
{
	[TestFixture]
	public class SampleTest : BugTestCase
	{
		protected override void OnSetUp()
		{
			base.OnSetUp();
			using (ISession session = OpenSession())
			{
				session.Save(new DomainClass {Id = 1});
				session.Flush();
			}
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();
			using (ISession session = OpenSession())
			{
				session.Delete("from DomainClass");
				session.Flush();
			}
		}

		[Test]
		public void NativeQueryWithTwoComments()
		{
			using (ISession session = OpenSession())
			{
				IQuery qry = session.CreateSQLQuery("select /* first comment */ o.* /* second comment*/ from domainclass o")
					.AddEntity("o", typeof (DomainClass));
				var res = qry.List<DomainClass>();
				Assert.AreEqual(res[0].Id, 1);
			}
		}
	}
}
