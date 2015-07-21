using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Dialect.Function;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Criterion
{
	[Serializable]
	public class SqlFunctionProjection : SimpleProjection
	{
		private readonly IProjection[] args;
		private readonly ISQLFunction function;
		private readonly string functionName;
		private readonly IType returnType;

		public SqlFunctionProjection(string functionName, IType returnType, params IProjection[] args)
		{
			this.functionName = functionName;
			this.returnType = returnType;
			this.args = args;
		}

		public SqlFunctionProjection(ISQLFunction function, IType returnType, params IProjection[] args)
		{
			this.function = function;
			this.returnType = returnType;
			this.args = args;
		}

		public override bool IsAggregate
		{
			get { return false; }
		}

		public override bool IsGrouped
		{
			get
			{
				foreach (IProjection projection in args)
				{
					if (projection.IsGrouped)
					{
						return true;
					}
				}
				return false;
			}
		}

		public override SqlString ToGroupSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery,
												   IDictionary<string, IFilter> enabledFilters)
		{
			SqlStringBuilder buf = new SqlStringBuilder();
			foreach (IProjection projection in args)
			{
				if (projection.IsGrouped)
				{
					buf.Add(projection.ToGroupSqlString(criteria, criteriaQuery, enabledFilters)).Add(", ");
				}
			}
			if (buf.Count >= 2)
			{
				buf.RemoveAt(buf.Count - 1);
			}
			return buf.ToSqlString();
		}

		public override SqlString ToSqlString(ICriteria criteria, int position, ICriteriaQuery criteriaQuery,
											  IDictionary<string, IFilter> enabledFilters)
		{
			ISQLFunction sqlFunction = GetFunction(criteriaQuery);

			var arguments = new List<object>();
			for (int i = 0; i < args.Length; i++)
			{
				SqlString projectArg = GetProjectionArgument(criteriaQuery, criteria, args[i], 0, enabledFilters); // The loc parameter is unused.
				arguments.Add(projectArg);
			}

			return new SqlString(
				sqlFunction.Render(arguments, criteriaQuery.Factory),
				" as ",
				GetColumnAliases(position, criteria, criteriaQuery)[0]);
		}

		private ISQLFunction GetFunction(ICriteriaQuery criteriaQuery)
		{
			if (function != null)
			{
				return function;
			}
			ISQLFunction dialectFunction = criteriaQuery.Factory.SQLFunctionRegistry.FindSQLFunction(functionName);
			if (dialectFunction == null)
			{
				throw new HibernateException("Current dialect " + criteriaQuery.Factory.Dialect + " doesn't support the function: "
											 + functionName);
			}
			return dialectFunction;
		}

		private static SqlString GetProjectionArgument(ICriteriaQuery criteriaQuery, ICriteria criteria,
													   IProjection projection, int loc,
													   IDictionary<string, IFilter> enabledFilters)
		{
			SqlString sql = projection.ToSqlString(criteria, loc, criteriaQuery, enabledFilters);
			return SqlStringHelper.RemoveAsAliasesFromSql(sql);
		}

		public override IType[] GetTypes(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			ISQLFunction sqlFunction = GetFunction(criteriaQuery);
			IType type = sqlFunction.ReturnType(returnType, criteriaQuery.Factory);
			return new IType[] {type};
		}

		public override TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			List<TypedValue> types = new List<TypedValue>();
			foreach (IProjection projection in args)
			{
				TypedValue[] argTypes = projection.GetTypedValues(criteria, criteriaQuery);
				types.AddRange(argTypes);
			}
			return types.ToArray();
		}
	}
}
