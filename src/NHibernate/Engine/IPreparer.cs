using System;
using System.Data;

using NHibernate.SqlCommand;

namespace NHibernate.Engine
{
	/// <summary>
	/// Manages retreiving the <c>IDbCommand</c>s for the Entity's NHibernate Commands. 
	/// </summary>
	/// <remarks>
	/// A Prepared IDbCommand is specific to an IDbConnection.  When a prepared IDbCommand
	/// is executed the sql sp_prepexec will be sent to the db.
	/// 
	/// Once an IDbConnection is closed the "handle" to the prepared Command is lost
	/// and the sp_execsql is executed unless the call to Prepare() is made again, then
	/// the sp_prepexec is executed.
	/// </remarks>
	public interface IPreparer
	{
		/// <summary>
		/// Prepares the IDbCommand for the IDbConnection
		/// 
		/// DESIGNTHOUGHT: This is a bit of a false statement now - in order for ADO.NET to Prepare a command the
		/// Size &amp; Precision of the Parameter HAVE to be set.  The current NHibernate codebase does
		/// not do this.  I think to IType we can add a method int[] Size() and int[] Precision and
		/// update each Type accordingly.  The only bad part about that is that for var length Parameters
		/// we will probably be setting the length to the max size and precision - I don't know if that would 
		/// work or if we would end up getting errors because of that.  If we did get errors we would need to
		/// look at a way to override the default values in the class mapping files...
		/// </summary>
		/// <param name="dbCommand">An IDbCommand object to Prepare.</param>
		IDbCommand PrepareCommand(IDbCommand dbCommand);

		/// <summary>
		/// Prepares an IDbCommand for the IDbConnection from a simple SQL string.  The
		/// SQL String can have no parameters.
		/// </summary>
		/// <param name="sql">The String to build and then prepare.</param>
		IDbCommand PrepareCommand(string sql);

		/// <summary>
		/// Prepares an IDbCommand for the IDbConnection from a SqlString.
		/// </summary>
		/// <param name="sqlString">The SqlString to Build and then Prepare</param>
		/// <returns>A prepared IDbCommand.</returns>
		IDbCommand PrepareCommand(SqlString sqlString);

	}
}
