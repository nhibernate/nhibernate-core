using System;
using System.Collections;
using NHibernate.Dialect.Function;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Dialect
{
	/// <summary>
	/// Treats bitwise operations as SQL function calls.
	/// </summary>
	[Serializable]
	public class BitwiseFunctionOperation : ISQLFunction
	{
		private readonly string _functionName;
		private SqlStringBuilder _sqlBuffer;
		private Queue _args;

		/// <summary>
		/// Creates an instance of this class using the provided function name
		/// </summary>
		/// <param name="functionName">
		/// The bitwise function name as defined by the SQL-Dialect
		/// </param>
		public BitwiseFunctionOperation(string functionName)
		{
			_functionName = functionName;			
		}

		#region ISQLFunction Members

		public IType ReturnType(IType columnType, IMapping mapping)
		{
			return NHibernateUtil.Int64;
		}

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
			Prepare(args);

			AddFunctionName(); 
			OpenParens(); 
			AddArguments(); 
			CloseParens();

			return SqlResult();
		}

		#endregion

		private void Prepare(IList args)
		{
			_sqlBuffer = new SqlStringBuilder();
			_args = new Queue();
			foreach (var arg in args)
			{
				if (!IsParens(arg.ToString()))
					_args.Enqueue(arg);
			}
		}

		private static bool IsParens(string candidate)
		{
			return candidate == "(" || candidate == ")";
		}

		private void AddFunctionName()
		{
			_sqlBuffer.Add(_functionName);
		}

		private void OpenParens()
		{
			_sqlBuffer.Add("(");
		}

		private void AddArguments()
		{
			while (_args.Count > 0)
			{
				var arg = _args.Dequeue();
				if (arg is Parameter || arg is SqlString)
					_sqlBuffer.AddObject(arg);
				else
					_sqlBuffer.Add(arg.ToString());
				if (_args.Count > 0)
					_sqlBuffer.Add(", ");
			}
		}

		private void CloseParens()
		{
			_sqlBuffer.Add(")");
		}

		private SqlString SqlResult()
		{
			return _sqlBuffer.ToSqlString();
		}
	}
}
