using NHibernate.Engine;
using NHibernate.Hql;
using NHibernate.Type;

namespace NHibernate.Dialect
{
	/// <summary>
	/// Provides support routines for the HQL functions as used
	/// in the various SQL Dialects
	///
	/// Provides an interface for supporting various HQL functions that are
	/// translated to SQL. The Dialect and its sub-classes use this interface to
	/// provide details required for processing of the function.
	/// </summary>
	public interface ISQLFunction
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="columnType"></param>
		/// <param name="mapping"></param>
		/// <returns></returns>
		IType ReturnType( IType columnType, IMapping mapping );

		/// <summary>
		/// 
		/// </summary>
		bool HasArguments { get; }

		/// <summary>
		/// 
		/// </summary>
		bool HasParenthesesIfNoArguments { get; }
	}
}
