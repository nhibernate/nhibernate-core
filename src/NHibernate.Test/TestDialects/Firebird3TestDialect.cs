namespace NHibernate.Test.TestDialects
{
	public class Firebird3TestDialect : FirebirdTestDialect
	{
		public Firebird3TestDialect(Dialect.Dialect dialect) : base(dialect)
		{
		}

		/// <summary>
		/// "start with" sets the current value. Firebird 4 fixes this.
		/// </summary>
		public override bool SequenceStartsAtInitialValue => false;
	}
}
