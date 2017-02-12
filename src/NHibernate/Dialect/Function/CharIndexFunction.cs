using System;
using System.Collections;
using System.Text;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Dialect.Function
{
	/// <summary>
	/// Emulation of locate() on Sybase
	/// </summary>
	[Serializable]
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

		public SqlString Render(IList args, ISessionFactoryImplementor factory)
		{
			// TODO: QueryException if args.Count<2 (not present in H3.2) 
			bool threeArgs = args.Count > 2;
			object pattern = args[0];
			object orgString = args[1];
			object start = threeArgs ? args[2] : null;

			SqlStringBuilder buf = new SqlStringBuilder();
			if (threeArgs)
			{
				buf.Add("(case ");
				RenderPositionInSubstring(buf, pattern, orgString, start);
				buf.Add(" when 0 then 0 else (");
				RenderPositionInSubstring(buf, pattern, orgString, start);
				buf.Add("+(")
				   .AddObject(start)
				   .Add("-1)) end)");
			}
			else
			{
				buf.Add("charindex(")
				.AddObject(pattern)
				.Add(", ")
				.AddObject(orgString)
				.Add(")");
			}
			return buf.ToSqlString();
		}

		private static void RenderPositionInSubstring(SqlStringBuilder buf, object pattern, object orgString, object start)
		{
			buf.Add("charindex(")
			   .AddObject(pattern)
			   .Add(", right(")
			   .AddObject(orgString)
			   .Add(", char_length(")
			   .AddObject(start)
			   .Add(")))");
		}

		#endregion
	}
}
