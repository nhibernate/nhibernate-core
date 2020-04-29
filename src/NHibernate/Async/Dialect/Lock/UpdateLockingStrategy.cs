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
using NHibernate.Exceptions;
using NHibernate.Impl;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;

namespace NHibernate.Dialect.Lock
{
	using System.Threading.Tasks;
	using System.Threading;
	public partial class UpdateLockingStrategy : ILockingStrategy
	{

		#region ILockingStrategy Members

		public Task LockAsync(object id, object version, object obj, ISessionImplementor session, CancellationToken cancellationToken)
		{
			if (!lockable.IsVersioned)
			{
				throw new HibernateException("write locks via update not supported for non-versioned entities [" + lockable.EntityName + "]");
			}
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			return InternalLockAsync();
			async Task InternalLockAsync()
			{
				// todo : should we additionally check the current isolation mode explicitly?
				ISessionFactoryImplementor factory = session.Factory;
				try
				{
					var st = await (session.Batcher.PrepareCommandAsync(CommandType.Text, sql, lockable.IdAndVersionSqlTypes, cancellationToken)).ConfigureAwait(false);
					try
					{
						await (lockable.VersionType.NullSafeSetAsync(st, version, 1, session, cancellationToken)).ConfigureAwait(false);
						int offset = 2;

						await (lockable.IdentifierType.NullSafeSetAsync(st, id, offset, session, cancellationToken)).ConfigureAwait(false);
						offset += lockable.IdentifierType.GetColumnSpan(factory);

						if (lockable.IsVersioned)
						{
							await (lockable.VersionType.NullSafeSetAsync(st, version, offset, session, cancellationToken)).ConfigureAwait(false);
						}

						int affected = await (session.Batcher.ExecuteNonQueryAsync(st, cancellationToken)).ConfigureAwait(false);
						if (affected < 0)
						{
							factory.StatisticsImplementor.OptimisticFailure(lockable.EntityName);
							throw new StaleObjectStateException(lockable.EntityName, id);
						}
					}
					finally
					{
						session.Batcher.CloseCommand(st, null);
					}
				}
				catch (HibernateException)
				{
					// Do not call Convert on HibernateExceptions
					throw;
				}
				catch (Exception sqle)
				{
					var exceptionContext = new AdoExceptionContextInfo
					                       	{
					                       		SqlException = sqle,
					                       		Message = "could not lock: " + MessageHelper.InfoString(lockable, id, factory),
					                       		Sql = sql.ToString(),
					                       		EntityName = lockable.EntityName,
					                       		EntityId = id
					                       	};
					throw ADOExceptionHelper.Convert(session.Factory.SQLExceptionConverter, exceptionContext);
				}
			}
		}

		#endregion
	}
}
