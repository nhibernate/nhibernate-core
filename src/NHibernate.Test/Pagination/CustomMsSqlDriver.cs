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
			bool hasLimit = new Regex(@"select\s+top").IsMatch(command.CommandText.ToLower());

			if (hasLimit && CustomMsSqlDialect.ForcedSupportsVariableLimit && CustomMsSqlDialect.ForcedBindLimitParameterFirst)
			{
				int offset = (int)((IDataParameter)command.Parameters[0]).Value;
				int limit = (int)((IDataParameter)command.Parameters[1]).Value;

				Assert.That(command.CommandText.ToLower().Contains("top " + limit),
					"Expected string containing 'top " + limit + "', but got " + command.CommandText);

				Assert.That(command.CommandText.ToLower().Contains("hibernate_sort_row > " + offset),
					"Expected string containing 'hibernate_sort_row > " + offset + "', but got " + command.CommandText);
			}

			base.OnBeforePrepare(command);
		}
	}
}