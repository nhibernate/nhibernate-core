using System;
using System.Collections;
using NHibernate.Exceptions;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2166
{
	public class Fixture: TestCase
	{
		protected override IList Mappings
		{
			get { return new string[0]; }
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