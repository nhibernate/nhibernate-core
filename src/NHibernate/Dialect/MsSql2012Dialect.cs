using NHibernate.Dialect.Function;

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
			return "drop sequence " + sequenceName;
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
	}
}
