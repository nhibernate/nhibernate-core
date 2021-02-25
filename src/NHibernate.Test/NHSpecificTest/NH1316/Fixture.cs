using System;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1316
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is PostgreSQL81Dialect;
		}

		// create the trigger
		protected override void OnSetUp()
		{
			using (var s = OpenSession())
			{
				var command = s.Connection.CreateCommand();

				command.CommandText = "CREATE OR REPLACE FUNCTION audit_parent() RETURNS trigger AS $audit_parent$" +
				                      Environment.NewLine +
				                      "BEGIN" + Environment.NewLine +
									  "INSERT INTO parent_history SELECT nextval('parent_history_histid_seq'), now(), NEW.*;" +
				                      Environment.NewLine +
				                      "RETURN NEW;" + Environment.NewLine +
				                      "END" + Environment.NewLine +
				                      " $audit_parent$ LANGUAGE 'plpgsql';";
				command.ExecuteNonQuery();

				command.CommandText = "CREATE TRIGGER parent_audit" + Environment.NewLine +
				                      "AFTER INSERT OR UPDATE ON parent" + Environment.NewLine +
				                      "FOR EACH ROW EXECUTE PROCEDURE audit_parent();";
				command.ExecuteNonQuery();
			}
		}

		// remove trigger and remove data from tables
		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			{
				var command = s.Connection.CreateCommand();
				command.CommandText = "DROP FUNCTION audit_parent() CASCADE;";
				command.ExecuteNonQuery();
				command.CommandText = "DELETE from parent_history;";
				command.ExecuteNonQuery();
				command.CommandText = "DELETE from parent;";
				command.ExecuteNonQuery();
			}
		}

		[Test]
		public void Correct_Id_Returned_When_Using_Trigger()
		{
			//We expected this test to fail - if the problem has been fixed, clean-up the test.
			var entity1 = new Parent {Name = "Parent1_0"}; // when saved this entity should have the id of 1
			var entity2 = new Parent {Name = "Parent2_0"}; // when saved this entity should have the id of 2
			var entity3 = new Parent {Name = "Parent3_0"}; // when saved this entity should have the id of 3

			using (var s = OpenSession())
			{
				// save first entity
				s.Save(entity1);
				s.Flush();

				Assert.That(entity1.Id, Is.EqualTo(1));

				// save second entity
				s.Save(entity2);
				s.Flush();

				Assert.That(entity2.Id, Is.EqualTo(2));

				// update this entity 10 times - adds entries to the audit table 
				// causing the sequences for the parent and history table to no longer be aligned
				for (var i = 1; i < 11; i++)
				{
					entity2.Name = string.Format("Parent2_{0}", i);
					s.Update(entity2);
					s.Flush();
				}

				// save third entity
				s.Save(entity3);
				s.Flush();

				Assert.That(
					entity3.Id,
					Is.EqualTo(3),
					"oh uh - it would appear that lastval() is not our friend when a trigger updates other sequences.");
			}
		}
	}
}
