using System;
using System.Collections.Generic;
using System.Linq;
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
		private static IEnumerable<System.Type> GetAllDialectTypes()
		{
			var dialectBaseType = typeof(NHibernate.Dialect.Dialect);

			return dialectBaseType.Assembly.GetExportedTypes()
				.Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(dialectBaseType))
				.ToList();
		}

		[TestCaseSource(nameof(GetAllDialectTypes))]
		public void DialectShouldUseCorrectSubstringImplementation(System.Type dialectType)
		{
			var dialect = (NHibernate.Dialect.Dialect) Activator.CreateInstance(dialectType);

			if (!dialect.Functions.ContainsKey("substring"))
				Assert.Ignore("Dialect does not support the substring function.");

			var substringFunction = dialect.Functions["substring"];

			switch (dialect)
			{
				case MsSql2000Dialect _:
				case MsSqlCeDialect _:
				case SybaseASE15Dialect _:
					Assert.That(substringFunction, Is.TypeOf<EmulatedLengthSubstringFunction>());
					break;
				case SybaseSQLAnywhere10Dialect _:
					Assert.That(substringFunction, Is.TypeOf<VarArgsSQLFunction>());
					break;
				case DB2Dialect _:
				case Oracle8iDialect _:
				case SQLiteDialect _:
				case HanaDialectBase _:
					Assert.That(substringFunction, Is.TypeOf<StandardSQLFunction>());
					break;
				default:
					Assert.That(substringFunction, Is.TypeOf<AnsiSubstringFunction>());
					break;
			}
		}
	}
}
