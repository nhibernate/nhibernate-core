﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Runtime.Serialization;
using System.Security;

using NHibernate.Engine;

namespace NHibernate.AdoNet
{
	using System.Threading.Tasks;
	using System.Threading;
	public partial class ConnectionManager : ISerializable, IDeserializationCallback
	{

		public Task<DbConnection> GetConnectionAsync(CancellationToken cancellationToken)
		{
			if (!_allowConnectionUsage)
			{
				throw new HibernateException("Connection usage is currently disallowed");
			}
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<DbConnection>(cancellationToken);
			}
			return InternalGetConnectionAsync();
			async Task<DbConnection> InternalGetConnectionAsync()
			{

				if (_connectionReleaseRequired)
				{
					_connectionReleaseRequired = false;
					if (_connection != null)
					{
						_log.Debug("Releasing database connection");
						CloseConnection();
					}
				}

				if (_connectionEnlistmentRequired)
				{
					_connectionEnlistmentRequired = false;
					// No null check on transaction: we need to do it for connection supporting it, and
					// _connectionEnlistmentRequired should not be set if the transaction is null while the
					// connection does not support it.
					_connection?.EnlistTransaction(_currentSystemTransaction);
				}

				if (_connection == null)
				{
					if (_ownConnection)
					{
						_connection = await (Factory.ConnectionProvider.GetConnectionAsync(cancellationToken)).ConfigureAwait(false);
						// Will fail if the connection is already enlisted in another transaction.
						// Probable case: nested transaction scope with connection auto-enlistment enabled.
						// That is an user error.
						if (_currentSystemTransaction != null)
							_connection.EnlistTransaction(_currentSystemTransaction);

						if (Factory.Statistics.IsStatisticsEnabled)
						{
							Factory.StatisticsImplementor.Connect();
						}
					}
					else if (Session.IsOpen)
					{
						throw new HibernateException("Session is currently disconnected");
					}
					else
					{
						throw new HibernateException("Session is closed");
					}
				}
				return _connection;
			}
		}

		public async Task<DbCommand> CreateCommandAsync(CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			var result = (await (GetConnectionAsync(cancellationToken)).ConfigureAwait(false)).CreateCommand();
			Transaction.Enlist(result);
			return result;
		}
	}
}
