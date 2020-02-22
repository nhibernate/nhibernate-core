using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Dialect.Function
{
	/// <summary>
	/// Support for slightly more general templating than StandardSQLFunction,
	/// with an unlimited number of arguments.
	/// </summary>
	[Serializable]
	public class VarArgsSQLFunction : ISQLFunction, ISQLFunctionExtended
	{
		private readonly string begin;
		private readonly string sep;
		private readonly string end;
		private readonly IType returnType = null;

		public VarArgsSQLFunction(string begin, string sep, string end)
		{
			this.begin = begin;
			this.sep = sep;
			this.end = end;
		}

		public VarArgsSQLFunction(IType type, string begin, string sep, string end)
			: this(begin, sep, end)
		{
			this.returnType = type;
		}

		#region ISQLFunction Members

		// Since v5.3
		[Obsolete("Use GetReturnType method instead.")]
		public virtual IType ReturnType(IType columnType, IMapping mapping)
		{
			return (returnType == null) ? columnType : returnType;
		}

		/// <inheritdoc />
		public virtual IType GetReturnType(IEnumerable<IType> argumentTypes, IMapping mapping, bool throwOnError)
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
		public virtual string FunctionName => null;

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
			SqlStringBuilder buf = new SqlStringBuilder().Add(begin);
			for (int i = 0; i < args.Count; i++)
			{
				buf.AddObject(args[i]);
				if (i < args.Count - 1) buf.Add(sep);
			}
			return buf.Add(end).ToSqlString();
		}

		#endregion
	}
}
