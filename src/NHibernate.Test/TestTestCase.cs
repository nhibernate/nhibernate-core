using System;
using NUnit.Framework;

namespace NHibernate.Test
{
	/// <summary>
	/// Summary description for TestTestCase.
	/// </summary>
	[TestFixture]
	public class TestTestCase : TestCase
	{
		[Test]
		public void TestExecuteStatement()
		{
			base.ExecuteStatement("create table yyyy (x int)");
			base.ExecuteStatement("drop table yyyy");
		}
	}
}
