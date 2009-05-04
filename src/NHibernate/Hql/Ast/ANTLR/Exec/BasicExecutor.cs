using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Antlr.Runtime;
using Antlr.Runtime.Tree;
using log4net;
using NHibernate.Engine;
using NHibernate.Exceptions;
using NHibernate.Hql.Ast.ANTLR.Tree;
using NHibernate.Param;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;

namespace NHibernate.Hql.Ast.ANTLR.Exec
{
	[CLSCompliant(false)]
	public class BasicExecutor : AbstractStatementExecutor
	{
		private readonly IQueryable persister;
		private static readonly ILog log = LogManager.GetLogger(typeof(BasicExecutor));
		private readonly SqlString sql;

		public BasicExecutor(IStatement statement, IQueryable persister)
			: base(statement, log)
		{
			this.persister = persister;
			try
			{
				var gen = new SqlGenerator(Factory, new CommonTreeNodeStream(statement));
				gen.statement();
				sql = gen.GetSQL();
				gen.ParseErrorHandler.ThrowQueryException();
				Parameters = gen.GetCollectedParameters();
			}
			catch (RecognitionException e)
			{
				throw QuerySyntaxException.Convert(e);
			}
		}

		protected IList<IParameterSpecification> Parameters{get;private set;}

		public override SqlString[] SqlStatements
		{
			get { return new[] {sql}; }
		}

		public override int Execute(QueryParameters parameters, ISessionImplementor session)
		{
			CoordinateSharedCacheCleanup(session);

			IDbCommand st = null;
			RowSelection selection = parameters.RowSelection;

			try
			{
				try
				{
					var parameterTypes = new List<SqlType>(Parameters.Count);
					foreach (var parameterSpecification in Parameters)
					{
						parameterTypes.AddRange(parameterSpecification.ExpectedType.SqlTypes(Factory));
					}
					st = session.Batcher.PrepareCommand(CommandType.Text, sql, parameterTypes.ToArray());
					IEnumerator<IParameterSpecification> paramSpecifications = Parameters.GetEnumerator();
					// NH Different behavior: The inital value is 0 (initialized to 1 in JAVA)
					int pos = 0;
					while (paramSpecifications.MoveNext())
					{
						var paramSpec = paramSpecifications.Current;
						pos += paramSpec.Bind(st, parameters, session, pos);
					}
					if (selection != null)
					{
						if (selection.Timeout != RowSelection.NoValue)
						{
							st.CommandTimeout = selection.Timeout;
						}
					}
					return session.Batcher.ExecuteNonQuery(st);
				}
				finally
				{
					if (st != null)
					{
						session.Batcher.CloseCommand(st, null);
					}
				}
			}
			catch (DbException sqle)
			{
				throw ADOExceptionHelper.Convert(session.Factory.SQLExceptionConverter, sqle,
																 "could not execute update query", sql);
			}
		}

		protected override IQueryable[] AffectedQueryables
		{
			get { return new[] { persister }; }
		}
	}
}