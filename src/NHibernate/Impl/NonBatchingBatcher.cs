using System.Data;
using NHibernate.Engine;

namespace NHibernate.Impl
{
	/// <summary>
	/// An implementation of the <c>IBatcher</c> inteface that does no batching
	/// </summary>
	internal class NonBatchingBatcher : BatcherImpl
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		public NonBatchingBatcher( ISessionImplementor session ) : base( session )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="expectedRowCount"></param>
		public override void AddToBatch( int expectedRowCount )
		{
			int rowCount = this.ExecuteNonQuery( this.GetCommand() );

			//negative expected row count means we don't know how many rows to expect
			if( expectedRowCount > 0 && expectedRowCount != rowCount )
			{
				throw new HibernateException( "SQL update or deletion failed (row not found)" );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ps"></param>
		protected override void DoExecuteBatch( IDbCommand ps )
		{
		}

	}
}