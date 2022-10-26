using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3079
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.CreateQuery("delete from Employment").ExecuteUpdate();

				s.CreateQuery("delete from System.Object").ExecuteUpdate();

				t.Commit();
			}
		}

		protected override void OnSetUp()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var personList = new List<Person>();
				var employerList = new List<Employer>();

				// Global id to avoid false positive assertion with positional parameter
				var gId = 1;

				for (var i = 0; i < 3; i++)
				{
					var personCpId = new PersonCpId { IdA = gId++, IdB = gId++ };
					var personObj = new Person
						{ CpId = personCpId, Name = "PERSON_" + personCpId.IdA + "_" + personCpId.IdB };
					s.Save(personObj);
					personList.Add(personObj);
				}

				for (var i = 0; i < 3; i++)
				{
					var employerCpId = new EmployerCpId { IdA = gId++, IdB = gId++ };
					var employerObj = new Employer
						{ CpId = employerCpId, Name = "EMPLOYER_" + employerCpId.IdA + "_" + employerCpId.IdB };
					s.Save(employerObj);
					employerList.Add(employerObj);
				}

				var employmentIds = new[]
				{
					gId++,
					gId++,
					gId++,
					gId++,
				};
				var employmentNames = new[]
				{
					//P1 + E1
					"EMPLOYMENT_" + employmentIds[0] + "_" +
					personList[0].CpId.IdA + "_" + personList[0].CpId.IdA + "_" +
					employerList[0].CpId.IdA + "_" + employerList[0].CpId.IdB,
					//P1 + E2
					"EMPLOYMENT_" + employmentIds[1] + "_" +
					personList[0].CpId.IdA + "_" + personList[0].CpId.IdA + "_" +
					employerList[1].CpId.IdA + "_" + employerList[1].CpId.IdB,
					//P2 + E2
					"EMPLOYMENT_" + employmentIds[2] + "_" +
					personList[1].CpId.IdA + "_" + personList[1].CpId.IdA + "_" +
					employerList[1].CpId.IdA + "_" + employerList[1].CpId.IdB,
					//P2 + E3
					"EMPLOYMENT_" + employmentIds[2] + "_" +
					personList[1].CpId.IdA + "_" + personList[1].CpId.IdA + "_" +
					employerList[2].CpId.IdA + "_" + employerList[2].CpId.IdB
				};
				var employmentPersons = new[]
				{
					personList[0],
					personList[0],
					personList[1],
					personList[1]
				};
				var employmentEmployers = new[]
				{
					employerList[0],
					employerList[1],
					employerList[1],
					employerList[2]
				};

				for (var k = 0; k < employmentIds.Length; k++)
				{
					var employmentCpId = new EmploymentCpId
					{
						Id = employmentIds[k],
						PersonObj = employmentPersons[k],
						EmployerObj = employmentEmployers[k]
					};
					var employmentObj = new Employment { CpId = employmentCpId, Name = employmentNames[k] };
					s.Save(employmentObj);
				}

				for (var i = 0; i < 3; i++)
				{
					var personNoComponentObj = new PersonNoComponent { IdA = gId++, IdB = gId++ };
					personNoComponentObj.Name = "PERSON_NO_COMPONENT_" + personNoComponentObj.IdA + "_" +
						personNoComponentObj.IdB;
					s.Save(personNoComponentObj);
				}

				t.Commit();
			}
		}

		// Test reproducing the problem.
		[Test]
		public void GetPersonTest()
		{
			using (var session = OpenSession())
			{
				var person1_2 = session.Get<Person>(new PersonCpId { IdA = 1, IdB = 2 });
				Assert.That(person1_2.Name, Is.EqualTo("PERSON_1_2"));
				Assert.That(
					person1_2.EmploymentList.Select(e => e.Name),
					Is.EquivalentTo(new[] { "EMPLOYMENT_13_1_1_7_8", "EMPLOYMENT_14_1_1_9_10" }));
			}
		}

		// Test reproducing the problem.
		[Test]
		public void GetEmployerTest()
		{
			using (var session = OpenSession())
			{
				var employer7_8 = session.Get<Employer>(new EmployerCpId { IdA = 7, IdB = 8 });
				Assert.That(employer7_8.Name, Is.EqualTo("EMPLOYER_7_8"));
				Assert.That(
					employer7_8.EmploymentList.Select(e => e.Name),
					Is.EquivalentTo(new[] { "EMPLOYMENT_13_1_1_7_8" }));
			}
		}

		[Test]
		public void GetEmploymentTest()
		{
			using (var session = OpenSession())
			{
				var employment_13_1_2_7_8 =
					session.Get<Employment>(
						new EmploymentCpId
						{
							Id = 13,
							PersonObj =
								new Person
								{
									CpId = new PersonCpId { IdA = 1, IdB = 2 }
								},
							EmployerObj =
								new Employer
								{
									CpId = new EmployerCpId { IdA = 7, IdB = 8 }
								}
						});
				Assert.That(employment_13_1_2_7_8.Name, Is.EqualTo("EMPLOYMENT_13_1_1_7_8"));
			}
		}

		[Test]
		public void HqlPersonPositional()
		{
			using (var session = OpenSession())
			{
				var personList =
					session
						.GetNamedQuery("personPositional")
						.SetParameter(0, new PersonCpId { IdA = 1, IdB = 2 })
						.SetParameter(1, new PersonCpId { IdA = 3, IdB = 4 })
						.List<Person>();
				Assert.That(
					personList.Select(e => e.Name),
					Is.EquivalentTo(new[] { "PERSON_1_2", "PERSON_3_4" }));
			}
		}

		[Test]
		public void HqlPersonNamed()
		{
			using (var session = OpenSession())
			{
				var personList =
					session
						.GetNamedQuery("personNamed")
						.SetParameter("id1", new PersonCpId { IdA = 1, IdB = 2 })
						.SetParameter("id2", new PersonCpId { IdA = 3, IdB = 4 })
						.List<Person>();
				Assert.That(
					personList.Select(e => e.Name),
					Is.EquivalentTo(new[] { "PERSON_1_2", "PERSON_3_4" }));
			}
		}

		[Test]
		public void GetPersonNoComponent()
		{
			using (var session = OpenSession())
			{
				var person17_18 =
					session.Get<PersonNoComponent>(new PersonNoComponent { IdA = 17, IdB = 18 });
				Assert.That(person17_18.Name, Is.EqualTo("PERSON_NO_COMPONENT_17_18"));
			}
		}

		[Test]
		public void SqlPersonNoComponent()
		{
			using (var session = OpenSession())
			{
				var personList =
					session
						.GetNamedQuery("personNoComponentSql")
						.SetParameter(0, new PersonNoComponent { IdA = 17, IdB = 18 })
						.SetParameter(1, new PersonNoComponent { IdA = 19, IdB = 20 })
						.List<PersonNoComponent>();
				Assert.That(
					personList.Select(e => e.Name),
					Is.EquivalentTo(new[] { "PERSON_NO_COMPONENT_17_18", "PERSON_NO_COMPONENT_19_20" }));
			}
		}
	}
}
