using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1775
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new[] { "NHSpecificTest.NH1775.Member.hbm.xml" }; }
		}

		[Test]
		public void BitwiseOperationsShouldBeSupported()
		{
			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					Member m = new Member {FirstName = "Bob", LastName = "One", Roles = 1};
					s.Save(m);

					m = new Member { FirstName = "Bob", LastName = "Two", Roles = 2 };
					s.Save(m);

					m = new Member { FirstName = "Bob", LastName = "Four", Roles = 4 };
					s.Save(m);

					m = new Member { FirstName = "Bob", LastName = "OneAndFour", Roles = 5 };
					s.Save(m);
					tx.Commit();
				}
			}

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
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
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Delete("from Member");
				tx.Commit();
			}
		}
	}
}
