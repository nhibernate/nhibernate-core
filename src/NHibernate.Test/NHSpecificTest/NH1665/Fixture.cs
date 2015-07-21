using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1665
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect.SupportsSequences;
		}

		[Test]
		public void SupportsHibernateQuotingSequenceName()
		{
			ISession session = OpenSession();
			session.BeginTransaction();

			var e = new MyEntity { Name = "entity-1" };
			session.Save(e);
			Assert.AreEqual(1, (int)session.GetIdentifier(e));

			session.Delete(e);
			session.Transaction.Commit();
			session.Close();
		}
	}
}