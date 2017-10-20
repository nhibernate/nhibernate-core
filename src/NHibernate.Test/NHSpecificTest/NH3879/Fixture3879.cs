using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3879
{
    [TestFixture]
    public class Fixture3879 : BugTestCase
    {
        public override string BugNumber
        {
            get
            {
                return "NH3879";
            }
        }

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
                string getSequenceSql = Dialect.GetSequenceNextValString("seqhiloEntity_ids");
				//some adonet providers return an int, others a long
				var boxedPreviousSequenceValue = session.CreateSQLQuery(getSequenceSql).UniqueResult();
				previousSequenceValue = (boxedPreviousSequenceValue is long longValue) ? (int)longValue : (int)boxedPreviousSequenceValue;
			}

            long id;
            using (var session = OpenSession())
            {
                var entity = new SeqhiloEntity();
                session.Save(entity);
                session.Flush();
                id = entity.Id;
            }

            try
            {
                long expectedId = (previousSequenceValue + 1) * 10; //max_lo (9) + 1
                Assert.AreEqual(expectedId, id);
            }
            finally
            {
                using (var session = OpenSession())
                {
                    var entity = session.Load<SeqhiloEntity>(id);
                    session.Delete(entity);
                    session.Flush();
                }
            }
        }
    }
}
