using System.Collections.Generic;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1775
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return !(dialect is Oracle8iDialect);
		}

		protected override void OnSetUp()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.Save(new Member { FirstName = "Bob", LastName = "One", Roles = 1 });
				s.Save(new Member { FirstName = "Bob", LastName = "Two", Roles = 2 });
				s.Save(new Member { FirstName = "Bob", LastName = "Four", Roles = 4 });
				s.Save(new Member { FirstName = "Bob", LastName = "OneAndFour", Roles = 5 });

				tx.Commit();
			}
		}

		[Test]
		public void BitwiseOperationsShouldBeSupported()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				// &
				IList<DTO> result = s.CreateQuery("select new DTO(m.Id, concat(m.FirstName, ' ', m.LastName)) from Member m where (m.Roles & :roles) = :roles")
					.SetInt32("roles", 1)
					.List<DTO>();

				Assert.AreEqual(2, result.Count);
				Assert.IsTrue(
								(result[0].Name == "Bob One" && result[1].Name == "Bob OneAndFour")
								||
								(result[0].Name == "Bob OneAndFour" && result[1].Name == "Bob One")
							);

				// |
				result = s.CreateQuery("select new DTO(m.Id, concat(m.FirstName, ' ', m.LastName)) from Member m where (m.Roles & (:firstRole | :secondRole)) = (:firstRole | :secondRole)")
					.SetInt32("firstRole", 1)
					.SetInt32("secondRole", 4)
					.List<DTO>();

				Assert.AreEqual(1, result.Count);
				Assert.AreEqual("Bob OneAndFour", result[0].Name);

				// !
				result = s.CreateQuery("select new DTO(m.Id, concat(m.FirstName, ' ', m.LastName)) from Member m where (m.Roles & (!(!:roles))) = :roles")
					.SetInt32("roles", 1)
					.List<DTO>();

				Assert.AreEqual(2, result.Count);
				Assert.IsTrue(
								(result[0].Name == "Bob One" && result[1].Name == "Bob OneAndFour")
								||
								(result[0].Name == "Bob OneAndFour" && result[1].Name == "Bob One")
							);

				tx.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.Delete("from Member");
				tx.Commit();
			}
		}
	}
}
