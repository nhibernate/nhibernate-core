using System;

using NHibernate.Dialect;
using NHibernate.Connection;

namespace NHibernate.SqlCommand 
{
	
	public interface ISqlStringBuilder 
	{
		/// <summary>
		/// Builds a SqlString from the internal data.
		/// </summary>
		/// <returns>A valid SqlString that can be converted into an IDbCommand</returns>
		SqlString ToSqlString();
	}
}
