using System;
using System.Data;
using NHibernate.Engine;

namespace NHibernate.Impl
{
	/// <summary>
	/// Summary description for BatchingBatcher.
	/// </summary>
	internal class BatchingBatcher : BatcherImpl
	{
		private int batchSize;
		private int[] expectedRowCounts;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		public BatchingBatcher( ISessionImplementor session ) : base( session )
		{
			expectedRowCounts = new int[ Factory.BatchSize ];
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="expectedRowCount"></param>
		public override void AddToBatch( int expectedRowCount )
		{
			throw new NotImplementedException( "Batching not implemented yet" );

			/*
			log.Info( "Adding to batch" );
			IDbCommand batchUpdate = CurrentStatment;
			*/
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ps"></param>
		protected override void DoExecuteBatch( IDbCommand ps )
		{
			throw new NotImplementedException( "Batching not implemented yet" );
		}
	}
}
