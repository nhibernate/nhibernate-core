using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Antlr.Runtime;
using log4net;
using NHibernate.Engine;
using NHibernate.Exceptions;
using NHibernate.Param;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;

namespace NHibernate.Hql.Ast.ANTLR.Exec
{
	[CLSCompliant(false)]
	public class BasicExecutor : AbstractStatementExecutor
	{
		private readonly IQueryable persister;
		private static readonly ILog log = LogManager.GetLogger(typeof(QueryTranslatorImpl));
		private SqlString sql;

		public BasicExecutor(HqlSqlWalker walker, IQueryable persister) : base(walker, log)
		{
			this.persister = persister;
			try
			{

				//SqlGenerator gen = new SqlGenerator(Factory);
				//gen.statement(walker.getAST());
				//sql = gen.GetSQL();
				//gen.ParseErrorHandler.ThrowQueryException();
			}
			catch (RecognitionException e)
			{
				throw QuerySyntaxException.Convert(e);
			}
		}

		protected ISessionFactoryImplementor Factory
		{
			get
			{
				return Walker.SessionFactoryHelper.Factory;
			}
		}

		public override SqlString[] SqlStatements
		{
			get { return new[] {sql}; }
		}

		public override int Execute(QueryParameters parameters, ISessionImplementor session)
		{
			//CoordinateSharedCacheCleanup(session);

			IDbCommand st = null;
			RowSelection selection = parameters.RowSelection;

			try
			{
				try
				{
					//st = session.Batcher.PrepareCommand(CommandType.Text, sql, parameterTypes);
					IEnumerator<IParameterSpecification> paramSpecifications = Walker.Parameters.GetEnumerator();
					int pos = 1;
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
	}
}