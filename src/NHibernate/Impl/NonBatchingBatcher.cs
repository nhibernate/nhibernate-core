using System;
using System.Data;
using NHibernate.Engine;
#region Hack transaction
using Tx = NHibernate.Transaction;
#endregion
namespace NHibernate.Impl 
{
	/// <summary>
	/// An implementation of the <c>IBatcher</c> inteface that does no batching
	/// </summary>
	public class NonBatchingBatcher : BatcherImpl {
		
		public NonBatchingBatcher(ISessionImplementor session) : base(session) { }

		public override void AddToBatch(int expectedRowCount) {

			IDbCommand s = GetStatement();

			#region Hack transaction: see remarks at Impl\SessionImpl
			// the actual problem is the TransactionManager approach.
			// Ideally it should register the current AdoTransaction at the Session instance.
			s.Transaction =  ((Tx.Transaction)((Impl.SessionImpl)session).Transaction).AdoTransaction;
			#endregion

			int rowCount = s.ExecuteNonQuery();

			//negative expected row count means we don't know how many rows to expect
			if ( expectedRowCount>0 && expectedRowCount!=rowCount )
				throw new HibernateException("SQL update or deletion failed (row not found)");
		}

		protected override void DoExecuteBatch(IDbCommand ps) {
		}

	}
}
