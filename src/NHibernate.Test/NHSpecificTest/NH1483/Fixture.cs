using System;
using System.Data;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1483
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnTearDown()
		{
			DeleteAll(true);
		}

		/// <summary>
		/// Tests that a Subclass can be loaded from second level cache as the specified 
		/// type of baseclass
		/// </summary>
		/// <typeparam name="TBaseClass">The type of the BaseClass to test.</typeparam>
		public void TestLoadFromSecondLevelCache<TBaseClass>() where TBaseClass : BaseClass
		{
			//create a new persistent entity to work with
			Guid id = CreateAndSaveNewSubclass().Id;

			using (ISession session = OpenSession())
			{
				//make sure the entity can be pulled
				TBaseClass entity = session.Get<TBaseClass>(id);
				Assert.IsNotNull(entity);
			}

			//delete the subclass so we know we will be getting
			//it from the second level cache
			DeleteAll(false);

			using (ISession session = OpenSession())
			{
				//reload the subclass, this should pull it directly from cache
				TBaseClass restoredEntity = session.Get<TBaseClass>(id);

				Assert.IsNotNull(restoredEntity);
			}
		}

		/// <summary>
		/// Creates and save a new subclass to the database.
		/// </summary>
		/// <returns>the new persistent SubClass</returns>
		private SubClass CreateAndSaveNewSubclass()
		{
			using (ISession session = OpenSession())
			{
				using (ITransaction trans = session.BeginTransaction())
				{
					SubClass entity = new SubClass();
					session.Save(entity);
					trans.Commit();

					return entity;
				}
			}
		}

		/// <summary>
		/// Deletes all the baseclass entities from the persistence medium
		/// </summary>
		/// <param name="inNHibernateScope">whether to delete the entities though NHibernate
		/// scope our outside of the scope so that entities will still remain in the session cache</param>
		private void DeleteAll(bool inNHibernateScope)
		{
			using (ISession session = OpenSession())
			{
				if (inNHibernateScope)
				{
					using (ITransaction trans = session.BeginTransaction())
					{
						session.Delete("from BaseClass");
						trans.Commit();
					}
				}
				else
				{
					//delete directly from the db
					using (IDbCommand cmd = session.Connection.CreateCommand())
					{
						cmd.CommandText = "DELETE FROM BaseClass";
						cmd.ExecuteNonQuery();
					}
				}
			}
		}

		/// <summary>
		/// Verifies that a subclass can be loaded from the second level cache
		/// </summary>
		[Test]
		public void LoadSubclassFromSecondLevelCache()
		{
			TestLoadFromSecondLevelCache<SubClass>();
		}

		/// <summary>
		/// Verifies that a subclass can be loaded from the second level cache
		/// </summary>
		[Test]
		public void LoadSubclassFromSecondLevelCacheAsBaseClass()
		{
			TestLoadFromSecondLevelCache<BaseClass>();
		}
	}
}