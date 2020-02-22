using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Dialect.Function
{
	/// <summary>
	/// Emulation of coalesce() on Oracle, using multiple nvl() calls
	/// </summary>
	[Serializable]
	public class NvlFunction : ISQLFunction, ISQLFunctionExtended
	{
		public NvlFunction()
		{
		}

		#region ISQLFunction Members

		// Since v5.3
		[Obsolete("Use GetReturnType method instead.")]
		public IType ReturnType(IType columnType, IMapping mapping)
		{
			return columnType;
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
		public string FunctionName => "nvl";

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
			// DONE: QueryException if args.Count==0 (not present in H3.2)
			if (args.Count == 0)
			{
				throw new QueryException("nvl(): Not enough parameters.");
			}
			int lastIndex = args.Count - 1;
			object last = args[lastIndex];
			args.RemoveAt(lastIndex);
			if (lastIndex == 0)
			{
				return new SqlString(last);
			}
			object secondLast = args[lastIndex - 1];
			SqlString nvl = new SqlString("nvl(", secondLast, ", ", last, ")");
			args[lastIndex - 1] = nvl;
			return Render(args, factory);
		}

		#endregion
	}
}
