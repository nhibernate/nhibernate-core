using NHibernate.Dialect;

namespace NHibernate.Test.Pagination
{
	/// <summary>
	/// Class to simulate dialects with different binding of limit parameters
	/// using an MSSql database
	/// </summary>
	public class CustomMsSqlDialect : MsSql2005Dialect
	{
		public bool ForcedBindLimitParameterFirst;
		public bool ForcedSupportsVariableLimit;

		public override bool SupportsVariableLimit { get { return ForcedSupportsVariableLimit; } }
	}
}