using System;
using System.Text;

namespace NHibernate.Sql {
	
	public class Select {
		private string selectClause;
		private string fromClause;
		private string outerJoinsAfterFrom;
		private string whereClause;
		private string outerJoinsAfterWhere;
		private string orderByClause;

		/// <summary>
		/// Construct an SQL <c>SELECT</c> statement from the given clauses
		/// </summary>
		/// <returns>The SQL <c>SELECT</c> statement</returns>
		public string ToStatementString() {
			StringBuilder buf = new StringBuilder(
				selectClause.Length +
				fromClause.Length +
				outerJoinsAfterFrom.Length +
				whereClause.Length +
				outerJoinsAfterWhere.Length +
				20
				);
			buf.Append("SELECT ").Append(selectClause)
				.Append(" FROM ").Append(fromClause)
				.Append(outerJoinsAfterFrom)
				.Append(" WHERE ").Append(whereClause)
				.Append(outerJoinsAfterWhere);
			if (orderByClause != null && orderByClause.Trim().Length > 0)
				buf.Append(" ORDER BY ")
					.Append(orderByClause);
			return buf.ToString();
		}

		/// <summary>
		/// Sets the from clause
		/// </summary>
		/// <param name="fromClause">The fromClause to set</param>
		public Select SetFromClause(string fromClause) {
			this.fromClause = fromClause;
			return this;
		}

		public Select SetFromClause(string tableName, string alias) {
			this.fromClause = tableName + ' ' + alias;
			return this;
		}

		/// <summary>
		/// Sets the orderByClause
		/// </summary>
		/// <param name="orderByClause">The orderByClause to set</param>
		public Select SetOrderByClause(string orderByClause) {
			this.orderByClause = orderByClause;
			return this;
		}

		/// <summary>
		/// Sets the outer joins
		/// </summary>
		/// <param name="outerJoinsAfterFrom">The outerJoinsAfterFrom to set</param>
		/// <param name="outerJoinsAfterWhere">The outerJoinsAfterWhere to set</param>
		public Select SetOuterJoins(string outerJoinsAfterFrom, string outerJoinsAfterWhere) {
			this.outerJoinsAfterFrom = outerJoinsAfterFrom;
			this.outerJoinsAfterWhere = outerJoinsAfterWhere;
			return this;
		}

		/// <summary>
		/// Sets the selectClause
		/// </summary>
		/// <param name="selectClause">The selectClause to set</param>
		/// <returns></returns>
		public Select SetSelectClause(string selectClause) {
			this.selectClause = selectClause;
			return this;
		}

		/// <summary>
		/// Sets the whereClause
		/// </summary>
		/// <param name="selectClause">The whereClause to set</param>
		/// <returns></returns>
		public Select SetWhereClause(string whereClause) {
			this.whereClause = whereClause;
			return this;
		}
	}
}
