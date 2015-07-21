using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2242
{
	[TestFixture]
	public class FormulaTest : BugTestCase
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect as MsSql2005Dialect != null;
		}

		[Test]
		public void FormulaOfEscapedDomainClassShouldBeRetrievedCorrectly()
		{
			using (ISession session = OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					var entity = new EscapedFormulaDomainClass();
					entity.Id = 1;
					session.Save(entity);

					transaction.Commit();
				}

				session.Clear();

				using (ITransaction transaction = session.BeginTransaction())
				{
					var entity = session.Get<EscapedFormulaDomainClass>(1);

					Assert.AreEqual(1, entity.Formula);
					session.Delete(entity);

					transaction.Commit();
				}
			}
		}

		[Test]
		public void FormulaOfUnescapedDomainClassShouldBeRetrievedCorrectly()
		{
			using (ISession session = OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					var entity = new UnescapedFormulaDomainClass();
					entity.Id = 1;
					session.Save(entity);

					transaction.Commit();
				}

				session.Clear();

				using (ITransaction transaction = session.BeginTransaction())
				{
					var entity = session.Get<UnescapedFormulaDomainClass>(1);

					Assert.AreEqual(1, entity.Formula);
					session.Delete(entity);
					transaction.Commit();
				}
			}
		}
	}
}