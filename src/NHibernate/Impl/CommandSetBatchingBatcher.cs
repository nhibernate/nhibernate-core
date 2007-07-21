#if NET_2_0

using System.Data;
using NHibernate.AdoNet;

namespace NHibernate.Impl
{
    internal abstract class CommandSetBatchingBatcher : BatcherImpl
    {
        private int batchSize;
        private int totalExpectedRowsAffected;
        private IDbCommandSet currentBatch;

        protected CommandSetBatchingBatcher(ConnectionManager connectionManager)
            : base(connectionManager)
        {
            batchSize = Factory.BatchSize;
        }

        protected abstract IDbCommandSet CreateCommandSet();

        private IDbCommandSet CurrentBatch
        {
            get
            {
                if (currentBatch == null)
                    currentBatch = CreateCommandSet();
                return currentBatch;
            }
        }

        public override void AddToBatch(IExpectation expectation)
        {
            totalExpectedRowsAffected += expectation.ExpectedRowCount;
            log.Debug("Adding to batch:");
            IDbCommand batchUpdate = CurrentCommand;
            LogCommand(batchUpdate);

            CurrentBatch.Append(batchUpdate);
            if (CurrentBatch.CountOfCommands >= batchSize)
            {
                DoExecuteBatch(batchUpdate);
            }
        }

        protected override void DoExecuteBatch(IDbCommand ps)
        {
            log.Debug("Executing batch");
            CheckReaders();
            Prepare(CurrentBatch.BatchCommand);
            int rowsAffected = CurrentBatch.ExecuteNonQuery();

            Expectations.VerifyOutcomeBatched(totalExpectedRowsAffected, rowsAffected);

            CurrentBatch.Dispose();
            totalExpectedRowsAffected = 0;
            currentBatch = null;
        }

        protected override void Dispose(bool disposing)
        {
            if (currentBatch != null)
            {
                currentBatch.Dispose();
                currentBatch = null;
            }
            base.Dispose(disposing);
        }
    }
}

#endif