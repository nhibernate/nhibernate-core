using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.Generatedkeys.ByTrigger
{
	[TestFixture]
	public class GeneratedIdentityFixture : TestCase
	{
		protected override IList Mappings
		{
			get { return new[] { "Generatedkeys.ByTrigger.MyEntity.hbm.xml" }; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is Dialect.Oracle8iDialect;
		}

		[Test]
		public void GetGeneratedKeysSupport()
		{
			ISession session = OpenSession();
			session.BeginTransaction();

			var e = new MyEntity { Name = "entity-1" };
			session.Save(e);

			// this insert should happen immediately!
			Assert.AreEqual(1, e.Id, "id not generated through forced insertion");

			session.Delete(e);
			session.Transaction.Commit();
			session.Close();
		}
	}
}