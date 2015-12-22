using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NHibernate.Dialect;
using NUnit.Framework;
using NHibernate.Test.NHSpecificTest;
using Iesi.Collections.Generic;
using NHibernate;
using System.Data;
using NHibernate.Criterion;

namespace NHibernate.Test.NHSpecificTest.NH3079
{
	/// <summary>
	/// <para>
	/// </para>
	/// </remarks>
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void Configure(NHibernate.Cfg.Configuration configuration)
		{
			#region removing possible second-level-cache configs
			configuration.Properties.Remove(NHibernate.Cfg.Environment.CacheProvider);
			configuration.Properties.Remove(NHibernate.Cfg.Environment.UseQueryCache);
			configuration.Properties.Add(NHibernate.Cfg.Environment.UseQueryCache, "true");
			configuration.Properties.Remove(NHibernate.Cfg.Environment.UseSecondLevelCache); 
			#endregion

			base.Configure(configuration);
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();
			using (ISession s = OpenSession())
			{
				int countUpdate = 0;

				countUpdate =
					s
						.CreateSQLQuery("DELETE FROM T_EMPLOYMENT")
						.ExecuteUpdate();
								Assert.AreEqual(4, countUpdate);

				countUpdate =
					s
						.CreateSQLQuery("DELETE FROM T_PERSON")
						.ExecuteUpdate();
				Assert.AreEqual(3, countUpdate);

				countUpdate =
					s
						.CreateSQLQuery("DELETE FROM T_EMPLOYER")
						.ExecuteUpdate();
				Assert.AreEqual(3, countUpdate);

				countUpdate =
					s
						.CreateSQLQuery("DELETE FROM T_PERSON_NO_COMP")
						.ExecuteUpdate();
				Assert.AreEqual(3, countUpdate);

				s.Flush();
			}
		}

		protected override void OnSetUp()
		{
			base.OnSetUp();

			using (ISession session = this.OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				List<Person> personList = new List<Person>();
				List<Employer> employerList = new List<Employer>();
				
				//Global id to avoid false positive assertion with positional parameter
				int gId = 1;

				//BEGIN: Person count: 3
				for (int i = 0; i < 3; i++)
				{
					PersonCpId personCpId = new PersonCpId() { IdA = gId++, IdB = gId++ };
					Person personObj = new Person() { CpId = personCpId, Name = "PERSON_" + personCpId.IdA + "_" + personCpId.IdB };
					session.Save(personObj);
					session.Flush();
					personList.Add(personObj);
				}
				//END: Person count: 3

				//BEGIN: Employer count: 3
				for (int i = 0; i < 3; i++)
				{
					EmployerCpId employerCpId = new EmployerCpId() { IdA = gId++, IdB = gId++ };
					Employer employerObj = new Employer() { CpId = employerCpId, Name = "EMPLOYER_" + employerCpId.IdA + "_" + employerCpId.IdB };
					session.Save(employerObj);
					session.Flush();
					employerList.Add(employerObj);
				}
				//END: Employer count: 3
				//BEGIN: employment count: 4
				int[] employmentIdAraay = new int[] { 
					gId++,                    
					gId++,
					gId++,
					gId++,
				};
				string[] employmentNameArray = new string[]
				{
					//P1 + E1
					"EMPLOYMENT_" + employmentIdAraay[0] + "_" + 
						personList[0].CpId.IdA + "_" + personList[0].CpId.IdA + "_" + 
						employerList[0].CpId.IdA + "_" + employerList[0].CpId.IdB,
					//P1 + E2
					"EMPLOYMENT_" + employmentIdAraay[1] + "_" + 
						personList[0].CpId.IdA + "_" + personList[0].CpId.IdA + "_" + 
						employerList[1].CpId.IdA + "_" + employerList[1].CpId.IdB,
					//P2 + E2
					"EMPLOYMENT_" + employmentIdAraay[2] + "_" + 
						personList[1].CpId.IdA + "_" + personList[1].CpId.IdA + "_" + 
						employerList[1].CpId.IdA + "_" + employerList[1].CpId.IdB,
					//P2 + E3
					"EMPLOYMENT_" + employmentIdAraay[2] + "_" + 
						personList[1].CpId.IdA + "_" + personList[1].CpId.IdA + "_" + 
						employerList[2].CpId.IdA + "_" + employerList[2].CpId.IdB
				};
				Person[] employemenPersonArray = new Person[] { 
					personList[0],
					personList[0],
					personList[1],
					personList[1]
				};
				Employer[] employemenEmployerArray = new Employer[] { 
					employerList[0],
					employerList[1],
					employerList[1],
					employerList[2]
				};
				//persinting Employment's
				for (int k = 0; k < employmentIdAraay.Length; k++)
				{
					EmploymentCpId employmentCpId = new EmploymentCpId() { 
						Id = employmentIdAraay [k],
						PersonObj = employemenPersonArray[k],
						EmployerObj = employemenEmployerArray[k]
					};
					Employment employmentObj = new Employment() { CpId = employmentCpId, Name = employmentNameArray[k] };
					session.Save(employmentObj);
					session.Flush();
				}
				//END: employment count: 4

				//BEGIN: PersonNoComponent count: 3
				for (int i = 0; i < 3; i++)
				{
					PersonNoComponent personNoComponentObj = new PersonNoComponent() { IdA = gId++, IdB = gId++};
					personNoComponentObj.Name = "PERSON_NO_COMPONENT_" + personNoComponentObj.IdA + "_" + personNoComponentObj.IdB;
					session.Save(personNoComponentObj);
					session.Flush();
				}
				//END: PersonNoComponent count: 3

				tx.Commit();
			}
		}

