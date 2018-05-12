using System;
using System.Collections;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;

// 6.0 TODO: remove NHibernate.Dialect.BitwiseFunctionOperation,
// and remove "Function." prefix where the non obsolete one is used.
namespace NHibernate.Dialect
{
	/// <inheritdoc />
	[Serializable]
	// Since 5.2
	[Obsolete("Use NHibernate.Dialect.Function.BitwiseFunctionOperation instead")]
	public class BitwiseFunctionOperation : Function.BitwiseFunctionOperation
	{
		/// <inheritdoc />
		public BitwiseFunctionOperation(string functionName): base(functionName)
		{
		}
	}
}

namespace NHibernate.Dialect.Function
{
	/// <summary>
	/// Treats bitwise operations as SQL function calls.
	/// </summary>
	[Serializable]
	public class BitwiseFunctionOperation : ISQLFunction
	{
		private readonly string _functionName;

		/// <summary>
		/// Creates an instance of this class using the provided function name.
		/// </summary>
		/// <param name="functionName">
		/// The bitwise function name as defined by the SQL-Dialect.
		/// </param>
		public BitwiseFunctionOperation(string functionName)
		{
			_functionName = functionName;
		}

		#region ISQLFunction Members

		/// <inheritdoc />
		public IType ReturnType(IType columnType, IMapping mapping)
		{
			return NHibernateUtil.Int64;
		}

		/// <inheritdoc />
		public bool HasArguments => true;

		/// <inheritdoc />
		public bool HasParenthesesIfNoArguments => true;

		/// <inheritdoc />
		public SqlString Render(IList args, ISessionFactoryImplementor factory)
		{
			var sqlBuffer = new SqlStringBuilder();

			sqlBuffer.Add(_functionName);
			sqlBuffer.Add("(");
			foreach (var arg in args)
			{
				// The actual second argument may be surrounded by parentesis as additional arguments.
				// They have to be ignored, otherwise it would emit "functionName(firstArg, (, secondArg, ))"
				if (IsParens(arg.ToString()))
					continue;
				if (arg is Parameter || arg is SqlString)
					sqlBuffer.AddObject(arg);
				else
					sqlBuffer.Add(arg.ToString());
				sqlBuffer.Add(", ");
			}

			sqlBuffer.RemoveAt(sqlBuffer.Count - 1);
			sqlBuffer.Add(")");

			return sqlBuffer.ToSqlString();
		}

		#endregion

		private static bool IsParens(string candidate)
		{
			return candidate == "(" || candidate == ")";
		}
	}
}
