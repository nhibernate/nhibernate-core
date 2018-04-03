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
	/// Represents HQL functions that can have different representations in different SQL dialects.
	/// E.g. in HQL we can define function <code>concat(?1, ?2)</code> to concatenate two strings 
	/// p1 and p2. Target SQL function will be dialect-specific, e.g. <code>(?1 || ?2)</code> for 
	/// Oracle, <code>concat(?1, ?2)</code> for MySql, <code>(?1 + ?2)</code> for MS SQL.
	/// Each dialect will define a template as a string (exactly like above) marking function 
	/// parameters with '?' followed by parameter's index (first index is 1).
	/// </summary>
	[Serializable]
	public class SQLFunctionTemplateWithRequiredParameters : SQLFunctionTemplate
	{
		private readonly object[] _requiredArgs;

		public SQLFunctionTemplateWithRequiredParameters(IType type, string template, object[] requiredArgs) : base(type, template)
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
