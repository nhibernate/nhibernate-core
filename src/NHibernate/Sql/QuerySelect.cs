using System;
using System.Text;
using System.Collections;

using NHibernate.Dialect;
using NHibernate.Util;

namespace NHibernate.Sql {
	/// <summary>
	/// Summary description for QuerySelect.
	/// </summary>
	public class QuerySelect {
		
		private JoinFragment joins;
		private StringBuilder select = new StringBuilder();
		private StringBuilder where = new StringBuilder();
		private StringBuilder groupBy = new StringBuilder();
		private StringBuilder orderBy = new StringBuilder();
		private StringBuilder having = new StringBuilder();
		private bool distinct=false;

		private static readonly IList dontSpace = new ArrayList();

		static QuerySelect() {

			//dontSpace.add("'");
			dontSpace.Add(".");
			dontSpace.Add("+");
			dontSpace.Add("-");
			dontSpace.Add("/");
			dontSpace.Add("*");
			dontSpace.Add("<");
			dontSpace.Add(">");
			dontSpace.Add("=");
			dontSpace.Add("#");
			dontSpace.Add("~");
			dontSpace.Add("|");
			dontSpace.Add("&");
			dontSpace.Add("<=");
			dontSpace.Add(">=");
			dontSpace.Add("=>");
			dontSpace.Add("=<");
			dontSpace.Add("!=");
			dontSpace.Add("<>");
			dontSpace.Add("!#");
			dontSpace.Add("!~");
			dontSpace.Add("!<");
			dontSpace.Add("!>");
			dontSpace.Add(StringHelper.OpenParen); //for MySQL
			dontSpace.Add(StringHelper.ClosedParen);
		}

		public QuerySelect(Dialect.Dialect dialect) {
			joins = new QueryJoinFragment(dialect);
		}

		public JoinFragment JoinFragment {
			get { return joins; }
		}
	
		public void AddSelectFragmentString(string fragment) {
			if ( fragment.Length>0 && fragment[0]==',' ) fragment = fragment.Substring(1);
			fragment = fragment.Trim();
			if ( fragment.Length>0 ) {
				if ( select.Length>0 ) select.Append(StringHelper.CommaSpace);
				select.Append(fragment);
			}
		}
	
		public void AddSelectColumn(string columnName, string alias) {
			AddSelectFragmentString(columnName + ' ' + alias);
		}
	
		public bool Distinct {
			set { this.distinct = value; }
		}
	
		public IEnumerator WhereTokens {
			//if ( conjunctiveWhere.length()>0 ) conjunctiveWhere.append(" and ");
			set { AppendTokens(where, value); }
		}
		
		public IEnumerator GroupByTokens {
			//if ( groupBy.length()>0 ) groupBy.append(" and ");
			set { AppendTokens(groupBy, value); }
		}
		
		public IEnumerator OrderByTokens {
			//if ( orderBy.length()>0 ) orderBy.append(" and ");
			set { AppendTokens(orderBy, value); }
		}
		
		public IEnumerator HavingTokens {
			//if ( having.length()>0 ) having.append(" and ");
			set { AppendTokens(having, value); }
		}

		public String ToQueryString() {
			StringBuilder buf = new StringBuilder(50)
				.Append("select ");
			if (distinct) buf.Append("distinct ");
			buf.Append(select)
				.Append(" from")
				.Append( joins.ToFromFragmentString.Substring(1) );
			String part1 = joins.ToWhereFragmentString.Trim();
			String part2 = where.ToString().Trim();
			bool hasPart1 = part1.Length > 0;
			bool hasPart2 = part2.Length > 0;
			if (hasPart1 || hasPart2) buf.Append(" where ");
			if (hasPart1) buf.Append( part1.Substring(4) );
			if (hasPart2) {
				if (hasPart1) buf.Append(" and (");
				buf.Append(part2);
				if (hasPart1) buf.Append(")");
			}
			if ( groupBy.Length > 0 ) buf.Append(" group by ").Append(groupBy);
			if ( having.Length > 0 ) buf.Append(" having ").Append(having);
			if ( orderBy.Length > 0 ) buf.Append(" order by ").Append(orderBy);
			return buf.ToString();
		}

		private void AppendTokens(StringBuilder buf, IEnumerator iter) {
			bool lastSpaceable=true;
			while ( iter.MoveNext() ) {
				string token = (string) iter.Current;
				bool spaceable = !dontSpace.Contains(token);
				if (spaceable && lastSpaceable) buf.Append(' ');
				lastSpaceable = spaceable;
				buf.Append(token);
			}
		}
	}
}