using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1831
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return !(dialect is Oracle8iDialect);
		}

		[Test]
		public void CorrectPrecedenceForBitwiseOperators()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				const string hql = @"SELECT dt FROM DocumentType dt WHERE dt.systemAction & :sysAct = :sysAct ";

				s.CreateQuery(hql).SetParameter("sysAct", SystemAction.Denunciation).List();
			}
		}
	}
}
