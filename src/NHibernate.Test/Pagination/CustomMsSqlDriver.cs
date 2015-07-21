using System.Data;
using System.Text.RegularExpressions;
using NHibernate.Driver;
using NUnit.Framework;

namespace NHibernate.Test.Pagination
{
	/// <summary>
	/// Class to work with CustomMsSqlDialect to allow
	/// verification of simulated limit parameters
	/// </summary>
	public class CustomMsSqlDriver : SqlClientDriver
	{
		public CustomMsSqlDialect CustomMsSqlDialect;

		protected override void OnBeforePrepare(IDbCommand command)
		{
			// We will probably remove all stuff regarding BindParameterFirst, last, in the middle, in inverse-order and so on, then this part of the test is unneeded.
			//bool hasLimit = new Regex(@"select\s+top").IsMatch(command.CommandText.ToLower());

			//if (hasLimit && CustomMsSqlDialect.ForcedSupportsVariableLimit && CustomMsSqlDialect.ForcedBindLimitParameterFirst)
			//{
			//  int offset = (int)((IDataParameter)command.Parameters[0]).Value;
			//  int limit = (int)((IDataParameter)command.Parameters[1]).Value;

			//  Assert.That(command.CommandText.ToLower().Contains("top (@p0)"),
			//    "Expected string containing 'top (@p0)', but got " + command.CommandText);

			//  Assert.That(command.CommandText.ToLower().Contains("hibernate_sort_row > @p1"),
			//    "Expected string containing 'hibernate_sort_row > @p1', but got " + command.CommandText);
			//}

			base.OnBeforePrepare(command);
		}
	}
}