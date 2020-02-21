using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
		/// <returns>SQL fragment for the function.</returns>
		SqlString Render(IList args, ISessionFactoryImplementor factory);
	}

	// 6.0 TODO: Remove
	internal static class SQLFunctionExtensions
	{
		/// <summary>
		/// Get the type that will be effectively returned by the underlying database.
		/// </summary>
		/// <param name="sqlFunction">The sql function.</param>
		/// <param name="argumentTypes">The types of arguments.</param>
		/// <param name="mapping">The mapping for retrieving the argument sql types.</param>
		/// <param name="throwOnError">Whether to throw when the number of arguments is invalid or they are not supported.</param>
		/// <returns>The type returned by the underlying database or <see langword="null"/> when the number of arguments
		/// is invalid or they are not supported.</returns>
		/// <exception cref="QueryException">When <paramref name="throwOnError"/> is set to <see langword="true"/> and the
		/// number of arguments is invalid or they are not supported.</exception>
		public static IType GetEffectiveReturnType(
			this ISQLFunction sqlFunction,
			IEnumerable<IType> argumentTypes,
			IMapping mapping,
			bool throwOnError)
		{
			if (!(sqlFunction is ISQLFunctionExtended extendedSqlFunction))
			{
				try
				{
#pragma warning disable 618
					return sqlFunction.ReturnType(argumentTypes.FirstOrDefault(), mapping);
#pragma warning restore 618
				}
				catch (QueryException)
				{
					if (throwOnError)
					{
						throw;
					}

					return null;
				}
			}

			return extendedSqlFunction.GetEffectiveReturnType(argumentTypes, mapping, throwOnError);
		}
	}
}
