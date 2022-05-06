using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace NHibernate.Driver
{
	/// <summary>
	/// Base class for the implementation of IDriver
	/// </summary>
	partial class DriverBase
	{
#if NETSTANDARD2_1_OR_GREATER
		/// <summary>
		/// Begin an ADO <see cref="DbTransaction" />.
		/// </summary>
		/// <param name="isolationLevel">The isolation level requested for the transaction.</param>
		/// <param name="connection">The connection on which to start the transaction.</param>
		/// <returns>The started <see cref="DbTransaction" />.</returns>
		public virtual async ValueTask<DbTransaction> BeginTransactionAsync(IsolationLevel isolationLevel, DbConnection connection)
		{
			await connection.OpenAsync();

			if (isolationLevel == IsolationLevel.Unspecified)
			{
				return await connection.BeginTransactionAsync();
			}
			return await connection.BeginTransactionAsync(isolationLevel);
		}
#endif
	}
}
