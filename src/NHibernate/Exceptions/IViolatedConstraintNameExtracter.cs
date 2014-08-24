using System.Data.Common;

namespace NHibernate.Exceptions
{
	/// <summary> 
	/// Defines a contract for implementations that can extract the name of a violated
	/// constraint from a SQLException that is the result of that constraint violation. 
	/// </summary>
	public interface IViolatedConstraintNameExtracter
	{
		/// <summary> 
		/// Extract the name of the violated constraint from the given SQLException. 
		/// </summary>
		/// <param name="sqle">The exception that was the result of the constraint violation. </param>
		/// <returns> The extracted constraint name. </returns>
		string ExtractConstraintName(DbException sqle);
	}
}