using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH2099
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
		}

		protected override void OnTearDown()
		{
		}

		//Note: If this test is failed on DB creation you need to adjust Dialect.UniqueIndexNameForDatabase
		[Test]
		public void BaseClassCanShareIndexNameWithSubclass()
		{
			var table = cfg.CreateMappings().IterateTables.FirstOrDefault(t => t.Name == "Level3");

			var index = table.GetIndex("CF_BASE_IDX");
			Assert.That(index?.ColumnSpan, Is.EqualTo(3));
			Assert.That(index.ColumnIterator.FirstOrDefault(c => c.Name == "PDO_Deleted"), Is.Not.Null);
		}
	}
}
