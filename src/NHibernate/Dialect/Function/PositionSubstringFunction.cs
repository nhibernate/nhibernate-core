using System;
using System.Collections;
using System.Text;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Dialect.Function
{
	/// <summary>
	/// Emulation of locate() on PostgreSQL
	/// </summary>
	[Serializable]
	public class PositionSubstringFunction : ISQLFunction
	{
		public PositionSubstringFunction()
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

		public SqlString Render(IList args, ISessionFactoryImplementor factory)
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

			SqlStringBuilder buf = new SqlStringBuilder();
			if (threeArgs)
			{
				buf.Add("(");
			}
			buf.Add("position(")
				.AddObject(pattern)
				.Add(" in ");
			if (threeArgs)
			{
				buf.Add("substring(");
			}
			buf.AddObject(orgString);
			if (threeArgs)
			{
				buf.Add(", ")
					.AddObject(start)
					.Add(")");
			}
			buf.Add(")");
			if (threeArgs)
			{
				buf.Add("+")
					.AddObject(start)
					.Add("-1)");
			}
			return buf.ToSqlString();
		}

		#endregion
	}
}
