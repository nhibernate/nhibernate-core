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
	/// <summary> 
	/// A locking strategy where the locks are obtained through update statements.
	/// </summary>
	/// <remarks> This strategy is not valid for read style locks. </remarks>
	public partial class UpdateLockingStrategy : ILockingStrategy
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(UpdateLockingStrategy));
		private readonly ILockable lockable;
		private readonly LockMode lockMode;
		private readonly SqlString sql;

		/// <summary> 
		/// Construct a locking strategy based on SQL UPDATE statements.
		/// </summary>
		/// <param name="lockable">The metadata for the entity to be locked. </param>
		/// <param name="lockMode">Indicates the type of lock to be acquired. </param>
		/// <remarks>
		/// read-locks are not valid for this strategy.
		/// </remarks>
		public UpdateLockingStrategy(ILockable lockable, LockMode lockMode)
		{
			this.lockable = lockable;
			this.lockMode = lockMode;
			if (lockMode.LessThan(LockMode.Upgrade))
			{
				throw new HibernateException("[" + lockMode + "] not valid for update statement");
			}
			if (!lockable.IsVersioned)
			{
				log.Warn("write locks via update not supported for non-versioned entities [" + lockable.EntityName + "]");
				sql = null;
			}
			else
			{
				sql = GenerateLockString();
			}
		}

		private SqlString GenerateLockString()
		{
			ISessionFactoryImplementor factory = lockable.Factory;
			SqlUpdateBuilder update = new SqlUpdateBuilder(factory.Dialect, factory);
			update.SetTableName(lockable.RootTableName);
			update.SetIdentityColumn(lockable.RootTableIdentifierColumnNames, lockable.IdentifierType);
			update.SetVersionColumn(new string[] { lockable.VersionColumnName }, lockable.VersionType);
			update.AddColumns(new string[] { lockable.VersionColumnName }, null, lockable.VersionType);
			if (factory.Settings.IsCommentsEnabled)
			{
				update.SetComment(lockMode + " lock " + lockable.EntityName);
			}
			return update.ToSqlString();
		}

		#region ILockingStrategy Members

		public void Lock(object id, object version, object obj, ISessionImplementor session)
		{
			if (!lockable.IsVersioned)
			{
				throw new HibernateException("write locks via update not supported for non-versioned entities [" + lockable.EntityName + "]");
			}
			// todo : should we additionally check the current isolation mode explicitly?
			ISessionFactoryImplementor factory = session.Factory;
			try
			{
				var st = session.Batcher.PrepareCommand(CommandType.Text, sql, lockable.IdAndVersionSqlTypes);
				try
				{
					lockable.VersionType.NullSafeSet(st, version, 1, session);
					int offset = 2;

					lockable.IdentifierType.NullSafeSet(st, id, offset, session);
					offset += lockable.IdentifierType.GetColumnSpan(factory);

					if (lockable.IsVersioned)
					{
						lockable.VersionType.NullSafeSet(st, version, offset, session);
					}

					int affected = session.Batcher.ExecuteNonQuery(st);
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

		#endregion
	}
}