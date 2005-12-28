using System;
using System.Data;

using NHibernate.Dialect;
using NHibernate.SqlCommand;

namespace NHibernate.Driver 
{
	/// <summary>
	/// A NHibernate Driver for using the Oracle.DataAccess DataProvider
	/// </summary>
	/// <remarks>
	/// Code was contributed by <a href="http://sourceforge.net/users/jemcalgary/">James Mills</a>
	/// on the NHibernate forums in this 
	/// <a href="http://sourceforge.net/forum/message.php?msg_id=2952662">post</a>.
	/// </remarks>
	public class OracleDataClientDriver : ReflectionBasedDriver
	{
		/// <summary>
		/// Initializes a new instance of <see cref="OracleDataClientDriver"/>.
		/// </summary>
		/// <exception cref="HibernateException">
		/// Thrown when the <c>Oracle.DataAccess</c> assembly can not be loaded.
		/// </exception>
		public OracleDataClientDriver() : base(
			"Oracle.DataAccess",
			"Oracle.DataAccess.Client.OracleConnection",
			"Oracle.DataAccess.Client.OracleCommand" )
		{
		}

		/// <summary></summary>
		public override bool UseNamedPrefixInSql
		{
			get { return true; }
		}

		/// <summary></summary>
		public override bool UseNamedPrefixInParameter
		{
			get { return true; }
		}

		/// <summary></summary>
		public override string NamedPrefix
		{
			get { return ":"; }
		}

		/// <summary>
		/// Generates an IDbDataParameter for the IDbCommand.  It does not add the IDbDataParameter to the IDbCommand's
		/// Parameter collection.
		/// </summary>
		/// <param name="command">The IDbCommand to use to create the IDbDataParameter.</param>
		/// <param name="name">The name to set for IDbDataParameter.Name</param>
		/// <param name="parameter">The Parameter to convert to an IDbDataParameter.</param>
		/// <param name="dialect">The Dialect to use for Default lengths if needed.</param>
		/// <returns>An IDbDataParameter ready to be added to an IDbCommand.</returns>
		/// <remarks>
		/// This adds logic to ensure that a DbType.Boolean parameter is not created since
		/// ODP.NET doesn't support it.
		/// </remarks>
		protected override System.Data.IDbDataParameter GenerateParameter(IDbCommand command, string name, Parameter parameter, NHibernate.Dialect.Dialect dialect)
		{
			// if the parameter coming in contains a boolean then we need to convert it 
			// to another type since ODP.NET doesn't support DbType.Boolean
			if( parameter.SqlType.DbType==DbType.Boolean )
			{
				parameter = new Parameter( parameter.Name, NHibernateUtil.Int16.SqlType ) ;
			}
			return base.GenerateParameter( command, name, parameter, dialect );
		}

	}
}
	
