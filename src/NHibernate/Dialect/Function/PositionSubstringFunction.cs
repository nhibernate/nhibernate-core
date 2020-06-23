using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Dialect.Function
{
	/// <summary>
	/// Emulation of locate() on PostgreSQL
	/// </summary>
	[Serializable]
	public class PositionSubstringFunction : ISQLFunction, ISQLFunctionExtended
	{
		public PositionSubstringFunction()
		{
		}

		#region ISQLFunction Members

		// Since v5.3
		[Obsolete("Use GetReturnType method instead.")]
		public IType ReturnType(IType columnType, IMapping mapping)
		{
			return NHibernateUtil.Int32;
		}

		/// <inheritdoc />
		public IType GetReturnType(IEnumerable<IType> argumentTypes, IMapping mapping, bool throwOnError)
		{
#pragma warning disable 618
			return ReturnType(argumentTypes.FirstOrDefault(), mapping);
#pragma warning restore 618
		}

		/// <inheritdoc />
		public virtual IType GetEffectiveReturnType(IEnumerable<IType> argumentTypes, IMapping mapping, bool throwOnError)
		{
			return GetReturnType(argumentTypes, mapping, throwOnError);
		}

		/// <inheritdoc />
		public string Name => "position";

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
				buf.Add("(case ");
				RenderPositionInSubstring(buf, pattern, orgString, start);
				buf.Add(" when 0 then 0 else (");
				RenderPositionInSubstring(buf, pattern, orgString, start);
				buf.Add("+")
				   .AddObject(start)
				   .Add("-1) end)");
			}
			else
			{
				buf.Add("position(")
				.AddObject(pattern)
				.Add(" in ")
				.AddObject(orgString)
				.Add(")");
			}
			return buf.ToSqlString();
		}

		private static void RenderPositionInSubstring(SqlStringBuilder buf, object pattern, object orgString, object start)
		{
			buf.Add("position(")
			   .AddObject(pattern)
			   .Add(" in substring(")
			   .AddObject(orgString)
			   .Add(", ")
			   .AddObject(start)
			   .Add("))");
		}

		#endregion
	}
}
