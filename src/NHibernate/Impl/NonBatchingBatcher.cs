using System.Data;
using NHibernate.Engine;

namespace NHibernate.Impl
{
	/// <summary>
	/// An implementation of the <see cref="IBatcher" /> 
	/// interface that does no batching.
	/// </summary>
	internal class NonBatchingBatcher : BatcherImpl
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="NonBatchingBatcher"/> class.
		/// </summary>
		/// <param name="session">The <see cref="ISessionImplementor"/> the Batcher is in.</param>
		public NonBatchingBatcher( ISessionImplementor session ) : base( session )
		{
		}

		/// <summary>
		/// Executes the current <see cref="IDbCommand"/> and compares the row Count
		/// to the <c>expectedRowCount</c>.
		/// </summary>
		/// <param name="expectedRowCount">
		/// The expected number of rows affected by the query.  A value of less than <c>0</c>
		/// indicates that the number of rows to expect is unknown or should not be a factor.
		/// </param>
		/// <exception cref="HibernateException">
		/// Thrown when there is an expected number of rows to be affected and the
		/// actual number of rows is different.
		/// </exception>
		public override void AddToBatch( int expectedRowCount )
		{
			int rowCount = this.ExecuteNonQuery( this.CurrentCommand );

			//negative expected row count means we don't know how many rows to expect
			if( expectedRowCount > 0 && expectedRowCount != rowCount )
			{
				throw new HibernateException( "SQL update or deletion failed (row not found)" );
			}
		}

		/// <summary>
		/// This Batcher implementation does not support batching so this is a no-op call.  The
		/// actual execution of the <see cref="IDbCommand"/> is run in the <c>AddToBatch</c> 
		/// method.
		/// </summary>
		/// <param name="ps"></param>
		protected override void DoExecuteBatch( IDbCommand ps )
		{
		}

	}
}