using System.Collections;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Dialect.Function
{
	/// <summary>
	/// Summary description for NoArgSQLFunction.
	/// </summary>
	public class NoArgSQLFunction : ISQLFunction
	{
		private IType returnType = null;
		private readonly string name;
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

		public string Render(IList args, ISessionFactoryImplementor factory)
		{
			if (args.Count > 0)
			{
				throw new QueryException("function takes no arguments: " + name);
			}
			return hasParenthesesIfNoArguments ? name + "()" : name;
		}

		#endregion
	}
}