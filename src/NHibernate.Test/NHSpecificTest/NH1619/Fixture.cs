using NHibernate.Dialect;
using NHibernate.SqlTypes;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1619
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH1619"; }
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is PostgreSQLDialect;
		}

		[Test]
		public void SavingAndRetrieving()
		{
			var entity = new Dude
			             	{
			             		BooleanValue = true
			             	};

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Save(entity);
				tx.Commit();

				Assert.AreEqual(true, s.CreateQuery("from Dude").UniqueResult<Dude>().BooleanValue);
			}

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Delete(entity);
				tx.Commit();
			}
		}

		[Test]
		public void UsingBooleanPostgreSQLType()
		{
			Assert.AreEqual("boolean", Dialect.GetTypeName(SqlTypeFactory.Boolean));
		}
	}
}