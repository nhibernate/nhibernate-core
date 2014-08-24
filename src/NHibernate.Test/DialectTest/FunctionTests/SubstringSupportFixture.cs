using System;
using System.Linq;
using System.Collections.Generic;
using NHibernate.Dialect;
using NHibernate.Dialect.Function;
using NUnit.Framework;


namespace NHibernate.Test.DialectTest.FunctionTests
{
	[TestFixture]
	public class SubstringSupportFixture
	{
		/// <summary>
		/// Test case data source for DialectShouldUseCorrectSubstringImplementation().
		/// </summary>
		private IEnumerable<System.Type> GetAllDialectTypes()
		{
			var dialectBaseType = typeof(NHibernate.Dialect.Dialect);

			return dialectBaseType.Assembly.GetExportedTypes()
				.Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(dialectBaseType))
				.ToList();
		}


		[TestCaseSource("GetAllDialectTypes")]
		public void DialectShouldUseCorrectSubstringImplementation(System.Type dialectType)
		{
			var dialect = (NHibernate.Dialect.Dialect)Activator.CreateInstance(dialectType);

			if (!dialect.Functions.ContainsKey("substring"))
				Assert.Ignore("Dialect does not support the substring function.");

			var substringFunction = dialect.Functions["substring"];

			if (dialect is MsSql2000Dialect || dialect is MsSqlCeDialect || dialect is SybaseASE15Dialect)
				Assert.That(substringFunction, Is.TypeOf<EmulatedLengthSubstringFunction>());
			else if (dialect is DB2Dialect)
				Assert.That(substringFunction, Is.TypeOf<SQLFunctionTemplate>());
			else if (dialect is SybaseSQLAnywhere10Dialect)
				Assert.That(substringFunction, Is.TypeOf<VarArgsSQLFunction>());
			else if (dialect is Oracle8iDialect)
				Assert.That(substringFunction, Is.TypeOf<StandardSQLFunction>());
			else if (dialect is SQLiteDialect)
				Assert.That(substringFunction, Is.TypeOf<StandardSQLFunction>());
			else
				Assert.That(substringFunction, Is.TypeOf<AnsiSubstringFunction>());

		}
	}
}