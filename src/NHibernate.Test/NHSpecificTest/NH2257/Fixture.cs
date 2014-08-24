using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2257
{
	public class Fixture : BugTestCase
	{
		protected override bool AppliesTo(NHibernate.Dialect.Dialect dialect)
		{
			return (dialect is NHibernate.Dialect.InformixDialect1000);
		}

		[Test]
		public void InformixUsingDuplicateParameters()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Save(new Foo() { Name = "aa" });

				var list =
					session.CreateQuery("from Foo f where f.Name = :p1 and not f.Name <> :p1")
						.SetParameter("p1", "aa")
						.List<Foo>();

				Assert.That(list.Count, Is.EqualTo(1));
			}
		}
	}
}