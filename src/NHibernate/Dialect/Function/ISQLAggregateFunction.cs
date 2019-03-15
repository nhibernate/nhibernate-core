using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Dialect.Function
{
	/// <inheritdoc />
	internal interface ISQLAggregateFunction : ISQLFunction
	{
		/// <summary>
		/// The name of the aggregate function.
		/// </summary>
		string FunctionName { get; }

		/// <summary>
		/// Get the type that will be effectively returned by the underlying database.
		/// </summary>
		/// <param name="argumentType">The type of the first argument</param>
		/// <param name="mapping">The mapping for retrieving the argument sql types.</param>
		/// <returns></returns>
		IType GetActualReturnType(IType argumentType, IMapping mapping);
	}
}
