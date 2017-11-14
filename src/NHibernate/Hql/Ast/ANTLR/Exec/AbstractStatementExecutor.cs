using System;
using System.Data.Common;
using Antlr.Runtime;
using Antlr.Runtime.Tree;
using NHibernate.Action;
using NHibernate.AdoNet.Util;
using NHibernate.Engine;
using NHibernate.Engine.Transaction;
using NHibernate.Event;
using NHibernate.Hql.Ast.ANTLR.Tree;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Transaction;
using NHibernate.Util;
using System.Data;

namespace NHibernate.Hql.Ast.ANTLR.Exec
{
	[CLSCompliant(false)]
	public abstract partial class AbstractStatementExecutor : IStatementExecutor
	{
		private readonly IInternalLogger log;

		protected AbstractStatementExecutor(IStatement statement, IInternalLogger log)
		{
			Statement = statement;
			Walker = statement.Walker;
			this.log = log;
		}

		protected HqlSqlWalker Walker { get; private set; }
		protected IStatement Statement { get; private set; }

		public abstract SqlString[] SqlStatements { get; }

		public abstract int Execute(QueryParameters parameters, ISessionImplementor session);

		protected abstract IQueryable[] AffectedQueryables { get; }

		protected ISessionFactoryImplementor Factory
		{
			get{return Walker.SessionFactoryHelper.Factory;}
		}

		protected virtual void CoordinateSharedCacheCleanup(ISessionImplementor session)
		{
			var action = new BulkOperationCleanupAction(session, AffectedQueryables);

			action.Init();

			if (session.IsEventSource)
			{
				((IEventSource)session).ActionQueue.AddAction(action);
			}
		}

		protected SqlString GenerateIdInsertSelect(IQueryable persister, string tableAlias, IASTNode whereClause)
		{
			var select = new SqlSelectBuilder(Factory);
			SelectFragment selectFragment = new SelectFragment(Factory.Dialect)
				.AddColumns(tableAlias, persister.IdentifierColumnNames, persister.IdentifierColumnNames);
			select.SetSelectClause(selectFragment.ToFragmentString().Substring(2));

			string rootTableName = persister.TableName;
			SqlString fromJoinFragment = persister.FromJoinFragment(tableAlias, true, false);
			select.SetFromClause(rootTableName + " " + tableAlias + fromJoinFragment);
			
			var whereJoinFragment = GetWhereJoinFragment(persister, tableAlias);

			SqlString userWhereClause = SqlString.Empty;
			if (whereClause.ChildCount != 0)
			{
				// If a where clause was specified in the update/delete query, use it to limit the
				// returned ids here...
				try
				{
					var nodes = new CommonTreeNodeStream(whereClause);
					var gen = new SqlGenerator(Factory, nodes);
					gen.whereClause();
					userWhereClause = gen.GetSQL().Substring(7);
				}
				catch (RecognitionException e)
				{
					throw new HibernateException("Unable to generate id select for DML operation", e);
				}
				if (whereJoinFragment.Length > 0)
				{
					whereJoinFragment = whereJoinFragment.Append(" and ");
				}
			}

			select.SetWhereClause(whereJoinFragment + userWhereClause);

			var insert = new InsertSelect();
			if (Factory.Settings.IsCommentsEnabled)
			{
				insert.SetComment("insert-select for " + persister.EntityName + " ids");
			}
			insert.SetTableName(persister.TemporaryIdTableName);
			insert.SetSelect(select);
			return insert.ToSqlString();
		}

		private static SqlString GetWhereJoinFragment(IJoinable persister, string tableAlias)
		{
			SqlString whereJoinFragment = persister.WhereJoinFragment(tableAlias, true, false);
			if (whereJoinFragment == null)
			{
				whereJoinFragment = SqlString.Empty;
			}
			else
			{
				whereJoinFragment = whereJoinFragment.Trim();
				if (whereJoinFragment.StartsWithCaseInsensitive("and "))
				{
					whereJoinFragment = whereJoinFragment.Substring(4);
				}
			}
			return whereJoinFragment;
		}

		protected string GenerateIdSubselect(IQueryable persister)
		{
			return "select " + StringHelper.Join(", ", persister.IdentifierColumnNames) + " from " + persister.TemporaryIdTableName;
		}

		protected virtual void CreateTemporaryTableIfNecessary(IQueryable persister, ISessionImplementor session)
		{
			// Don't really know all the codes required to adequately decipher returned ADO exceptions here.
			// simply allow the failure to be eaten and the subsequent insert-selects/deletes should fail
			IIsolatedWork work = new TmpIdTableCreationIsolatedWork(persister, log, session);
			if (ShouldIsolateTemporaryTableDDL())
			{
				if (Factory.Settings.IsDataDefinitionInTransactionSupported)
				{
					Isolater.DoIsolatedWork(work, session);
				}
				else
				{
					Isolater.DoNonTransactedWork(work, session);
				}
			}
			else
			{
				using (var dummyCommand = session.ConnectionManager.CreateCommand())
				{
					work.DoWork(dummyCommand.Connection, dummyCommand.Transaction);
					session.ConnectionManager.AfterStatement();
				}
			}
		}

