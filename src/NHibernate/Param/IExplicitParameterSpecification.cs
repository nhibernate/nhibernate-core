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
	}
}