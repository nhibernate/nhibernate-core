using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using log4net.Appender;
using log4net.Repository.Hierarchy;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2459
{
	[TestFixture]
	public class Test : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (ISession session = OpenSession())
			{
				SkillSet skillSet = new SkillSet() { Code = "123", Title = "Skill Set" };
				Qualification qualification = new Qualification() { Code = "124", Title = "Qualification" };

				session.Save(skillSet);
				session.Save(qualification);
				session.Flush();
			}
		}

		[Test]
		public void IsTypeOperator()
		{
			using (ISession session = OpenSession())
			{
				//first query is OK
				IQueryable<TrainingComponent> query = session.Query<TrainingComponent>().Where(c => c is SkillSet);
				Assert.That(!query.ToList().Any(c => !(c is SkillSet)));

				//Second time round the a cached version of the SQL for the query is used BUT the type parameter is not updated... 
				query = session.Query<TrainingComponent>().Where(c => c is Qualification);
				Assert.That(!query.ToList().Any(c => !(c is Qualification)));
			}
		}

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			{
				session.Delete("from TrainingComponent");
				session.Flush();
			}
		}
	}
}