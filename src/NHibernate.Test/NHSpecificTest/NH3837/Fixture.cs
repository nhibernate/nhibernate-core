using System;
using System.IO;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3837
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void Configure(Cfg.Configuration configuration)
		{
			cfg.SetProperty("cache.use_second_level_cache", "true");
		}

		protected override string CacheConcurrencyStrategy => null;

		[Test]
		public void AfterInsertingEntitiesAndCollectionNoSQLSelectShouldHaveBeenIssued()
		{
			var item_guid = Guid.NewGuid();

			var bid1_guid = Guid.NewGuid();
			var bid2_guid = Guid.NewGuid();

			using (ISession session = OpenSession())
			{
				using (var tx = session.BeginTransaction())
				{
					// item
					var item = new Item
					{
						Id = item_guid, 
						Description = "ItemNote"
					};

					// bid1
					var bid1 = new Bid
					{
						Id = bid1_guid,
						Description = "Bid1Description", Item = item
					};

					// bid2
					var bid2 = new Bid
					{
						Id = bid2_guid,
						Description = "Bid2Description", 
						Item = item
					};

					item.AddBid(bid1);
					item.AddBid(bid2);

					session.Save(item);

					tx.Commit();
				}
			}

			using (ISession session = OpenSession())
			{
				using (var tx = session.BeginTransaction())
				{
					using (var sqlLogSpy = new SqlLogSpy())
					{
						session.Get<Item>(item_guid);
						tx.Commit();

						Assert.AreEqual(
							0,
							GetNumberOfSelectStatements(sqlLogSpy),
							"No SQL select statements should have been issued");
					}
				}
			}
		}

		[Test]
		public void AfterInsertingAndUpdateEntitiesAndCollectionNoSQLSelectShouldHaveBeenIssued()
		{
			var item_guid = Guid.NewGuid();

			using (ISession session = OpenSession())
			{
				using (var tx = session.BeginTransaction())
				{
					// item
					var item = new Item
					{
						Id = item_guid, 
						Description = "ItemNote"
					};

					// bid1
					var bid1 = new Bid
					{
						Id = Guid.NewGuid()
					};
					;
					bid1.Description = "Bid1Description";
					bid1.Item = item;

					// bid2
					var bid2 = new Bid
					{
						Id = Guid.NewGuid(),
						Description = "Bid2Description", 
						Item = item
					};

					item.AddBid(bid1);
					item.AddBid(bid2);

					session.Save(item);

					tx.Commit();
				}
			}

			using (ISession session = OpenSession())
			{
				using (var tx = session.BeginTransaction())
				{
					using (var sqlLogSpy = new SqlLogSpy())
					{
						var item = session.Get<Item>(item_guid);

						// bid3
						var bid3 = new Bid
						{
							Id = Guid.NewGuid(), 
							Description = "Bid3Description", 
							Item = item
						};

						item.AddBid(bid3);
						tx.Commit();

						Assert.AreEqual(
							0,
							GetNumberOfSelectStatements(sqlLogSpy),
							"No SQL select statements should have been issued");
					}
				}
			}
		}

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				session.Delete("from Item");
				session.Delete("from Bid");
				tx.Commit();
			}
		}

		private static int GetNumberOfSelectStatements(SqlLogSpy sqlLogSpy)
		{
			int numberOfSelectStatements = 0;
			
			using (var wr = new StringWriter())
			{
				foreach (var sqlEvent in sqlLogSpy.Appender.GetEvents())
				{
					sqlEvent.WriteRenderedMessage(wr);
					if (wr.ToString().TrimStart().StartsWith("SELECT"))
					{
						numberOfSelectStatements++;
					}
				}
			}

			return numberOfSelectStatements;
		}
	}
}
