using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;
using NHibernate.Type;
using System;

namespace NHibernate.Test.NHSpecificTest.NH3963
{
	[TestFixture]
	public class MappedAsFixture : BugTestCase
	{
		[Test]
		public void ShouldThrowExplicitErrorOnInvalidUsage()
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.Query<Entity>()
					.Where(e => e.Name.MappedAs(NHibernateUtil.AnsiString) == "Bob");

				Assert.Throws<HibernateException>(() => { result.ToList(); });
			}
		}

		[Test]
		public void ShouldThrowExplicitErrorOnUnsupportedTypeUsage()
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.Query<Entity>()
					.Where(e => e.Name == "Bob".MappedAs(e.Id == Guid.Empty ? (IType)NHibernateUtil.AnsiString : NHibernateUtil.StringClob));

				Assert.Throws<HibernateException>(() => { result.ToList(); });
			}
		}
	}
}