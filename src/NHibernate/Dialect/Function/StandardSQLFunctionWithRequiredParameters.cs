using System;
using System.Collections;
using System.Linq;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Dialect.Function
{
	/// <summary>
	/// A SQL function which substitutes required missing parameters with defaults.
	/// </summary>
	[Serializable]
	internal class StandardSQLFunctionWithRequiredParameters : StandardSQLFunction
	{
		private readonly object[] _requiredArgs;

		/// <inheritdoc />
		public StandardSQLFunctionWithRequiredParameters(string name, object[] requiredArgs)
			: base(name)
		{
			_requiredArgs = requiredArgs;
		}

		/// <inheritdoc />
		public StandardSQLFunctionWithRequiredParameters(string name, IType typeValue, object[] requiredArgs)
			: base(name, typeValue)
		{
			_requiredArgs = requiredArgs;
		}

		/// <inheritdoc />
		public override SqlString Render(IList args, ISessionFactoryImplementor factory)
		{
			var combinedArgs =
				args.Cast<object>()
					.Concat(_requiredArgs.Skip(args.Count))
					.ToArray();
			return base.Render(combinedArgs, factory);
		}
	}
}