		protected override bool AppliesTo(global::NHibernate.Dialect.Dialect dialect)
		{
			//return dialect as MsSql2005Dialect != null;
			return base.AppliesTo(dialect);
		}

		/// <summary>
		/// Test that reproduces the problem.
		/// </summary>
		[Test]
		public void GetPersonTest()
		{
			using (ISession session = this.OpenSession())
			{
				Person person1_2 = session.Get<Person>(new PersonCpId() { IdA = 1, IdB = 2 });
				Assert.AreEqual("PERSON_1_2", person1_2.Name);
				String[] expetedArray = new String[] { "EMPLOYMENT_13_1_1_7_8", "EMPLOYMENT_14_1_1_9_10" };
				for (int i = 0; i < person1_2.EmploymentList.Count; i++)
				{
					Assert.AreEqual(expetedArray[i], new List<Employment>(person1_2.EmploymentList)[i].Name);
				}
			}
		}

		/// <summary>
		/// Test that reproduces the problem.
		/// </summary>
		[Test]
		public void GetEmployerTest()
		{
			using (ISession session = this.OpenSession())
			{
				Employer employer7_8 = session.Get<Employer>(new EmployerCpId() { IdA = 7, IdB = 8 });

				Assert.AreEqual("EMPLOYER_7_8", employer7_8.Name);
				String[] expetedArray = new String[] { "EMPLOYMENT_13_1_1_7_8", "EMPLOYMENT_13_1_1_7_8" };
				for (int i = 0; i < employer7_8.EmploymentList.Count; i++)
				{
					Assert.AreEqual(expetedArray[i], new List<Employment>(employer7_8.EmploymentList)[i].Name);
				}
			}

			using (ISession session = this.OpenSession())
			{
				Employer employer7_8 = session.Get<Employer>(new EmployerCpId() { IdA = 7, IdB = 8 });

				Assert.AreEqual("EMPLOYER_7_8", employer7_8.Name);
				String[] expetedArray = new String[] { "EMPLOYMENT_13_1_1_7_8", "EMPLOYMENT_13_1_1_7_8" };
				for (int i = 0; i < employer7_8.EmploymentList.Count; i++)
				{
					Assert.AreEqual(expetedArray[i], new List<Employment>(employer7_8.EmploymentList)[i].Name);
				}
			}
		}

		[Test]
		public void GetEmploymentTest()
		{
			using (ISession session = this.OpenSession())
			{
				Employment employment_13_1_2_7_8 =
					session.Get<Employment>(
						new EmploymentCpId()
						{
							Id = 13,
							PersonObj =
								new Person()
								{
									CpId = new PersonCpId() { IdA = 1, IdB = 2 }
								},
							EmployerObj =
								new Employer()
								{
									CpId = new EmployerCpId() { IdA = 7, IdB = 8 }
								}
						});
				Assert.AreEqual("EMPLOYMENT_13_1_1_7_8", employment_13_1_2_7_8.Name);
			}
		}

		/// <summary>
		/// Test that reproduces the problem.
		/// </summary>
		[Test]
		public void HQLpersonPositional()
		{
			using (ISession session = this.OpenSession())
			{
				IList<Person> personList =
					session
						.GetNamedQuery("personPositional")
						.SetParameter<PersonCpId>(0, new PersonCpId() { IdA = 1, IdB = 2 })
						.SetParameter<PersonCpId>(1, new PersonCpId() { IdA = 3, IdB = 4 })
						.List<Person>();
				String[] expetedArray = new String[] { "PERSON_1_2", "PERSON_3_4" };
				for (int i = 0; i < personList.Count; i++)
				{
					Assert.AreEqual(expetedArray[i], personList[i].Name);
				}
			}
		}

		[Test]
		public void HQLpersonNamed()
		{
			using (ISession session = this.OpenSession())
			{
				IList<Person> personList =
					session
						.GetNamedQuery("personNamed")
						.SetParameter<PersonCpId>("id1", new PersonCpId() { IdA = 1, IdB = 2 })
						.SetParameter<PersonCpId>("id2", new PersonCpId() { IdA = 3, IdB = 4 })
						.List<Person>();
				String[] expetedArray = new String[] { "PERSON_1_2", "PERSON_3_4" };
				for (int i = 0; i < personList.Count; i++)
				{
					Assert.AreEqual(expetedArray[i], personList[i].Name);
				}
			}
		}


		[Test]
		public void GetPersonNoComponent()
		{
			using (ISession session = this.OpenSession())
			{
				PersonNoComponent person17_18 = session.Get<PersonNoComponent>(new PersonNoComponent() { IdA = 17, IdB = 18 });
				Assert.AreEqual("PERSON_NO_COMPONENT_17_18", person17_18.Name);
			}
		}
	}
}