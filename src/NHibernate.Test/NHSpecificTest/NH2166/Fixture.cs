using System;
using System.Collections;
using NHibernate.Exceptions;
using NUnit.Framework;
using SharpTestsEx;

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
				Executing.This(()=> s.CreateSQLQuery("select make from ItFunky").UniqueResult<int>()).Should().Throw<GenericADOException>();
			}
		}
	}
}