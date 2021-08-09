using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH2856
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (var s = Sfi.OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Save(new Entity {Name = "Company", Phone = new PhoneNumber("745-555-1234"),});
				s.Save(new Entity {Name = "Bob", Phone = new PhoneNumber("745-555-1234", "x123"),});
				s.Save(new Entity {Name = "Jane", Phone = new PhoneNumber("745-555-1235"),});
				s.Flush();
				t.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var s = Sfi.OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Delete("from Entity");
				s.Flush();
				t.Commit();
			}
		}

		[Test]
		public void CanDistinctOnCompositeUserType()
		{
			using (var s = OpenSession())
			{
				var numbers = s.Query<Entity>().Select(x => x.Phone.Ext).Distinct().ToList();
				Assert.That(numbers.Count, Is.EqualTo(2));
			}
		}
	}
}
