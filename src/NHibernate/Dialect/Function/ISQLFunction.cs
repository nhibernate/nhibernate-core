using System.Collections;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Dialect.Function
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
		/// The function return type
		/// </summary>
		/// <param name="columnType">The type of the first argument</param>
		/// <param name="mapping"></param>
		/// <returns></returns>
		IType ReturnType(IType columnType, IMapping mapping);

		/// <summary>
		/// Does this function have any arguments?
		/// </summary>
		bool HasArguments { get; }

		/// <summary>
		/// If there are no arguments, are parens required?
		/// </summary>
		bool HasParenthesesIfNoArguments { get; }

		/// <summary>
		/// Render the function call as SQL.
		/// </summary>
		/// <param name="args">List of arguments</param>
		/// <param name="factory"></param>
		/// <returns>SQL fragment for the fuction.</returns>
		SqlString Render(IList args, ISessionFactoryImplementor factory);
	}
}
