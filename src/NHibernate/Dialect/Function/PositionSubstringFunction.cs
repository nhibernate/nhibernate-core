using System;
using System.Text;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Dialect.Function
{
	/// <summary>
	/// Emulation of locate() on PostgreSQL
	/// </summary>
	public class PositionSubstringFunction: ISQLFunction
	{
		public PositionSubstringFunction() { }

		#region ISQLFunction Members

		public IType ReturnType(IType columnType, IMapping mapping)
		{
			return NHibernateUtil.Int32;
		}

		public bool HasArguments
		{
			get { return true; }
		}

		public bool HasParenthesesIfNoArguments
		{
			get { return true; }
		}

		public string Render(System.Collections.IList args, ISessionFactoryImplementor factory)
		{
			// DONE: QueryException if args.Count<2 (not present in H3.2) 
			if (args.Count < 2)
			{
				throw new QueryException("position(): Not enough parameters (attended from 2 to 3).");
			}
			bool threeArgs = args.Count > 2;
			object pattern = args[0];
			object orgString = args[1];
			object start = threeArgs ? args[2] : null;

			StringBuilder buf = new StringBuilder();
			if (threeArgs)
			{
				buf.Append('(');
			}
			buf.Append("position(")
				.Append( pattern )
				.Append(" in ");
			if (threeArgs)
			{
				buf.Append("substring(");
			}
			buf.Append(orgString);
			if (threeArgs)
			{
				buf.Append(", ")
					.Append(start)
					.Append(')');
			}
			buf.Append(')');
			if (threeArgs)
			{
				buf.Append('+')
					.Append(start)
					.Append("-1)");
			}
			return buf.ToString();
		}

		#endregion
	}
}
