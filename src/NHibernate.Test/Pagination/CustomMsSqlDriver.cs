using System.Data.Common;
using NHibernate.Driver;

namespace NHibernate.Test.Pagination
{
	/// <summary>
	/// Class to work with CustomMsSqlDialect to allow
	/// verification of simulated limit parameters
	/// </summary>
	public class CustomMsSqlDriver : SqlServer2000Driver
	{
		public CustomMsSqlDialect CustomMsSqlDialect;

		protected override void OnBeforePrepare(DbCommand command)
		{
			// We will probably remove all stuff regarding BindParameterFirst, last, in the middle, in inverse-order and so on, then this part of the test is unneeded.
			//bool hasLimit = new Regex(@"select\s+top").IsMatch(command.CommandText.ToLower());

			//if (hasLimit && CustomMsSqlDialect.ForcedSupportsVariableLimit && CustomMsSqlDialect.ForcedBindLimitParameterFirst)
			//{
			//  int offset = (int)((DbParameter)command.Parameters[0]).Value;
			//  int limit = (int)((DbParameter)command.Parameters[1]).Value;

			//  Assert.That(command.CommandText.ToLower().Contains("top (@p0)"),
			//    "Expected string containing 'top (@p0)', but got " + command.CommandText);

			//  Assert.That(command.CommandText.ToLower().Contains("hibernate_sort_row > @p1"),
			//    "Expected string containing 'hibernate_sort_row > @p1', but got " + command.CommandText);
			//}

			base.OnBeforePrepare(command);
		}
	}
}
