using System;
using System.Collections;
using System.Text;
using NHibernate.Engine;
using NHibernate.SqlCommand;
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
	[Serializable]
	public class AnsiSubstringFunction : ISQLFunction
	{
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

		public SqlString Render(IList args, ISessionFactoryImplementor factory)
		{
			if (args.Count < 2 || args.Count > 3)
			{
				throw new QueryException("substring(): Incorrect number of parameters (expected 2 or 3, got " + args.Count + ")");
			}
			SqlStringBuilder cmd = new SqlStringBuilder();
			cmd.Add("substring(")
				.AddObject(args[0])
				.Add(" from ")
				.AddObject(args[1]);
			if (args.Count > 2)
			{
				cmd.Add(" for ")
					.AddObject(args[2]);
			}
			cmd.Add(")");
			return cmd.ToSqlString();
		}

		#endregion
	}
}
