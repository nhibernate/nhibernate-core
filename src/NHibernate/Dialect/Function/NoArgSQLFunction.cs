using System.Collections;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Dialect.Function
{
	/// <summary>
	/// Summary description for NoArgSQLFunction.
	/// </summary>
	public class NoArgSQLFunction : ISQLFunction
	{
		protected readonly IType returnType = null;
		protected readonly string name;
		private readonly bool hasParenthesesIfNoArguments;

		public NoArgSQLFunction(string name, IType returnType) : this(name, returnType, true)
		{
		}

		public NoArgSQLFunction(string name, IType returnType, bool hasParenthesesIfNoArguments)
		{
			this.name = name;
			this.returnType = returnType;
			this.hasParenthesesIfNoArguments = hasParenthesesIfNoArguments;
		}

		protected IType FunctionReturnType
		{
			get { return returnType; }
		}

		protected string Name
		{
			get { return name; }
		}

		#region ISQLFunction Members

		public IType ReturnType(IType columnType, IMapping mapping)
		{
			return returnType;
		}

		public bool HasArguments
		{
			get { return false; }
		}

		public bool HasParenthesesIfNoArguments
		{
			get { return hasParenthesesIfNoArguments; }
		}

		public virtual SqlString Render(IList args, ISessionFactoryImplementor factory)
		{
			if (args.Count > 0)
			{
				throw new QueryException("function takes no arguments: " + name);
			}
			SqlStringBuilder buf = new SqlStringBuilder(2);
			buf.Add(name);
			if (hasParenthesesIfNoArguments)
			{
				buf.Add("()");
			}
			return buf.ToSqlString();
		}

		#endregion
	}
}
