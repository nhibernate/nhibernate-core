using System.Collections;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;
using System;

namespace NHibernate.Dialect.Function
{
	/// <summary>
	/// Provides a round implementation that supports single parameter round by translating to two parameters round.
	/// </summary>
	[Serializable]
	public class RoundEmulatingSingleParameterFunction : ISQLFunction
	{
		private readonly ISQLFunction _singleParamRound;
		private readonly ISQLFunction _round;
		private readonly string _name;

		/// <summary>
		/// Constructs a <c>RoundEmulatingSingleParameterFunction</c>.
		/// </summary>
		/// <param name="name">The SQL name of the round function to call.</param>
		public RoundEmulatingSingleParameterFunction(string name)
		{
			_singleParamRound = new SQLFunctionTemplate(null, $"{name}(?1, 0)");
			_round = new StandardSQLFunction(name);
			_name = name;
		}

		public IType ReturnType(IType columnType, IMapping mapping) => columnType;

		public bool HasArguments => true;

		public bool HasParenthesesIfNoArguments => true;

		public SqlString Render(IList args, ISessionFactoryImplementor factory)
		{
			return args.Count == 1 ? _singleParamRound.Render(args, factory) : _round.Render(args, factory);
		}

		public override string ToString() => _name;
	}
}
