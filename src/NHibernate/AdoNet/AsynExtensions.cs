#if NETFX || NETSTANDARD2_0 || NETCOREAPP2_0
using System;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.Common
{
	internal static class AsyncExtensions
	{
		public static Task<DbTransaction> BeginTransactionAsync(this DbConnection connection, CancellationToken cancellationToken = default)
		{
			if (cancellationToken.IsCancellationRequested)
				return Task.FromCanceled<DbTransaction>(cancellationToken);

			try
			{
				return Task.FromResult(connection.BeginTransaction());
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		public static Task<DbTransaction> BeginTransactionAsync(this DbConnection connection, IsolationLevel isolationLevel, CancellationToken cancellationToken = default)
		{
			if (cancellationToken.IsCancellationRequested)
				return Task.FromCanceled<DbTransaction>(cancellationToken);

			try
			{
				return Task.FromResult(connection.BeginTransaction(isolationLevel));
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}
	}
}
#endif
