using System;
using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test
{
	/// <summary>
	/// Summary description for TestTestCase.
	/// </summary>
	[TestFixture]
	public class TestTestCase : TestCase
	{
		protected override IList Mappings
		{
			get { return new string[0]; }
		}


		[Test]
		public void TestExecuteStatement()
		{
			base.ExecuteStatement("create table yyyy (x int)");
			base.ExecuteStatement("drop table yyyy");
		}
	}
}