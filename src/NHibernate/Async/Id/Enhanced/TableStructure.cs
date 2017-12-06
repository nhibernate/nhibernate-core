﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Data;
using System.Data.Common;

using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.AdoNet.Util;

namespace NHibernate.Id.Enhanced
{
	using System.Threading.Tasks;
	using System.Threading;
	public partial class TableStructure : TransactionHelper, IDatabaseStructure
	{

		#region Overrides of TransactionHelper

		public override async Task<object> DoWorkInCurrentTransactionAsync(ISessionImplementor session, DbConnection conn, DbTransaction transaction, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			long result;
			int updatedRows;

			do
			{
				try
				{
					object selectedValue;

					var selectCmd = session.Factory.ConnectionProvider.Driver.GenerateCommand(CommandType.Text, _selectQuery, SqlTypeFactory.NoTypes);
					using (selectCmd)
					{
						selectCmd.Connection = conn;
						selectCmd.Transaction = transaction;
						PersistentIdGeneratorParmsNames.SqlStatementLogger.LogCommand(selectCmd, FormatStyle.Basic);

						selectedValue = await (selectCmd.ExecuteScalarAsync(cancellationToken)).ConfigureAwait(false);
					}

					if (selectedValue ==null)
					{
						Log.Error("could not read a hi value - you need to populate the table: {0}", _tableName);
						throw new IdentifierGenerationException("could not read a hi value - you need to populate the table: " + _tableName);
					}
					result = Convert.ToInt64(selectedValue);
				}
				catch (Exception sqle)
				{
					Log.Error(sqle, "could not read a hi value");
					throw;
				}

				try
				{
					var updateCmd = session.Factory.ConnectionProvider.Driver.GenerateCommand(CommandType.Text, _updateQuery, _updateParameterTypes);
					using (updateCmd)
					{
						updateCmd.Connection = conn;
						updateCmd.Transaction = transaction;
						PersistentIdGeneratorParmsNames.SqlStatementLogger.LogCommand(updateCmd, FormatStyle.Basic);

						int increment = _applyIncrementSizeToSourceValues ? _incrementSize : 1;
						updateCmd.Parameters[0].Value = result + increment;
						updateCmd.Parameters[1].Value = result;
						updatedRows = await (updateCmd.ExecuteNonQueryAsync(cancellationToken)).ConfigureAwait(false);
					}
				}
				catch (Exception sqle)
				{
					Log.Error(sqle, "could not update hi value in: {0}", _tableName);
					throw;
				}
			}
			while (updatedRows == 0);

			_accessCounter++;

			return result;
		}

		#endregion

		#region Nested type: TableAccessCallback

		private partial class TableAccessCallback : IAccessCallback
		{

			#region IAccessCallback Members

			public virtual async Task<long> GetNextValueAsync(CancellationToken cancellationToken)
			{
				cancellationToken.ThrowIfCancellationRequested();
				return Convert.ToInt64(await (_owner.DoWorkInNewTransactionAsync(_session, cancellationToken)).ConfigureAwait(false));
			}

			#endregion
		}

		#endregion
	}
}
