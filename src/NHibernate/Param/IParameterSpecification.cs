using System.Data;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Param
{
	/// <summary>
	/// Maintains information relating to parameters which need to get bound into a
	/// JDBC {@link PreparedStatement}.
	/// Author: Steve Ebersole
	/// Ported by: Steve Strong
	/// </summary>
	public interface IParameterSpecification
	{
		/// <summary>
		/// Bind the appropriate value into the given statement at the specified position.
		/// </summary>
		/// <param name="statement">The statement into which the value should be bound.</param>
		/// <param name="qp">The defined values for the current query execution.</param>
		/// <param name="session">The session against which the current execution is occuring.</param>
		/// <param name="position">The position from which to start binding value(s).</param>
		/// <returns>The number of sql bind positions "eaten" by this bind operation.</returns>
		int Bind(IDbCommand statement, QueryParameters qp, ISessionImplementor session, int position);

		/// <summary>
		/// Get or set the type which we are expeting for a bind into this parameter based
		/// on translated contextual information.
		/// </summary>
		IType ExpectedType { get; set; }

		/// <summary>
		/// Render this parameter into displayable info (for logging, etc).
		/// </summary>
		/// <returns>The displayable info</returns>
		string RenderDisplayInfo();
	}
}