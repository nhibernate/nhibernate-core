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
		private static readonly ISQLFunction SingleParamRound = new SQLFunctionTemplate(null, "round(?1, 0)");

		private static readonly ISQLFunction Round = new StandardSQLFunction("round");

		public IType ReturnType(IType columnType, IMapping mapping) => columnType;

		public bool HasArguments => true;

		public bool HasParenthesesIfNoArguments => true;

		public SqlString Render(IList args, ISessionFactoryImplementor factory)
		{
			return args.Count == 1 ? SingleParamRound.Render(args, factory) : Round.Render(args, factory);
		}

		public override string ToString() => "round";
	}
}
