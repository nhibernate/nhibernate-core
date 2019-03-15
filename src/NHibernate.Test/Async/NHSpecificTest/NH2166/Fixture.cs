﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Collections;
using NHibernate.Exceptions;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2166
{
	using System.Threading.Tasks;
	[TestFixture]
	public class FixtureAsync: TestCase
	{
		protected override string[] Mappings
		{
			get { return Array.Empty<string>(); }
		}

		[Test]
		public void WhenUniqueResultShouldCallConverterAsync()
		{
			using (var s = OpenSession())
			{
				Assert.That(() => s.CreateSQLQuery("select make from ItFunky").UniqueResultAsync<int>(), Throws.TypeOf<GenericADOException>());
			}
		}
	}
}
