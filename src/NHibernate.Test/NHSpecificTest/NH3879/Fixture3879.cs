using System;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3879
{
	[TestFixture]
	public class Fixture3879 : BugTestCase
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect.SupportsSequences;
		}

		[Test]
		public void SeqhiloUsesFirstValidId()
		{
			//the first value of a sequence changes between the different databases (it can be 0 or 1)
			//but the bug associated to this test happens only when it is different from zero so 
			//we explicitly increment the sequence to ensure it
			long previousSequenceValue;
			using (var session = OpenSession())
			{
				var getSequenceSql = Dialect.GetSequenceNextValString("seqhiloEntity_ids");
				//some adonet providers return an int, others a long
				var boxedPreviousSequenceValue = session.CreateSQLQuery(getSequenceSql).UniqueResult();
				previousSequenceValue = Convert.ToInt64(boxedPreviousSequenceValue);
			}

			long id;
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				var entity = new SeqhiloEntity();
				session.Save(entity);
				tx.Commit();
				id = entity.Id;
			}

			var expectedId = (previousSequenceValue + 1) * 10; //max_lo (9) + 1
			Assert.That(id, Is.EqualTo(expectedId));
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				session.CreateQuery("delete from SeqhiloEntity").ExecuteUpdate();
				tx.Commit();
			}
		}
	}
}
