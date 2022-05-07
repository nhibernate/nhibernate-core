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

namespace NHibernate.AdoNet
{
	using System.Threading.Tasks;
	using System.Threading;
	internal partial class DbCommandWrapper : DbCommand
	{

		/// <inheritdoc />
		public override Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<int>(cancellationToken);
			}
			return Command.ExecuteNonQueryAsync(cancellationToken);
		}

		/// <inheritdoc />
		public override Task<object> ExecuteScalarAsync(CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			return Command.ExecuteScalarAsync(cancellationToken);
		}

		/// <inheritdoc />
		protected override Task<DbDataReader> ExecuteDbDataReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<DbDataReader>(cancellationToken);
			}
			return Command.ExecuteReaderAsync(behavior, cancellationToken);
		}
	}
}
