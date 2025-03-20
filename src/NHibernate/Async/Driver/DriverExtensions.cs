﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Data;
using System.Data.Common;
using NHibernate.AdoNet;
using NHibernate.SqlTypes;
using NHibernate.Util;

namespace NHibernate.Driver
{
	using System.Threading.Tasks;
	using System.Threading;
	public static partial class DriverExtensions
	{

		// 6.0 TODO: merge into IDriver
		/// <summary>
		/// Begin an ADO <see cref="DbTransaction" />.
		/// </summary>
		/// <param name="driver">The driver.</param>
		/// <param name="isolationLevel">The isolation level requested for the transaction.</param>
		/// <param name="connection">The connection on which to start the transaction.</param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		/// <returns>The started <see cref="DbTransaction" />.</returns>
		public static async Task<DbTransaction> BeginTransactionAsync(this IDriver driver, IsolationLevel isolationLevel, DbConnection connection, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (driver is DriverBase driverBase)
			{
				return await (driverBase.BeginTransactionAsync(isolationLevel, connection, cancellationToken)).ConfigureAwait(false);
			}

			// So long for custom drivers not deriving from DriverBase, they will have to wait for 6.0 if they
			// need the feature.
			if (isolationLevel == IsolationLevel.Unspecified)
			{
				return await (connection.BeginTransactionAsync(cancellationToken)).ConfigureAwait(false);
			}
			return await (connection.BeginTransactionAsync(isolationLevel, cancellationToken)).ConfigureAwait(false);
		}
	}
}
