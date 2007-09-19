using NUnit.Framework;

using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH1119
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH1119"; }
		}

		[Test]
		public void SelectMinFromEmptyTable()
		{
			using (ISession s = OpenSession())
			{
				try
				{
					DateTime dt = s.CreateQuery("select max(tc.DateTimeProperty) from TestClass tc").UniqueResult<DateTime>();
					string msg = "Calling UniqueResult<T> where T is a value type"
						+ " should throw InvalidCastException when the result"
						+ " is null";
					Assert.Fail(msg);
				}
				catch (InvalidCastException) { }
			}
		}
	}
}
