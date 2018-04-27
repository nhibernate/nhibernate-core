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

		/// <summary>
		/// Creates an instance using the giving token.
		/// </summary>
		/// <param name="sqlOpToken">
		/// The operation token.
		/// </param>
		/// <remarks>
		/// Use this constructor only if the token DOES NOT represent an unary operator.
		/// </remarks>
		public BitwiseNativeOperation(string sqlOpToken)
			: this(sqlOpToken, false)
		{
		}

		/// <summary>
		/// Creates an instance using the giving token and the flag indicating if it is an unary operator.
		/// </summary>
		/// <param name="sqlOpToken">The operation token.</param>
		/// <param name="isNot">Whether the operation is unary or not.</param>
		public BitwiseNativeOperation(string sqlOpToken, bool isNot)
		{
			_sqlOpToken = sqlOpToken;
			_isNot = isNot;
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
		public bool HasParenthesesIfNoArguments => false;

		/// <inheritdoc />
		public SqlString Render(IList args, ISessionFactoryImplementor factory)
		{
			if (args.Count == 0)
				throw new ArgumentException("Function argument list cannot be empty", nameof(args));

			var sqlBuffer = new SqlStringBuilder();

			if (!_isNot)
				AddToBuffer(args[0], sqlBuffer);

			sqlBuffer.Add(" ").Add(_sqlOpToken).Add(" ");
			for (var i = _isNot ? 0 : 1; i < args.Count; i++)
			{
				AddToBuffer(args[i], sqlBuffer);
			}

			return sqlBuffer.ToSqlString();
		}

		#endregion

		private static void AddToBuffer(object arg, SqlStringBuilder buffer)
		{
			if (arg is Parameter || arg is SqlString)
				buffer.AddObject(arg);
			else
				buffer.Add(arg.ToString());
		}
	}
}
