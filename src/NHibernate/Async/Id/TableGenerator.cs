﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;

using NHibernate.AdoNet.Util;
using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Id
{
	using System.Threading.Tasks;
	using System.Threading;
	public partial class TableGenerator : TransactionHelper, IPersistentIdentifierGenerator, IConfigurable
	{

		#region IIdentifierGenerator Members

		/// <summary>
		/// Generate a <see cref="short"/>, <see cref="int"/>, or <see cref="long"/> 
		/// for the identifier by selecting and updating a value in a table.
		/// </summary>
		/// <param name="session">The <see cref="ISessionImplementor"/> this id is being generated in.</param>
		/// <param name="obj">The entity for which the id is being generated.</param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		/// <returns>The new identifier as a <see cref="short"/>, <see cref="int"/>, or <see cref="long"/>.</returns>
		public virtual async Task<object> GenerateAsync(ISessionImplementor session, object obj, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			using (await (_asyncLock.LockAsync()).ConfigureAwait(false))
			{
				// This has to be done using a different connection to the containing
				// transaction becase the new hi value must remain valid even if the
				// containing transaction rolls back.
				return await (DoWorkInNewTransactionAsync(session, cancellationToken)).ConfigureAwait(false);
			}
		}

		#endregion

		public override async Task<object> DoWorkInCurrentTransactionAsync(ISessionImplementor session, DbConnection conn,
														  DbTransaction transaction, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			long result;
			int rows;
			do
			{
				//the loop ensure atomicitiy of the 
				//select + update even for no transaction
				//or read committed isolation level (needed for .net?)

				var qps = conn.CreateCommand();
				DbDataReader rs = null;
				qps.CommandText = query;
				qps.CommandType = CommandType.Text;
				qps.Transaction = transaction;
				PersistentIdGeneratorParmsNames.SqlStatementLogger.LogCommand("Reading high value:", qps, FormatStyle.Basic);
				try
				{
					rs = await (qps.ExecuteReaderAsync(cancellationToken)).ConfigureAwait(false);
					if (!await (rs.ReadAsync(cancellationToken)).ConfigureAwait(false))
					{
						var errFormat = string.IsNullOrEmpty(whereClause) 
							? "could not read a hi value - you need to populate the table: {0}" 
							: "could not read a hi value from table '{0}' using the where clause ({1})- you need to populate the table.";
						log.Error(errFormat, tableName, whereClause);
						throw new IdentifierGenerationException(string.Format(errFormat, tableName, whereClause));
					}
					result = Convert.ToInt64(columnType.Get(rs, 0, session));
				}
				catch (OperationCanceledException) { throw; }
				catch (Exception e)
				{
					log.Error(e, "could not read a hi value");
					throw;
				}
				finally
				{
					if (rs != null)
					{
						rs.Close();
					}
					qps.Dispose();
				}

				var ups = session.Factory.ConnectionProvider.Driver.GenerateCommand(CommandType.Text, updateSql, parameterTypes);
				ups.Connection = conn;
				ups.Transaction = transaction;

				try
				{
					columnType.Set(ups, result + 1, 0, session);
					columnType.Set(ups, result, 1, session);

					PersistentIdGeneratorParmsNames.SqlStatementLogger.LogCommand("Updating high value:", ups, FormatStyle.Basic);

					rows = await (ups.ExecuteNonQueryAsync(cancellationToken)).ConfigureAwait(false);
				}
				catch (OperationCanceledException) { throw; }
				catch (Exception e)
				{
					log.Error(e, "could not update hi value in: {0}", tableName);
					throw;
				}
				finally
				{
					ups.Dispose();
				}
			}
			while (rows == 0);

			return result;
		}
	}
}
