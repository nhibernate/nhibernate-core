using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1677
{
	[TestFixture,Ignore]
	public class EntityModeMapCriteria:BugTestCase
	{
		private const int NumberOfRecordPerEntity = 10;
		private const string Entity1Name = "Entity1";
		private const string Entity1Property = "Entity1Property";
		private const string Entity2Name = "Entity2";
		private const string Entity2Property = "Entity2Property";
		private const string EntityPropertyPrefix = "Record";

		protected override void Configure(NHibernate.Cfg.Configuration configuration)
		{
			base.Configure(configuration);
			configuration.SetProperty("default_entity_mode",
			                          EntityModeHelper.ToString(EntityMode.Map));
		}

		protected override void OnSetUp()
		{
			using (var s = OpenSession())
			{
				using (var tx = s.BeginTransaction(IsolationLevel.ReadCommitted))
				{
					for (int i = 0; i < NumberOfRecordPerEntity; i++)
					{
						var entity1 = new Hashtable();
						entity1[Entity1Property] = EntityPropertyPrefix + i;
						s.SaveOrUpdate(Entity1Name, entity1);
					}

					for (int i = 0; i < NumberOfRecordPerEntity; i++)
					{
						var entity2 = new Hashtable();
						entity2[Entity2Property] = EntityPropertyPrefix + i;
						s.SaveOrUpdate(Entity2Name, entity2);
					}
					tx.Commit();
				}
			}
		}
		protected override void OnTearDown()
		{
			using(var s=OpenSession())
			{
				using(var tx=s.BeginTransaction())
				{
					s.Delete(string.Format("from {0}", Entity1Name));
					tx.Commit();
				}

			}
		}
		[Test]
		public void EntityModeMapFailsWithCriteria()
		{
			using (var sf = cfg.BuildSessionFactory())
			{

				using (var s = sf.OpenSession())
				{
					var query = s.CreateQuery(string.Format("from {0}", Entity1Name));
					var entity1List = query.List();
					Assert.AreEqual(NumberOfRecordPerEntity, entity1List.Count);  // OK, Count == 10
				}

				using (var s = sf.OpenSession())
				{
					var entity1Criteria = s.CreateCriteria(Entity1Name);
					var entity1List = entity1Criteria.List();
					Assert.AreEqual(NumberOfRecordPerEntity, entity1List.Count);  // KO !!! Count == 20 !!!
				}
			}
		}
	}

}
