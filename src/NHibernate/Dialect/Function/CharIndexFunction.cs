using System;
using System.Collections;
using System.Text;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Dialect.Function
{
	/// <summary>
	/// Emulation of locate() on Sybase
	/// </summary>
	public class CharIndexFunction : ISQLFunction
	{
		public CharIndexFunction()
		{
		}

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

		public string Render(IList args, ISessionFactoryImplementor factory)
		{
			// TODO: QueryException if args.Count<2 (not present in H3.2) 
			bool threeArgs = args.Count > 2;
			object pattern = args[0];
			object orgString = args[1];
			object start = threeArgs ? args[2] : null;

			StringBuilder buf = new StringBuilder();
			buf.Append("charindex(")
				.Append(pattern)
				.Append(", ");
			if (threeArgs)
			{
				buf.Append("right(");
			}
			buf.Append(orgString);
			if (threeArgs)
			{
				buf.Append(", char_length(")
					.Append(orgString)
					.Append(")-(")
					.Append(start)
					.Append("-1))");
			}
			buf.Append(')');
			return buf.ToString();
		}

		#endregion
	}
}