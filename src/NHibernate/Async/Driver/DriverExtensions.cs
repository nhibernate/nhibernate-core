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
		/// Executes the command and returns a <see cref="DbDataReader"/>.
		/// </summary>
		/// <param name="driver">The driver.</param>
		/// <param name="command">The command to execute.</param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		/// <returns>A DbDataReader</returns>
		public static Task<DbDataReader> ExecuteReaderAsync(this IDriver driver, DbCommand command, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<DbDataReader>(cancellationToken);
			}
			try
			{
				return driver is DriverBase driverBase ? driverBase.ExecuteReaderAsync(command, cancellationToken) : command.ExecuteReaderAsync(cancellationToken);
			}
			catch (System.Exception ex)
			{
				return Task.FromException<DbDataReader>(ex);
			}
		}

	}
}
