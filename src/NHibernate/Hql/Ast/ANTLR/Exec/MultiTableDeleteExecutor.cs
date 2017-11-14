using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using NHibernate.Engine;
using NHibernate.Exceptions;
using NHibernate.Hql.Ast.ANTLR.Tree;
using NHibernate.Param;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Util;
using IQueryable = NHibernate.Persister.Entity.IQueryable;

namespace NHibernate.Hql.Ast.ANTLR.Exec
{
	[CLSCompliant(false)]
	public partial class MultiTableDeleteExecutor : AbstractStatementExecutor
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(MultiTableDeleteExecutor));
		private readonly IQueryable persister;
		private readonly SqlString idInsertSelect;
		private readonly SqlString[] deletes;

		public MultiTableDeleteExecutor(IStatement statement)
			: base(statement, log)
		{
			if (!Factory.Dialect.SupportsTemporaryTables)
			{
				throw new HibernateException("cannot perform multi-table deletes using dialect not supporting temp tables");
			}

			var deleteStatement = (DeleteStatement) statement;

			FromElement fromElement = deleteStatement.FromClause.GetFromElement();
			string bulkTargetAlias = fromElement.TableAlias;
			persister = fromElement.Queryable;

			idInsertSelect = GenerateIdInsertSelect(persister, bulkTargetAlias, deleteStatement.WhereClause);
			log.Debug("Generated ID-INSERT-SELECT SQL (multi-table delete) : " + idInsertSelect);

			string[] tableNames = persister.ConstraintOrderedTableNameClosure;
			string[][] columnNames = persister.ConstraintOrderedTableKeyColumnClosure;
			string idSubselect = GenerateIdSubselect(persister);

			deletes = new SqlString[tableNames.Length];
			for (int i = tableNames.Length - 1; i >= 0; i--)
			{
				// TODO : an optimization here would be to consider cascade deletes and not gen those delete statements;
				//      the difficulty is the ordering of the tables here vs the cascade attributes on the persisters ->
				//          the table info gotten here should really be self-contained (i.e., a class representation
				//          defining all the needed attributes), then we could then get an array of those
				SqlDeleteBuilder delete = new SqlDeleteBuilder(Factory.Dialect, Factory)
					.SetTableName(tableNames[i])
					.SetWhere("(" + StringHelper.Join(", ", columnNames[i]) + ") IN (" + idSubselect + ")");
				if (Factory.Settings.IsCommentsEnabled)
				{
					delete.SetComment("bulk delete");
				}

				deletes[i] = delete.ToSqlString();
			}
		}

		public override SqlString[] SqlStatements
		{
			get { return deletes; }
		}

		public override int Execute(QueryParameters parameters, ISessionImplementor session)
		{
			CoordinateSharedCacheCleanup(session);

			CreateTemporaryTableIfNecessary(persister, session);

			try
			{
				// First, save off the pertinent ids, saving the number of pertinent ids for return
				DbCommand ps = null;
				int resultCount;
				try
				{
					try
					{
						var paramsSpec = Walker.Parameters;
						var sqlQueryParametersList = idInsertSelect.GetParameters().ToList();
						SqlType[] parameterTypes = paramsSpec.GetQueryParameterTypes(sqlQueryParametersList, session.Factory);

						ps = session.Batcher.PrepareCommand(CommandType.Text, idInsertSelect, parameterTypes);
						foreach (var parameterSpecification in paramsSpec)
						{
							parameterSpecification.Bind(ps, sqlQueryParametersList, parameters, session);
						}

						resultCount = session.Batcher.ExecuteNonQuery(ps);
					}
					finally
					{
						if (ps != null)
						{
							session.Batcher.CloseCommand(ps, null);
						}
					}
				}
				catch (DbException e)
				{
					throw ADOExceptionHelper.Convert(Factory.SQLExceptionConverter, e, "could not insert/select ids for bulk delete", idInsertSelect);
				}

				// Start performing the deletes
				for (int i = 0; i < deletes.Length; i++)
				{
					try
					{
						try
						{
							ps = session.Batcher.PrepareCommand(CommandType.Text, deletes[i], new SqlType[0]);
							session.Batcher.ExecuteNonQuery(ps);
						}
						finally
						{
							if (ps != null)
							{
								session.Batcher.CloseCommand(ps, null);
							}
						}
					}
					catch (DbException e)
					{
						throw ADOExceptionHelper.Convert(Factory.SQLExceptionConverter, e, "error performing bulk delete", deletes[i]);
					}
				}

				return resultCount;
			}
			finally
			{
				DropTemporaryTableIfNecessary(persister, session);
			}
		}

		protected override IQueryable[] AffectedQueryables
		{
			get { return new[] { persister }; }
		}
	}
}