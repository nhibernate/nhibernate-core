using System.Collections;
using NHibernate.Dialect;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace NHibernate.Test.VersionTest.Db.MsSQL
{
	// related issues NH-1687, NH-1685

	[TestFixture, Ignore("Not fixed yet.")]
	public class GeneratedBinaryVersionFixture : TestCase
	{
		protected override IList Mappings
		{
			get { return new[] { "VersionTest.Db.MsSQL.SimpleVersioned.hbm.xml" }; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is MsSql2000Dialect;
		}

		[Test]
		public void ShouldRetrieveVersionAfterFlush()
		{
			var e = new SimpleVersioned {Something = "something"};
			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					Assert.That(e.LastModified, Is.Null);
					s.Save(e);
					s.Flush();
					Assert.That(e.LastModified, Is.Not.Null);
					s.Delete(e);
					tx.Commit();
				}
			}
		}
	}
}