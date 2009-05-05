using System;
using System.Collections.Generic;
using System.Data;
using log4net;
using NHibernate.Engine;
using NHibernate.Exceptions;
using NHibernate.Hql.Ast.ANTLR.Tree;
using NHibernate.Param;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Util;

namespace NHibernate.Hql.Ast.ANTLR.Exec
{
	[CLSCompliant(false)]
	public class MultiTableUpdateExecutor : AbstractStatementExecutor
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(MultiTableDeleteExecutor));
		private readonly IQueryable persister;
		private readonly SqlString idInsertSelect;
		private readonly SqlString[] updates;
		private readonly IParameterSpecification[][] hqlParameters;

		public MultiTableUpdateExecutor(IStatement statement) : base(statement, log)
		{
			if (!Factory.Dialect.SupportsTemporaryTables)
			{
				throw new HibernateException("cannot perform multi-table updates using dialect not supporting temp tables");
			}
			var updateStatement = (UpdateStatement)statement;

			FromElement fromElement = updateStatement.FromClause.GetFromElement();
			string bulkTargetAlias = fromElement.TableAlias;
			persister = fromElement.Queryable;

			idInsertSelect = GenerateIdInsertSelect(persister, bulkTargetAlias, updateStatement.WhereClause);
			log.Debug("Generated ID-INSERT-SELECT SQL (multi-table update) : " + idInsertSelect);

			string[] tableNames = persister.ConstraintOrderedTableNameClosure;
			string[][] columnNames = persister.ContraintOrderedTableKeyColumnClosure;

			string idSubselect = GenerateIdSubselect(persister);
			var assignmentSpecifications = Walker.AssignmentSpecifications;

			updates = new SqlString[tableNames.Length];
			hqlParameters = new IParameterSpecification[tableNames.Length][];
			for (int tableIndex = 0; tableIndex < tableNames.Length; tableIndex++)
			{
				bool affected = false;
				var parameterList = new List<IParameterSpecification>();
				SqlUpdateBuilder update = new SqlUpdateBuilder(Factory.Dialect, Factory)
					.SetTableName(tableNames[tableIndex])
					.SetWhere("(" + StringHelper.Join(", ", columnNames[tableIndex]) + ") IN (" + idSubselect + ")");

				if (Factory.Settings.IsCommentsEnabled)
				{
					update.SetComment("bulk update");
				}
				foreach (var specification in assignmentSpecifications)
				{
					if (specification.AffectsTable(tableNames[tableIndex]))
					{
						affected = true;
						update.AppendAssignmentFragment(specification.SqlAssignmentFragment);
						if (specification.Parameters != null)
						{
							for (int paramIndex = 0; paramIndex < specification.Parameters.Length; paramIndex++)
							{
								parameterList.Add(specification.Parameters[paramIndex]);
							}
						}
					}
				}
				if (affected)
				{
					updates[tableIndex] = update.ToSqlString();
					hqlParameters[tableIndex] = parameterList.ToArray();
				}
			}
		}

		public override SqlString[] SqlStatements
		{
			get { return updates; }
		}

		public override int Execute(QueryParameters parameters, ISessionImplementor session)
		{
			CoordinateSharedCacheCleanup(session);

			CreateTemporaryTableIfNecessary(persister, session);

			try
			{
				// First, save off the pertinent ids, as the return value
				IDbCommand ps = null;
				int resultCount;
				try
				{
					try
					{
						var parameterTypes = new List<SqlType>(Walker.Parameters.Count);
						foreach (var parameterSpecification in Walker.Parameters)
						{
							parameterTypes.AddRange(parameterSpecification.ExpectedType.SqlTypes(Factory));
						}

						ps = session.Batcher.PrepareCommand(CommandType.Text, idInsertSelect, parameterTypes.ToArray());

						int parameterStart = Walker.NumberOfParametersInSetClause;

						var allParams = Walker.Parameters;

						IEnumerator<IParameterSpecification> whereParams =
							(new List<IParameterSpecification>(allParams)).GetRange(parameterStart, allParams.Count - parameterStart).
								GetEnumerator();
						// NH Different behavior: The inital value is 0 (initialized to 1 in JAVA)
						int pos = 0;
						while (whereParams.MoveNext())
						{
							var paramSpec = whereParams.Current;
							pos += paramSpec.Bind(ps, parameters, session, pos);
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
				catch (System.Data.OleDb.OleDbException e)
				{
					throw ADOExceptionHelper.Convert(Factory.SQLExceptionConverter, e, "could not insert/select ids for bulk update", idInsertSelect);
				}

				// Start performing the updates
				for (int i = 0; i < updates.Length; i++)
				{
					if (updates[i] == null)
					{
						continue;
					}
					try
					{
						try
						{
							ps = session.Batcher.PrepareCommand(CommandType.Text, updates[i], new SqlType[0]);

							if (hqlParameters[i] != null)
							{
								int position = 1; // ADO params are 0-based
								for (int x = 0; x < hqlParameters[i].Length; x++)
								{
									position += hqlParameters[i][x].Bind(ps, parameters, session, position);
								}
							}
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
					catch (System.Data.OleDb.OleDbException e)
					{
						throw ADOExceptionHelper.Convert(Factory.SQLExceptionConverter, e, "error performing bulk update", updates[i]);
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