using NHibernate.Dialect.Function;
using NHibernate.SqlCommand;
using NHibernate.SqlCommand.Parser;

namespace NHibernate.Dialect
{
	public class MsSql2012Dialect : MsSql2008Dialect
	{
		public override bool SupportsSequences
		{
			get { return true; }
		}

		public override bool SupportsPooledSequences
		{
			get { return true; }
		}

		public override string GetCreateSequenceString(string sequenceName)
		{
			// by default sequence is created as bigint start with long.MinValue
			return GetCreateSequenceString(sequenceName, 1, 1);
		}

		protected override string GetCreateSequenceString(string sequenceName, int initialValue, int incrementSize)
		{
			// by default sequence is created as bigint
			return string.Format("create sequence {0} as int start with {1} increment by {2}", sequenceName, initialValue, incrementSize);
		}

		public override string GetDropSequenceString(string sequenceName)
		{
			string dropSequence = "IF EXISTS (select * from sys.sequences where name = N'{0}') DROP SEQUENCE {0}";

			return string.Format(dropSequence, sequenceName);
		}

		public override string GetSequenceNextValString(string sequenceName)
		{
			return "select " + GetSelectSequenceNextValString(sequenceName) + " as seq";
		}

		public override string GetSelectSequenceNextValString(string sequenceName)
		{
			return "next value for " + sequenceName;
		}

		public override string QuerySequencesString
		{
			get { return "select name from sys.sequences"; }
		}

		protected override void RegisterFunctions()
		{
			base.RegisterFunctions();
			RegisterFunction("iif", new StandardSafeSQLFunction("iif", 3));
		}

		public override SqlString GetLimitString(SqlString querySqlString, SqlString offset, SqlString limit)
		{
			var tokenEnum = new SqlTokenizer(querySqlString).GetEnumerator();
			if (!tokenEnum.TryParseUntilFirstMsSqlSelectColumn()) return null;

			var result = new SqlStringBuilder(querySqlString);
			if (!tokenEnum.TryParseUntil("order"))
			{
				result.Add(" ORDER BY CURRENT_TIMESTAMP");
			}

			result.Add(" OFFSET ");
			if (offset != null)
			{
				result.Add(offset).Add(" ROWS");
			}
			else
			{
				result.Add("0 ROWS");
			}

			if (limit != null)
			{
				result.Add(" FETCH FIRST ").Add(limit).Add(" ROWS ONLY");
			}

			return result.ToSqlString();
		}
	}
}
