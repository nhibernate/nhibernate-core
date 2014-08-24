using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1898
{
	[TestFixture]
	public class SampleTest : BugTestCase
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect as MsSql2005Dialect != null;
		}

		[Test]
		public void TypeOfParametersShouldBeSetCorrectly()
		{
			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					var entity = new DomainClass {Id = 1, Data = "some oldValue data"};
					session.Save(entity);
					tx.Commit();
				}
			}
			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					session.GetNamedQuery("replaceQuery").SetString("old", "oldValue").SetString("new", "newValue").ExecuteUpdate();
					tx.Commit();
				}
				using (ITransaction tx = session.BeginTransaction())
				{
					var entity = session.Get<DomainClass>(1);

					Assert.AreEqual("some newValue data", entity.Data);
					session.Delete(entity);
					tx.Commit();
				}
			}
		}
	}
}
