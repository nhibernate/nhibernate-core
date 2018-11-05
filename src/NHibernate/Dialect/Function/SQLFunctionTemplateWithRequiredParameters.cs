using System.Collections;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;
using System;

namespace NHibernate.Dialect.Function
{
	/// <summary>
	/// A template-based SQL function which substitutes required missing parameters with defaults.
	/// </summary>
	[Serializable]
	public class SQLFunctionTemplateWithRequiredParameters : SQLFunctionTemplate
	{
		private readonly object[] _requiredArgs;

		public SQLFunctionTemplateWithRequiredParameters(IType type, string template, object[] requiredArgs) : base(type, template)
		{
			_requiredArgs = requiredArgs;
		}

		public SQLFunctionTemplateWithRequiredParameters(IType type, string template, object[] requiredArgs, bool hasParenthesesIfNoArgs) : base(type, template, hasParenthesesIfNoArgs)
		{
			_requiredArgs = requiredArgs;
		}

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
