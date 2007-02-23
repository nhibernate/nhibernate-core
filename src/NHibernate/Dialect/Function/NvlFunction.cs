using System;
using System.Collections;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Dialect.Function
{
	/// <summary>
	/// Emulation of coalesce() on Oracle, using multiple nvl() calls
	/// </summary>
	public class NvlFunction : ISQLFunction
	{
		public NvlFunction()
		{
		}

		#region ISQLFunction Members

		public IType ReturnType(IType columnType, IMapping mapping)
		{
			return columnType;
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
				return last.ToString();
			}
			object secondLast = args[lastIndex - 1];
			string nvl = "nvl(" + secondLast + ", " + last + ")";
			args[lastIndex - 1] = nvl;
			return Render(args, factory);
		}

		#endregion
	}
}