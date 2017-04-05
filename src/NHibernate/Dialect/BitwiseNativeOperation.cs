using System;
using System.Collections;
using NHibernate.Dialect.Function;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Dialect
{
	/// <summary>
	/// Treats bitwise operations as native operations.
	/// </summary>
	[Serializable]
	public class BitwiseNativeOperation : ISQLFunction
	{
		private readonly string _sqlOpToken;
		private readonly bool _isNot;
        private Queue _args;
		private SqlStringBuilder _sqlBuffer;

		/// <summary>
		/// creates an instance using the giving token
		/// </summary>
		/// <param name="sqlOpToken">
		/// The operation token
		/// </param>
		/// <remarks>
		/// Use this constructor only if the token DOES NOT represent a NOT-Operation
		/// </remarks>
		public BitwiseNativeOperation(string sqlOpToken)
			: this(sqlOpToken, false)
		{
		}

        /// <summary>
		/// creates an instance using the giving token and the flag indicating a NOT-Operation
        /// </summary>
        /// <param name="sqlOpToken"></param>
        /// <param name="isNot"></param>
		public BitwiseNativeOperation(string sqlOpToken, bool isNot)
		{
			_sqlOpToken = sqlOpToken;
			_isNot = isNot;
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
			if (_isNot == false)
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
			AddToBuffer(string.Format(" {0} ", _sqlOpToken));
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
