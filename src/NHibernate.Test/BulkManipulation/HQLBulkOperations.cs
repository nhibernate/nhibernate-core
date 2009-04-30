using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace NHibernate.Test.BulkManipulation
{
	[TestFixture]
	public class HqlBulkOperations: BaseFixture
	{
		[Test, Ignore("Not supported yet.")]
		public void SimpleDelete()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.Save(new SimpleClass {Description = "simple1"});
				s.Save(new SimpleClass {Description = "simple2"});
				tx.Commit();
			}

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.CreateQuery("delete from SimpleClass").ExecuteUpdate();
				tx.Commit();
			}

			using (var s = OpenSession())
			{
				var l = s.CreateQuery("from SimpleClass").List();
				Assert.That(l.Count, Is.EqualTo(0));
			}
		}
	}
}