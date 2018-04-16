using NHibernate.Engine;
using NHibernate.SqlCommand;

namespace NHibernate.Id.Insert
{
	/// <summary> 
	/// Responsible for handling delegation relating to variants in how
	/// insert-generated-identifier generator strategies dictate processing:
	/// <ul>
	/// <li>building the sql insert statement</li>
	/// <li>determination of the generated identifier value</li>
	/// </ul> 
	/// </summary>
	public partial interface IInsertGeneratedIdentifierDelegate
	{
		/// <summary> 
		/// Build a <see cref="NHibernate.SqlCommand.SqlInsertBuilder"/> specific to the delegate's mode
		/// of handling generated key values. 
		/// </summary>
		/// <returns> The insert object. </returns>
		IdentifierGeneratingInsert PrepareIdentifierGeneratingInsert();

		/// <summary> 
		/// Perform the indicated insert SQL statement and determine the identifier value generated. 
		/// </summary>
		/// <param name="insertSQL"> </param>
		/// <param name="session"> </param>
		/// <param name="binder"> </param>
		/// <returns> The generated identifier value. </returns>
		object PerformInsert(SqlCommandInfo insertSQL, ISessionImplementor session, IBinder binder);
	}
}