		protected virtual bool ShouldIsolateTemporaryTableDDL()
		{
			bool? dialectVote = Factory.Dialect.PerformTemporaryTableDDLInIsolation();
			if (dialectVote.HasValue)
			{
				return dialectVote.Value;
			}
			return Factory.Settings.IsDataDefinitionImplicitCommit;
		}

		protected virtual void DropTemporaryTableIfNecessary(IQueryable persister, ISessionImplementor session)
		{
			if (Factory.Dialect.DropTemporaryTableAfterUse())
			{
				IIsolatedWork work = new TmpIdTableDropIsolatedWork(persister, log, session);

				if (ShouldIsolateTemporaryTableDDL())
				{
					session.ConnectionManager.Transaction.RegisterSynchronization(new AfterTransactionCompletes((success) =>
					{
						if (Factory.Settings.IsDataDefinitionInTransactionSupported)
						{
							Isolater.DoIsolatedWork(work, session);
						}
						else
						{
							Isolater.DoNonTransactedWork(work, session);
						}
					}));
				}
				else
				{
					using (var dummyCommand = session.ConnectionManager.CreateCommand())
					{
						work.DoWork(dummyCommand.Connection, dummyCommand.Transaction);
						session.ConnectionManager.AfterStatement();
					}
				}
			}
			else
			{
				// at the very least cleanup the data :)
				DbCommand ps = null;
				try
				{
					var commandText = new SqlString("delete from " + persister.TemporaryIdTableName);
					ps = session.Batcher.PrepareCommand(CommandType.Text, commandText, new SqlType[0]);
					session.Batcher.ExecuteNonQuery(ps);
				}
				catch (Exception t)
				{
					log.Warn("unable to cleanup temporary id table after use [" + t + "]");
				}
				finally
				{
					if (ps != null)
					{
						try
						{
							session.Batcher.CloseCommand(ps, null);
						}
						catch (Exception)
						{
							// ignore
						}
					}
				}
			}
		}

		private partial class TmpIdTableCreationIsolatedWork : IIsolatedWork
		{
			private readonly IQueryable persister;
			private readonly IInternalLogger log;
			private readonly ISessionImplementor session;

			public TmpIdTableCreationIsolatedWork(IQueryable persister, IInternalLogger log, ISessionImplementor session)
			{
				this.persister = persister;
				this.log = log;
				this.session = session;
			}

			public void DoWork(DbConnection connection, DbTransaction transaction)
			{
				DbCommand stmnt = null;
				try
				{
					stmnt = connection.CreateCommand();
					stmnt.Transaction = transaction;
					stmnt.CommandText = persister.TemporaryIdTableDDL;
					stmnt.ExecuteNonQuery();
					session.Factory.Settings.SqlStatementLogger.LogCommand(stmnt, FormatStyle.Ddl);
				}
				catch (Exception t)
				{
					log.Debug("unable to create temporary id table [" + t.Message + "]");
				}
				finally
				{
					if (stmnt != null)
					{
						try
						{
							stmnt.Dispose();
						}
						catch (Exception)
						{
							// ignore
						}
					}
				}
			}
		}

		private partial class TmpIdTableDropIsolatedWork : IIsolatedWork
		{
			public TmpIdTableDropIsolatedWork(IQueryable persister, IInternalLogger log, ISessionImplementor session)
			{
				this.persister = persister;
				this.log = log;
				this.session = session;
			}

			private readonly IQueryable persister;
			private readonly IInternalLogger log;
			private readonly ISessionImplementor session;

			public void DoWork(DbConnection connection, DbTransaction transaction)
			{
				DbCommand stmnt = null;
				try
				{
					stmnt = connection.CreateCommand();
					stmnt.Transaction = transaction;
					stmnt.CommandText = "drop table " + persister.TemporaryIdTableName;
					stmnt.ExecuteNonQuery();
					session.Factory.Settings.SqlStatementLogger.LogCommand(stmnt, FormatStyle.Ddl);
				}
				catch (Exception t)
				{
					log.Warn("unable to drop temporary id table after use [" + t.Message + "]");
				}
				finally
				{
					if (stmnt != null)
					{
						try
						{
							stmnt.Dispose();
						}
						catch (Exception)
						{
							// ignore
						}
					}
				}
			}
		}
	}
}