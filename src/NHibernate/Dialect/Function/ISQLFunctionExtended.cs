using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Dialect.Function
{
	// 6.0 TODO: Merge into ISQLFunction
	internal interface ISQLFunctionExtended : ISQLFunction
	{
		/// <summary>
		/// The function name or <see langword="null"/> when multiple functions/operators/statements are used.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Get the function general return type, ignoring underlying database specifics.
		/// </summary>
		/// <param name="argumentTypes">The types of arguments.</param>
		/// <param name="mapping">The mapping for retrieving the argument sql types.</param>
		/// <param name="throwOnError">Whether to throw when the number of arguments is invalid or they are not supported.</param>
		/// <returns>The type returned by the underlying database or <see langword="null"/> when the number of arguments
		/// is invalid or they are not supported.</returns>
		IType GetReturnType(IEnumerable<IType> argumentTypes, IMapping mapping, bool throwOnError);

		/// <summary>
		/// Get the type that will be effectively returned by the underlying database.
		/// </summary>
		/// <param name="argumentTypes">The types of arguments.</param>
		/// <param name="mapping">The mapping for retrieving the argument sql types.</param>
		/// <param name="throwOnError">Whether to throw when the number of arguments is invalid or they are not supported.</param>
		/// <returns>The type returned by the underlying database or <see langword="null"/> when the number of arguments
		/// is invalid or they are not supported.</returns>
		/// <exception cref="QueryException">When <paramref name="throwOnError"/> is set to <see langword="true"/> and the
		/// number of arguments is invalid or they are not supported.</exception>
		IType GetEffectiveReturnType(IEnumerable<IType> argumentTypes, IMapping mapping, bool throwOnError);
	}
}
