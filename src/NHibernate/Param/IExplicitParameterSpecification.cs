using NHibernate.Engine;

namespace NHibernate.Param
{
	/// <summary>
	/// An additional contract for parameters which originate from parameters explicitly encountered in the source statement
	/// (HQL or native-SQL).
	/// Author: Steve Ebersole
	/// Ported by: Steve Strong
	/// </summary>
	public interface IExplicitParameterSpecification : IParameterSpecification 
	{
		/// <summary>
		/// Retrieves the line number on which this parameter occurs in the source query.
		/// </summary>
		int SourceLine { get; }

		/// <summary>
		/// Retrieves the column number (within the {@link #getSourceLine()}) where this parameter occurs.
		/// </summary>
		int SourceColumn { get; }

		/// <summary>
		/// Explicit parameters may have no set the <see cref="IParameterSpecification.ExpectedType"/> during query parse.
		/// </summary>
		/// <param name="queryParameters">The defined values for the current query execution.</param>
		/// <remarks>
		/// This method should be removed when the parameter type is inferred during the parse.
		/// </remarks>
		void SetEffectiveType(QueryParameters queryParameters);
	}
}