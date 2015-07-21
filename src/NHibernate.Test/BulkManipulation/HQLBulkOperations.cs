using NUnit.Framework;

namespace NHibernate.Test.BulkManipulation
{
	[TestFixture]
	public class HqlBulkOperations: BaseFixture
	{
		[Test]
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
				Assert.That(s.CreateQuery("delete from SimpleClass where Description = 'simple2'").ExecuteUpdate(),
					Is.EqualTo(1));
				tx.Commit();
			}

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				Assert.That(s.CreateQuery("delete from SimpleClass").ExecuteUpdate(),
					Is.EqualTo(1));
				tx.Commit();
			}
		}
	}
}