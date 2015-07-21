using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;


namespace NHibernate.Test.DialectTest.FunctionTests
{
	/// <summary>
	/// Verify that all dialects (found by reflection) that claim to support sequences
	/// really implement all related methods.
	/// </summary>
	[TestFixture]
	public class SequenceSupportFixture
	{
		/// <summary>
		/// Test case data source for DialectSupportingSequencesMustFullfillSequenceContract().
		/// </summary>
		private IEnumerable<System.Type> GetAllDialectTypes()
		{
			var dialectBaseType = typeof(NHibernate.Dialect.Dialect);

			return dialectBaseType.Assembly.GetExportedTypes()
				.Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(dialectBaseType))
				.ToList();
		}


		[TestCaseSource("GetAllDialectTypes")]
		public void DialectSupportingSequencesMustFullfillSequenceContract(System.Type dialectType)
		{
			var dialect = (NHibernate.Dialect.Dialect)Activator.CreateInstance(dialectType);

			if (!dialect.SupportsSequences)
				Assert.Ignore("This test applies only to dialects that support sequences.");

			// Just call every method that should work if SupportsSequences returns true, to
			// verify that we at least don't get any exceptions because someone forgot to
			// override something.

			dialect.GetCreateSequenceString("foo");

			if (dialect.SupportsPooledSequences)
				dialect.GetCreateSequenceStrings("foo", 3, 3);

			dialect.GetDropSequenceString("foo");
			dialect.GetDropSequenceStrings("foo");
			dialect.GetSelectSequenceNextValString("foo");
			dialect.GetSequenceNextValString("foo");
			var sql = dialect.QuerySequencesString;

		}
	}
}