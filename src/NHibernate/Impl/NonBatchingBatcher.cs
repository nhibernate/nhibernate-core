using System;
using System.Data;
using NHibernate.Engine;
namespace NHibernate.Impl 
{
	/// <summary>
	/// An implementation of the <c>IBatcher</c> inteface that does no batching
	/// </summary>
	internal class NonBatchingBatcher : BatcherImpl {
		
		public NonBatchingBatcher(ISessionImplementor session) : base(session) { }

		public override void AddToBatch(int expectedRowCount) {

			IDbCommand command = GetStatement();

			// this should not be needed because all commands added to the Batcher should 
			// have their Transaction property set.
			// Hack: join transaction
			//Impl.AdoHack.JoinTx(s);
			// end-of Hack

			// verify that the commands IDbTransaction and the Session.Transaction.IDbTransaction are the 
			// same object
			// TODO: remove this copy and pasted code (in BatcherImpl.JoinTransaction)
			IDbTransaction sessionAdoTrx = null;
			if(this.session.Transaction!=null) sessionAdoTrx = ((Transaction.Transaction)session.Transaction).AdoTransaction;

			if(command.Transaction!= sessionAdoTrx) 
				throw new HibernateException("In NonBatchingBatcher.AddToBatch there is a Command's IDbTransaction that is different than the Session.Transaction.AdoTransaction");


			int rowCount = command.ExecuteNonQuery();

			//negative expected row count means we don't know how many rows to expect
			if ( expectedRowCount>0 && expectedRowCount!=rowCount )
				throw new HibernateException("SQL update or deletion failed (row not found)");
		}

		protected override void DoExecuteBatch(IDbCommand ps) {
		}

	}
}
