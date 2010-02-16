using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1981
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		public void CanGroupWithParameter()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Save(new Article() { Longitude = 90 });
				s.Save(new Article() { Longitude = 90 });
				s.Save(new Article() { Longitude = 120 });

				IList<double> quotients =
					s.CreateQuery(
						@"select (Longitude / :divisor)
						from Article
						group by (Longitude / :divisor)")
					.SetDouble("divisor", 30)
					.List<double>();

				Assert.That(quotients.Count, Is.EqualTo(2));
				Assert.That(quotients[0], Is.EqualTo(3));
				Assert.That(quotients[1], Is.EqualTo(4));
			}
		}
	}
}
