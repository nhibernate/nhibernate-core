using NHibernate.Engine;
using NHibernate.Hql;
using NHibernate.Type;

namespace NHibernate.Dialect
{
	/// <summary>
	/// Summary description for NoArgSQLFunction.
	/// </summary>
	public class NoArgSQLFunction : ISQLFunction
	{
		private IType returnType;
		private bool hasParenthesesIfNoArguments;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="returnType"></param>
		public NoArgSQLFunction( IType returnType ) : this( returnType, true )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="returnType"></param>
		/// <param name="hasParenthesesIfNoArguments"></param>
		public NoArgSQLFunction( IType returnType, bool hasParenthesesIfNoArguments )
		{
			this.returnType = returnType;
			this.hasParenthesesIfNoArguments = hasParenthesesIfNoArguments;
		}

		#region ISQLFunction Members

		/// <summary>
		/// 
		/// </summary>
		/// <param name="columnType"></param>
		/// <param name="mapping"></param>
		/// <returns></returns>
		public IType ReturnType(IType columnType, IMapping mapping)
		{
			return returnType;
		}

		/// <summary>
		/// 
		/// </summary>
		public bool HasArguments
		{
			get { return false; }
		}

		/// <summary>
		/// 
		/// </summary>
		public bool HasParenthesesIfNoArguments
		{
			get { return hasParenthesesIfNoArguments; }
		}

		#endregion
	}
}
