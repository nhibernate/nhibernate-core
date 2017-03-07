using System.Collections;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.VersionTest.Db.MsSQL
{
	[TestFixture]
	public class LazyVersionTest : TestCase
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is MsSql2000Dialect;
		}

		protected override IList Mappings
		{
			get { return new[] { "VersionTest.Db.MsSQL.ProductWithVersionAndLazyProperty.hbm.xml" }; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		[Test(Description = "NH-3589")]
		public void CanUseVersionOnEntityWithLazyProperty()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				session.Save(new ProductWithVersionAndLazyProperty { Id = 1, Summary = "Testing, 1, 2, 3" });

				session.Flush();

				session.Clear();

				var p = session.Get<ProductWithVersionAndLazyProperty>(1);

				p.Summary += ", 4!";

				session.Flush();
			}
		}
	}
}
