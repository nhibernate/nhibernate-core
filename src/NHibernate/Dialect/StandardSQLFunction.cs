using NHibernate.Engine;
using NHibernate.Hql;
using NHibernate.Type;

namespace NHibernate.Dialect
{
	/// <summary>
	/// Provides a standard implementation that supports the majority of the HQL
	/// functions that are translated to SQL.
	/// </summary>
	/// <remarks>
	/// The Dialect and its sub-classes use this class to provide details required
	/// for processing of the associated function.
	/// </remarks>
	public class StandardSQLFunction : ISQLFunction
		{
		private IType returnType = null;

		/// <summary>
		/// 
		/// </summary>
		public StandardSQLFunction()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="typeValue"></param>
		public StandardSQLFunction( IType typeValue )
		{
			returnType = typeValue;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="columnType"></param>
		/// <param name="mapping"></param>
		/// <returns></returns>
		public IType ReturnType( IType columnType, IMapping mapping )
		{
			if ( returnType == null )
				return columnType;
			return returnType;
		}

		/// <summary>
		/// 
		/// </summary>
		public bool HasArguments
		{
			get { return true; }
		}

		/// <summary>
		/// 
		/// </summary>
		public bool HasParenthesesIfNoArguments
		{
			get { return true; }
		}
	}
}
