using System;
using System.Data;
using NHibernate.Driver;

namespace NHibernate.AdoNet
{
	public interface IExpectation
	{
		void VerifyOutcomeNonBatched(int rowCount, IDbCommand statement);
		bool CanBeBatched { get; }
		
		/// <summary>
		/// Expected row count. Valid only for batchable expectations.
		/// </summary>
		int ExpectedRowCount { get; }
	}
}
