using System;
using System.Data;
using NHibernate.Engine;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Impl;
using NHibernate.Exceptions;

namespace NHibernate.Dialect.Lock
{
	/// <summary> 
	/// A locking strategy where the locks are obtained through select statements.
	///  </summary>
	/// <seealso cref="NHibernate.Dialect.Dialect.GetForUpdateString(NHibernate.LockMode)"/>
	/// <seealso cref="NHibernate.Dialect.Dialect.AppendLockHint(NHibernate.LockMode, string)"/>
	/// <remarks>
	/// For non-read locks, this is achieved through the Dialect's specific
	/// SELECT ... FOR UPDATE syntax.
	/// </remarks>
	public class SelectLockingStrategy : ILockingStrategy
	{
		private readonly ILockable lockable;
		private readonly LockMode lockMode;
		private readonly SqlString sql;

		public SelectLockingStrategy(ILockable lockable, LockMode lockMode)
		{
			this.lockable = lockable;
			this.lockMode = lockMode;
			sql = GenerateLockString();
		}

		private SqlString GenerateLockString()
		{
			ISessionFactoryImplementor factory = lockable.Factory;
			SqlSimpleSelectBuilder select = new SqlSimpleSelectBuilder(factory.Dialect, factory)
				.SetLockMode(lockMode)
				.SetTableName(lockable.RootTableName)
				.AddColumn(lockable.RootTableIdentifierColumnNames[0])
				.SetIdentityColumn(lockable.RootTableIdentifierColumnNames, lockable.IdentifierType);
			if (lockable.IsVersioned)
			{
				select.SetVersionColumn(new string[] { lockable.VersionColumnName }, lockable.VersionType);
			}
			if (factory.Settings.IsCommentsEnabled)
			{
				select.SetComment(lockMode + " lock " + lockable.EntityName);
			}
			return select.ToSqlString();
		}

		#region ILockingStrategy Members

		public void Lock(object id, object version, object obj, ISessionImplementor session)
		{
			ISessionFactoryImplementor factory = session.Factory;
			try
			{
				IDbCommand st = session.Batcher.PrepareCommand(CommandType.Text, sql, lockable.IdAndVersionSqlTypes);
				IDataReader rs = null;
				try
				{
					lockable.IdentifierType.NullSafeSet(st, id, 0, session);
					if (lockable.IsVersioned)
					{
						lockable.VersionType.NullSafeSet(st, version, lockable.IdentifierType.GetColumnSpan(factory), session);
					}

					rs = session.Batcher.ExecuteReader(st);
					try
					{
						if (!rs.Read())
						{
							if (factory.Statistics.IsStatisticsEnabled)
							{
								factory.StatisticsImplementor.OptimisticFailure(lockable.EntityName);
							}
							throw new StaleObjectStateException(lockable.EntityName, id);
						}
					}
					finally
					{
						rs.Close();
					}
				}
				finally
				{
					session.Batcher.CloseCommand(st, rs);
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