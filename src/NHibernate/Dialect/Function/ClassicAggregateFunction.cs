using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Type;

namespace NHibernate.Dialect.Function
{
	[Serializable]
	public class ClassicAggregateFunction : ISQLFunction, IFunctionGrammar, ISQLFunctionExtended
	{
		private IType returnType = null;
		private readonly string name;
		protected readonly bool acceptAsterisk;

		/// <summary>
		/// Initializes a new instance of the StandardSQLFunction class.
		/// </summary>
		/// <param name="name">SQL function name.</param>
		/// <param name="acceptAsterisk">Whether the function accepts an asterisk (*) in place of arguments</param>
		public ClassicAggregateFunction(string name, bool acceptAsterisk)
		{
			this.name = name;
			this.acceptAsterisk = acceptAsterisk;
		}

		/// <summary>
		/// Initializes a new instance of the StandardSQLFunction class.
		/// </summary>
		/// <param name="name">SQL function name.</param>
		/// <param name="acceptAsterisk">True if accept asterisk like argument</param>
		/// <param name="typeValue">Return type for the function.</param>
		public ClassicAggregateFunction(string name, bool acceptAsterisk, IType typeValue)
			: this(name, acceptAsterisk)
		{
			returnType = typeValue;
		}

		#region ISQLFunction Members

		// Since v5.3
		[Obsolete("Use GetReturnType method instead.")]
		public virtual IType ReturnType(IType columnType, IMapping mapping)
		{
			return returnType ?? columnType;
		}

		/// <inheritdoc />
		public virtual IType GetReturnType(IEnumerable<IType> argumentTypes, IMapping mapping, bool throwOnError)
		{
#pragma warning disable 618
			return ReturnType(argumentTypes.FirstOrDefault(), mapping);
#pragma warning restore 618
		}

		/// <inheritdoc />
		public virtual IType GetEffectiveReturnType(IEnumerable<IType> argumentTypes, IMapping mapping, bool throwOnError)
		{
			return GetReturnType(argumentTypes, mapping, throwOnError);
		}

		/// <inheritdoc />
		public string Name => name;

		public bool HasArguments
		{
			get { return true; }
		}

		public bool HasParenthesesIfNoArguments
		{
			get { return true; }
		}

		public SqlString Render(IList args, ISessionFactoryImplementor factory)
		{
			//ANSI-SQL92 definition
			//<general set function> ::=
			//<set function type> <leftparen> [ <setquantifier> ] <value expression> <right paren>
			//<set function type> : := AVG | MAX | MIN | SUM | COUNT
			//<setquantifier> ::= DISTINCT | ALL

			if (args.Count < 1 || args.Count > 2)
			{
				throw new QueryException(string.Format("Aggregate {0}(): Not enough parameters (attended from 1 to 2).", name));
			}
			else if ("*".Equals(args[args.Count - 1]) && !acceptAsterisk)
			{
				throw new QueryException(string.Format("Aggregate {0}(): invalid argument '*'.", name));
			}
			SqlStringBuilder cmd = new SqlStringBuilder();
			cmd.Add(name)
				.Add("(");
			if (args.Count > 1)
			{
				object firstArg = args[0];
				if (!"distinct".Equals(firstArg.ToString(), StringComparison.OrdinalIgnoreCase) &&
				    !"all".Equals(firstArg.ToString(), StringComparison.OrdinalIgnoreCase))
				{
					throw new QueryException(string.Format("Aggregate {0}(): token unknow {1}.", name, firstArg));
				}
				cmd.AddObject(firstArg).Add(" ");
			}
			cmd.AddObject(args[args.Count - 1])
				.Add(")");
			return cmd.ToSqlString();
		}

		#endregion

		protected bool TryGetArgumentType(
			IEnumerable<IType> argumentTypes,
			IMapping mapping,
			bool throwOnError,
			out IType argumentType,
			out SqlType sqlType)
		{
			sqlType = null;
			argumentType = null;
			if (argumentTypes.Count() != 1)
			{
				if (throwOnError)
				{
					throw new QueryException($"Invalid number of arguments for {name}()");
				}

				return false;
			}

			argumentType = argumentTypes.First();
			if (argumentType == null)
			{
				// The argument is a parameter (e.g. select avg(:p1) from OrderLine). In that case, if the datatype is needed
				// a QueryException will be thrown in SelectClause class, otherwise the query will be executed
				// (e.g. select case when avg(:p1) > 0 then 1 else 0 end from OrderLine).
				return false;
			}

			SqlType[] sqlTypes;
			try
			{
				sqlTypes = argumentType.SqlTypes(mapping);
			}
			catch (MappingException me)
			{
				if (throwOnError)
				{
					throw new QueryException(me);
				}

				return false;
			}

			if (sqlTypes.Length != 1)
			{
				if (throwOnError)
				{
					throw new QueryException($"Multi-column type can not be in {name}()");
				}

				return false;
			}

			sqlType = sqlTypes[0];
			return true;
		}

		public override string ToString()
		{
			return name;
		}

		#region IFunctionGrammar Members

		bool IFunctionGrammar.IsSeparator(string token)
		{
			return false;
		}

		bool IFunctionGrammar.IsKnownArgument(string token)
		{
			return "distinct".Equals(token, StringComparison.OrdinalIgnoreCase) ||
				"all".Equals(token, StringComparison.OrdinalIgnoreCase);
		}

		#endregion
	}
}
