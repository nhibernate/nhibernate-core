using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Antlr.Runtime;
using Antlr.Runtime.Tree;

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
	public partial class BasicExecutor : AbstractStatementExecutor
	{
		private readonly IQueryable persister;
		private static readonly INHibernateLogger log = NHibernateLogger.For(typeof(BasicExecutor));
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

		private IList<IParameterSpecification> Parameters { get; set; }

		public override SqlString[] SqlStatements
		{
			get { return new[] {sql}; }
		}

		public override int Execute(QueryParameters parameters, ISessionImplementor session)
		{
			CoordinateSharedCacheCleanup(session);

			DbCommand st = null;
			RowSelection selection = parameters.RowSelection;

			try
			{
				try
				{
					CheckParametersExpectedType(parameters); // NH Different behavior (NH-1898)
					// Create a copy of Parameters as ExpandDynamicFilterParameters may modify it
					var parameterSpecifications = Parameters.ToList();
					var sqlString = FilterHelper.ExpandDynamicFilterParameters(sql, parameterSpecifications, session);
					var sqlQueryParametersList = sqlString.GetParameters().ToList();
					SqlType[] parameterTypes = parameterSpecifications.GetQueryParameterTypes(sqlQueryParametersList, session.Factory);

					st = session.Batcher.PrepareCommand(CommandType.Text, sqlString, parameterTypes);
					foreach (var parameterSpecification in parameterSpecifications)
					{
						parameterSpecification.Bind(st, sqlQueryParametersList, parameters, session);
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

		private void CheckParametersExpectedType(QueryParameters parameters)
		{
			foreach (var specification in Parameters)
			{
				if (specification.ExpectedType == null)
				{
					var namedSpec = specification as NamedParameterSpecification;
					if (namedSpec != null)
					{
						TypedValue tv;
						if(parameters.NamedParameters.TryGetValue(namedSpec.Name, out tv))
						{
							specification.ExpectedType = tv.Type;
						}
					}
					else
					{
						var posSpec = specification as PositionalParameterSpecification;
						if (posSpec != null)
						{
							specification.ExpectedType = parameters.PositionalParameterTypes[posSpec.HqlPosition];
						}
					}
				}
			}
		}

		protected override IQueryable[] AffectedQueryables
		{
			get { return new[] { persister }; }
		}
	}
}
