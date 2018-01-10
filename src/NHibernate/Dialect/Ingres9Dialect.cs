using NHibernate.SqlCommand;

namespace NHibernate.Dialect
{
	public class Ingres9Dialect : IngresDialect
	{
		/// <summary>
		/// Does this Dialect have some kind of <c>LIMIT</c> syntax?
		/// </summary>
		/// <value>
		/// False, unless overridden.
		/// </value>
		public override bool SupportsLimit
		{
			get { return true; }
		}

		/// <summary>
		/// Can parameters be used for a statement containing a LIMIT?
		/// </summary>
		public override bool SupportsVariableLimit
		{
			get { return false; }
		}

		/// <summary>
		/// Does this Dialect support an offset?
		/// </summary>
		public override bool SupportsLimitOffset
		{
			get { return true; }
		}

		/// <inheritdoc />
		public override bool SupportsSequences => true;

		/// <inheritdoc />
		public override bool SupportsPooledSequences => true;

		/// <inheritdoc />
		public override string QuerySequencesString => "select seq_name from iisequences";

		/// <summary>
		/// Attempts to add a <c>LIMIT</c> clause to the given SQL <c>SELECT</c>.
		/// Expects any database-specific offset and limit adjustments to have already been performed (ex. UseMaxForLimit, OffsetStartsAtOne).
		/// </summary>
		/// <param name="queryString">The <see cref="SqlString"/> to base the limit query off.</param>
		/// <param name="offset">Offset of the first row to be returned by the query.  This may be represented as a parameter, a string literal, or a null value if no limit is requested.  This should have already been adjusted to account for OffsetStartsAtOne.</param>
		/// <param name="limit">Maximum number of rows to be returned by the query.  This may be represented as a parameter, a string literal, or a null value if no offset is requested.  This should have already been adjusted to account for UseMaxForLimit.</param>
		/// <returns>
		/// A new <see cref="SqlString"/> that contains the <c>LIMIT</c> clause. Returns <c>null</c>
		/// if <paramref name="queryString"/> represents a SQL statement to which a limit clause cannot be added,
		/// for example when the query string is custom SQL invoking a stored procedure.
		/// </returns>
		public override SqlString GetLimitString(SqlString queryString, SqlString offset, SqlString limit)
		{
			SqlStringBuilder pagingBuilder = new SqlStringBuilder();
			pagingBuilder.Add(queryString);

			if (offset != null)
			{
				pagingBuilder.Add(" offset ");
				pagingBuilder.Add(offset);
			}

			if (limit != null)
			{
				pagingBuilder.Add(" fetch ");
				pagingBuilder.Add(offset != null ? "next " : "first ");
				pagingBuilder.Add(limit);
				pagingBuilder.Add(" rows only");
			}

			return pagingBuilder.ToSqlString();
		}

		/// <inheritdoc />
		public override string GetSequenceNextValString(string sequenceName)
		{
			return "select " + GetSelectSequenceNextValString(sequenceName) + " as seq";
		}

		/// <inheritdoc />
		public override string GetSelectSequenceNextValString(string sequenceName)
		{
			return "next value for " + sequenceName;
		}

		/// <inheritdoc />
		public override string GetCreateSequenceString(string sequenceName)
		{
			return "create sequence " + sequenceName;
		}

		/// <inheritdoc />
		public override string GetDropSequenceString(string sequenceName)
		{
			return "drop sequence " + sequenceName;
		}

		#region Overridden informational metadata

		public override bool DoesRepeatableReadCauseReadersToBlockWriters => true;

		#endregion
	}
}
