using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Hql
{
	/// <summary>
	/// Provides support routines for the HQL functions as used in the various
	/// SQL Dialects.
	/// </summary>
	/// <remarks>
	/// Provides an interface for supporting various HQL functions that are translated
	/// to SQL.  The Dialect and its sub-classes use this interface to
	/// provide details required for processing of the function.
	/// </remarks>
	public interface IQueryFunctionInfo
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="columnType"></param>
		/// <param name="mapping"></param>
		/// <returns></returns>
		IType QueryFunctionType( IType columnType, IMapping mapping );

		/// <summary>
		/// 
		/// </summary>
		bool IsFunctionArgs { get; }

		/// <summary>
		/// 
		/// </summary>
		bool IsFunctionNoArgsUseParanthesis { get; }
	}
}