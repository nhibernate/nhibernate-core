using System;
using System.Data;
using NHibernate.Engine;

namespace NHibernate.Impl {
	/// <summary>
	/// An implementation of the <c>IBatcher</c> inteface that does no batching
	/// </summary>
	public class NonBatchingBatcher : BatcherImpl {
		
		public NonBatchingBatcher(ISessionImplementor session) : base(session) { }

		public override void AddToBatch(int expectedRowCount) {
			int rowCount = GetStatement().ExecuteNonQuery();

			//negative expected row count means we don't know how many rows to expect
			if ( expectedRowCount>0 && expectedRowCount!=rowCount )
				throw new HibernateException("SQL update or deletion failed (row not found)");
		}

		protected override void DoExecuteBatch(IDbCommand ps) {
		}

	}
}
