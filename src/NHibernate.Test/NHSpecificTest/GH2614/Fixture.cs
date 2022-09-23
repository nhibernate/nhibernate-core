using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH2614
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Save(new ConcreteClass1 {Name = "C1"});
				s.Save(new ConcreteClass2 {Name = "C2"});
				t.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.CreateQuery("delete from System.Object").ExecuteUpdate();
				t.Commit();
			}
		}

		[Test]
		public void PolymorphicListReturnsCorrectResults()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var query = s.CreateQuery(
					@"SELECT Name FROM NHibernate.Test.NHSpecificTest.GH2614.BaseClass ROOT");
				query.SetMaxResults(5);
				var list = query.List();
				Assert.That(list.Count, Is.EqualTo(2));
			}
		}
	}
}
