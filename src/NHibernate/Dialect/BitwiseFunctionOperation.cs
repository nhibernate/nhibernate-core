using System;
using System.Collections;
using NHibernate.Dialect.Function;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Dialect
{
	[Serializable]
	public class BitwiseFunctionOperation : ISQLFunction
	{
		private readonly string _functionName;
		private SqlStringBuilder _sqlBuffer;
		private Queue _args;

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

			Function(); 
			OpenParens(); 
			Arguments(); 
			CloseParens();

			return _sqlBuffer.ToSqlString();
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

		private void Function()
		{
			_sqlBuffer.Add(_functionName);
		}

		private void OpenParens()
		{
			_sqlBuffer.Add("(");
		}

		private void Arguments()
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
	}
}
