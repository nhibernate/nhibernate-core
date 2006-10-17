using System;

namespace NHibernate.Dialect.Function
{
	/// <summary>
	/// Emulation of coalesce() on Oracle, using multiple nvl() calls
	/// </summary>
	public class NvlFunction: ISQLFunction
	{
		public NvlFunction() { }

		#region ISQLFunction Members

		public NHibernate.Type.IType ReturnType(NHibernate.Type.IType columnType, NHibernate.Engine.IMapping mapping)
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

		public string Render(System.Collections.IList args, NHibernate.Engine.ISessionFactoryImplementor factory)
		{
			// TODO: QueryException if args.Count==0 (not present in H3.2)
			int lastIndex = args.Count - 1;
			object last = args[lastIndex];
			args.RemoveAt(lastIndex);
			if (lastIndex == 0)
			{
				return last.ToString();
			}
			object secondLast = args[lastIndex - 1];
			string nvl = "nvl(" + secondLast + ", " + last + ")";
			args[lastIndex - 1]= nvl;
			return Render(args, factory);
		}

		#endregion
	}
}
