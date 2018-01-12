using System.Data.Common;
using NHibernate.SqlCommand;

namespace NHibernate.Driver
{
	public partial interface IResultSetsCommand
	{
		void Append(ISqlCommand command);
		bool HasQueries { get; }
		SqlString Sql { get; }
		/// <summary>
		/// Get a data reader for this multiple result sets command.
		/// </summary>
		/// <param name="commandTimeout">The timeout in seconds for the underlying ADO.NET query.</param>
		/// <returns>A data reader.</returns>
		DbDataReader GetReader(int? commandTimeout);
	}
}
