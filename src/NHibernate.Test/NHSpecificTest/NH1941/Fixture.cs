using System;
using NHibernate.Driver;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1941
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Delete("from Person");

				t.Commit();
			}
		}
		
		[Test]
		public void SaveCanOverrideStringEnumGetValue()
		{
			var paramPrefix = ((DriverBase) Sfi.ConnectionProvider.Driver).NamedPrefix;
			using (var ls = new SqlLogSpy())
			{
				using (var s = OpenSession())
				using (var t = s.BeginTransaction())
				{
					var person = new Person { Sex = Sex.Male };
					s.Save(person);

					t.Commit();
				}
				
				var log = ls.GetWholeLog();
				Assert.That(log.Contains(paramPrefix + "p0 = 'M'"), Is.True);
			}
		}

		[Test]
		public void ReadCanOverrideStringEnumGetValue()
		{
			var paramPrefix = ((DriverBase) Sfi.ConnectionProvider.Driver).NamedPrefix;
			using (var ls = new SqlLogSpy())
			{
				using (var s = OpenSession())
				using (s.BeginTransaction())
				{
					var person = s.CreateQuery("from Person p where p.Sex = :personSex")
						.SetParameter("personSex", Sex.Female)
						.UniqueResult<Person>();

					Assert.That(person, Is.Null);
				}
			   
				string log = ls.GetWholeLog();
				Assert.IsTrue(log.Contains(paramPrefix + "p0 = 'F'"));
			}
		}
	}
}
