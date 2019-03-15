using System;
using System.Collections;
using NHibernate.Exceptions;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2166
{
	[TestFixture]
	public class Fixture: TestCase
	{
		protected override string[] Mappings
		{
			get { return Array.Empty<string>(); }
		}

		[Test]
		public void WhenUniqueResultShouldCallConverter()
		{
			using (var s = OpenSession())
			{
				Assert.That(() => s.CreateSQLQuery("select make from ItFunky").UniqueResult<int>(), Throws.TypeOf<GenericADOException>());
			}
		}
	}
}
