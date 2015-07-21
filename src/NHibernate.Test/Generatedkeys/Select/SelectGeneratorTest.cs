using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.Generatedkeys.Select
{
	[TestFixture]
	public class SelectGeneratorTest: TestCase
	{
		protected override IList Mappings
		{
			get { return new[] { "Generatedkeys.Select.MyEntity.hbm.xml" }; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is Dialect.FirebirdDialect || dialect is Dialect.Oracle8iDialect;
		}

		[Test]
		public void GetGeneratedKeysSupport()
		{
			ISession session = OpenSession();
			session.BeginTransaction();

			MyEntity e = new MyEntity("entity-1");
			session.Save(e);

			// this insert should happen immediately!
			Assert.AreEqual(1, e.Id, "id not generated through forced insertion");

			session.Delete(e);
			session.Transaction.Commit();
			session.Close();
		}

	}
}