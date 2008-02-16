namespace NHibernate.Expressions
{
	using System;
	using System.Collections.Generic;
	using Dialect;
	using Engine;
	using NHibernate.Dialect.Function;
	using SqlCommand;
	using Type;

	[Serializable]
	public class SqlFunctionProjection : SimpleProjection
	{
		private readonly string functionName;
		private readonly object[] args;
		private readonly ISQLFunction function;
		private readonly IType returnType;

		public SqlFunctionProjection(string functionName, IType returnType, params object[] args)
		{
			this.functionName = functionName;
			this.returnType = returnType;
			ValidateArguments(args);
			this.args = args;
		}

		public SqlFunctionProjection(ISQLFunction function, IType returnType, params object[] args)
		{
			this.function = function;
			this.returnType = returnType;
			ValidateArguments(args);
			this.args = args;
		}

		private static void ValidateArguments(object[] argsToValidate)
		{
			foreach (object arg in argsToValidate)
			{
				if (arg is IProjection || arg is TypedValue)
					continue;
				throw new HibernateException("SqlFunctionProjection can only accept an IProjection or TypedValue.");
			}
		}

		public override SqlString ToSqlString(ICriteria criteria, int position, ICriteriaQuery criteriaQuery)
		{
			ISQLFunction sqlFunction = GetFunction(criteriaQuery);
			List<string> tokens = new List<string>();
			string replacemenToken = Guid.NewGuid().ToString("n");
			for (int i = 0; i < args.Length; i++)
			{
				tokens.Add(replacemenToken);
			}
			string functionStatement = sqlFunction.Render(tokens, criteriaQuery.Factory);
			string[] splitted = functionStatement.Split(new string[] { replacemenToken }, StringSplitOptions.RemoveEmptyEntries);

			SqlStringBuilder sb = new SqlStringBuilder();
			for (int i = 0; i < splitted.Length; i++)
			{
				sb.Add(splitted[i]);
				if (i < args.Length)
				{
					if (args[i] is IProjection)
					{
						int loc = (position + 1) * 1000 + i;
						SqlString projectArg = GetProjectionArgument(
							criteriaQuery,
							criteria,
							(IProjection)args[i],
							loc);
						sb.Add(projectArg);
					}
					else
					{
						sb.AddParameter();
					}
				}
			}
			sb.Add(" as ");
			sb.Add(GetColumnAliases(position)[0]);
			return sb.ToSqlString();
		}

		private ISQLFunction GetFunction(ICriteriaQuery criteriaQuery)
		{
			if (function != null)
				return function;
			Dialect dialect = criteriaQuery.Factory.Dialect;
			if (dialect.Functions.ContainsKey(functionName) == false)
			{
				throw new HibernateException("Current dialect " + dialect + " doesn't support the function: " + functionName);
			}
			return dialect.Functions[functionName];
		}

		private static SqlString GetProjectionArgument(
			ICriteriaQuery criteriaQuery,
			ICriteria criteria,
			IProjection projection,
			int loc)
		{
			SqlString sql = projection.ToSqlString(criteria, loc, criteriaQuery);
			return RemoveAliasesFromSql(sql);
		}

		private static SqlString RemoveAliasesFromSql(SqlString sql)
		{
			SqlStringBuilder sb = new SqlStringBuilder();
			foreach (object part in sql.Parts)
			{
				if (part is string)
				{
					string partAsString = (string)part;
					if (partAsString.Contains(" as "))
						return sb.ToSqlString();
					sb.Add(partAsString);
				}
				else
				{
					sb.Add((Parameter)part);
				}
			}
			return sb.ToSqlString();
		}

		public override IType[] GetTypes(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			ISQLFunction function = GetFunction(criteriaQuery);
			IType type = function.ReturnType(returnType, criteriaQuery.Factory);
			return new IType[] { type };
		}

		public override TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			List<TypedValue> types = new List<TypedValue>();
			foreach (object arg in args)
			{
				IProjection projection = arg as IProjection;
				if (projection == null)
				{
					types.Add((TypedValue)arg);
				}
				else
				{
					TypedValue[] argTypes = projection.GetTypedValues(criteria, criteriaQuery);
					types.AddRange(argTypes);
				}
			}
			return types.ToArray();
		}
	}
}