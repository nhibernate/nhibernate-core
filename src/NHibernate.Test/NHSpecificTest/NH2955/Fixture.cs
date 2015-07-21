using System.Linq;
using NUnit.Framework;
using NHibernate.Linq;

namespace NHibernate.Test.NHSpecificTest.NH2955
{
	/// <summary>
	/// Linq query using SQL IN statement (Contains) with System.Linq.IGrouping&lt;TKey, TElement&gt;
	/// </summary>
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var t = session.BeginTransaction())
			{
				var emp1 = new Employee { Id = 1, FirstName = "Nancy", LastName = "Davolio", Department = "IT"};
				var emp2 = new Employee { Id = 2, FirstName = "Andrew", LastName = "Fuller", Department = "Sales"};
				var emp3 = new Employee { Id = 3, FirstName = "Janet", LastName = "Leverling", Department = "IT"};
				var emp4 = new Employee { Id = 4, FirstName = "Margaret", LastName = "Peacock", Department = "IT"};
				var emp5 = new Employee { Id = 5, FirstName = "Steven", LastName = "Buchanan", Department = "Sales" };
				session.Save(emp1);
				session.Save(emp2);
				session.Save(emp3);
				session.Save(emp4);
				session.Save(emp5);
				t.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Delete("FROM Employee");
				t.Commit();
			}
		}
		
		[Test]
		public void EnumerableContains()
		{
// ReSharper disable RedundantEnumerableCastCall
			var array = new[] { 1, 3, 4 }.OfType<int>();
// ReSharper restore RedundantEnumerableCastCall

			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var firstNames = session.CreateQuery("select e.FirstName from Employee e where e.Id in (:x)")
					.SetParameterList("x", array)
					.List<string>();

				Assert.AreEqual(3, firstNames.Count);
				Assert.AreEqual("Nancy", firstNames[0]);
				Assert.AreEqual("Janet", firstNames[1]);
				Assert.AreEqual("Margaret", firstNames[2]);
			}
		}
		
		[Test]
		public void GroupingContains()
		{
			var array = new[] {1, 3, 4}.ToLookup(x => 1).Single();

			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var firstNames = session.CreateQuery("select e.FirstName from Employee e where e.Id in (:x)")
					.SetParameterList("x", array)
					.List<string>();

				Assert.AreEqual(3, firstNames.Count);
				Assert.AreEqual("Nancy", firstNames[0]);
				Assert.AreEqual("Janet", firstNames[1]);
				Assert.AreEqual("Margaret", firstNames[2]);
			}
		}
	}
}