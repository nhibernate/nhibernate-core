using System;
using System.Linq;
using NHibernate.Linq;
using NHibernate.Collection;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3480
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH3480"; }
		}

		protected override void OnSetUp()
		{
			using (ISession session = OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					var parent1 = new Entity { Id = new Entity.Key() { Id = Guid.NewGuid() }, Name = "Bob", OtherId = 20 };
					session.Save(parent1);

					var child1 = new Child { Name = "Bob1", Parent = parent1 };
					session.Save(child1);

					var child2 = new Child { Name = "Bob2", Parent = parent1 };
					session.Save(child2);

					session.Flush();
					transaction.Commit();
				}
			}
		}

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					session.Delete("from System.Object");

					session.Flush();
					transaction.Commit();
				}
			}
		}

		[Test]
		public void Test()
		{
			using (ISession session = OpenSession())
			{
				using (session.BeginTransaction())
				{
					var result = from e in session.Query<Entity>()
								 where e.Name == "Bob"
								 select e;
					var entity = result.Single();

					NHibernateUtil.Initialize(entity.Children);
				}
			}
		}
	}
}