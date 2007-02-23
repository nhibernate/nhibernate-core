using System;
using System.Collections;
using System.Text;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Dialect.Function
{
	/// <summary>
	/// ANSI-SQL substring  
	/// Documented in:
	/// ANSI X3.135-1992
	/// American National Standard for Information Systems - Database Language - SQL
	/// </summary>
	/// <remarks>
	/// Syntax:
	///<![CDATA[
	/// <character substring function> ::=
	/// SUBSTRING <left paren> <character value expression> FROM < start position>
	/// [ FOR <string length> ] <right paren>
	///]]>
	/// </remarks>
	public class AnsiSubstringFunction : ISQLFunction
	{
		public AnsiSubstringFunction()
		{
		}

		#region ISQLFunction Members

		public IType ReturnType(IType columnType, IMapping mapping)
		{
			return NHibernateUtil.String;
		}

		public bool HasArguments
		{
			get { return true; }
		}

		public bool HasParenthesesIfNoArguments
		{
			get { return true; }
		}

		public string Render(IList args, ISessionFactoryImplementor factory)
		{
			if (args.Count < 2 || args.Count > 3)
			{
				throw new QueryException("substring(): Not enough parameters (attended from 2 to 3).");
			}
			StringBuilder cmd = new StringBuilder();
			cmd.Append("substring(")
				.Append(args[0])
				.Append(" from ")
				.Append(args[1]);
			if (args.Count > 2)
			{
				cmd.Append(" for ")
					.Append(args[2]);
			}
			cmd.Append(')');
			return cmd.ToString();
		}

		#endregion
	}
}