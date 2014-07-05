using System;
using System.Collections;
using NHibernate.Dialect.Function;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Dialect
{
	[Serializable]
	public class NativeBitwiseOperation : ISQLFunction
	{
		private readonly string _sqlToken;
        private Queue _args;
		private SqlStringBuilder _sqlBuffer;

		public NativeBitwiseOperation(string sqlToken)
		{
			_sqlToken = sqlToken;
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
			get { return false; }
		}

		public SqlString Render(IList args, ISessionFactoryImplementor factory)
		{
			Prepare(args);
			if (_sqlToken != "~")
				AddFirstArgument();
			AddToken();
			AddRestOfArguments();

			return _sqlBuffer.ToSqlString();
		}

		#endregion

		private void Prepare(IList args)
		{
			_sqlBuffer = new SqlStringBuilder();
			_args = new Queue(args);
		}

		private void AddFirstArgument()
		{
			AddToBuffer(_args.Dequeue());
		}

		private void AddToken()
		{
			AddToBuffer(string.Format(" {0} ", _sqlToken));
		}

		private void AddRestOfArguments()
		{
			while (_args.Count > 0)
			{
				AddToBuffer(_args.Dequeue());
			}
		}

		private void AddToBuffer(object arg)
		{
			if (arg is Parameter || arg is SqlString)
				_sqlBuffer.AddObject(arg);
			else
				_sqlBuffer.Add(arg.ToString());
		}
	}
}
