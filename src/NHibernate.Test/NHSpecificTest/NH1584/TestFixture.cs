/*
    The documentation for NHibernate likes to work with cats / kittens for examples or demonstrations.  
*/
using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1584
{
	[TestFixture]
	public class TestFixture : BugTestCase
	{
		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			{
				using (ITransaction trx = session.BeginTransaction())
				{
					session.Delete("from Male");
					trx.Commit();
				}
			}
		}

		/// <summary>
		/// Demonstrate that the session is able to load the one-to-one composition between a joined subclass and its related entity. 
		/// </summary>
		[Test]
		public void Load_One_To_One_Composition_For_Joined_Subclass_Succeeds()
		{
			var tabby = new Tabby {HasSpots = true, HasStripes = true, HasSwirls = false};

			var newInstance = new Male {Name = "Male", Coat = tabby};

			using (ISession session = OpenSession())
			{
				using (ITransaction trx = session.BeginTransaction())
				{
					session.Save(newInstance);
					trx.Commit();
				}
			}

			Assert.AreNotEqual(0, newInstance.Id);
			Assert.AreNotEqual(0, tabby.Id);

			using (ISession session = OpenSession())
			{
				ICriteria criteria = session.CreateCriteria(typeof (Cat));
				var loaded = criteria.Add(Restrictions.Eq("Id", newInstance.Id)).UniqueResult<Male>();

				Assert.IsNotNull(loaded.Coat);
			}
		}
	}
}