using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Dialect.Function;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Criterion
{
	[Serializable]
	public class SqlFunctionProjection : SimpleProjection
	{
		private readonly IProjection[] args;
		private readonly ISQLFunction function;
		private readonly string functionName;
		private readonly IType returnType;
		private readonly IProjection returnTypeProjection;

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

		public SqlFunctionProjection(string functionName, IProjection returnTypeProjection, params IProjection[] args)
		{
			this.functionName = functionName;
			this.returnTypeProjection = returnTypeProjection;
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

		public override SqlString ToGroupSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			SqlStringBuilder buf = new SqlStringBuilder();
			foreach (IProjection projection in args)
			{
				if (projection.IsGrouped)
				{
					buf.Add(projection.ToGroupSqlString(criteria, criteriaQuery)).Add(", ");
				}
			}
			if (buf.Count >= 2)
			{
				buf.RemoveAt(buf.Count - 1);
			}
			return buf.ToSqlString();
		}

		public override SqlString ToSqlString(ICriteria criteria, int position, ICriteriaQuery criteriaQuery)
		{
			ISQLFunction sqlFunction = GetFunction(criteriaQuery);

			var arguments = new List<object>();
			for (int i = 0; i < args.Length; i++)
			{
				var projectArg = GetProjectionArguments(criteriaQuery, criteria, args[i]);
				arguments.AddRange(projectArg);
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

		private static object[] GetProjectionArguments(ICriteriaQuery criteriaQuery, ICriteria criteria, IProjection projection)
		{
			return CriterionUtil.GetColumnNamesAsSqlStringParts(null, projection, criteriaQuery, criteria);
		}

		public override IType[] GetTypes(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			var type = GetReturnType(criteria, criteriaQuery);
			return type != null ? new[] {type} : Array.Empty<IType>();
		}

		private IType GetReturnType(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			ISQLFunction sqlFunction = GetFunction(criteriaQuery);

			var resultType = returnType ?? returnTypeProjection?.GetTypes(criteria, criteriaQuery).FirstOrDefault();

			return sqlFunction.GetReturnType(new[] {resultType}, criteriaQuery.Factory, true);
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
