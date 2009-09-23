using System;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1941
{
	[TestFixture]
	public class Fixture : BugTestCase
	{

		protected override void OnTearDown()
		{
			base.OnTearDown();
			using (ISession s = OpenSession())
			{
				s.Delete("from Person");
			}
		}
		[Test]
		public void CanOverrideStringEnumGetValue()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				using (SqlLogSpy ls = new SqlLogSpy())
				{
					Person person = new Person() { Sex = Sex.Male };
					s.Save(person);

					string log = ls.GetWholeLog();
					Assert.IsTrue(log.Contains("@p0 = 'M'"));
				}

				using (SqlLogSpy ls = new SqlLogSpy())
				{
					Person person =
						s.CreateQuery("from Person p where p.Sex = :personSex")
							.SetParameter("personSex", Sex.Female)
							.UniqueResult<Person>();

					Assert.That(person, Is.Null);

					string log = ls.GetWholeLog();
					Assert.IsTrue(log.Contains("@p0 = 'F'"));
				}

				tx.Rollback();
			}
		}
	}
}
