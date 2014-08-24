using NHibernate.Cfg;
using NUnit.Framework;
using System.Collections;

namespace NHibernate.Test.Unionsubclass
{
	[TestFixture]
	public class DatabaseKeywordsFixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new string[] { "Unionsubclass.DatabaseKeyword.hbm.xml" }; }
		}

		protected override void Configure(Configuration configuration)
		{
			base.Configure(configuration);

			configuration.SetProperty(Environment.Hbm2ddlKeyWords, "auto-quote");
		}

		[Test]
		public void UnionSubClassQuotesReservedColumnNames()
		{
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Save(new DatabaseKeyword() { User = "user", View = "view", Table = "table", Create = "create" });

				t.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Delete("from DatabaseKeywordBase");

				t.Commit();
			}
		}
	}
}
