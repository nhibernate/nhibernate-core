using System;

using NUnit.Framework;
using NHibernate.DomainModel;

namespace NHibernate.Test
{
	/// <summary>
	/// Helps with Assertions with two Objects.
	/// </summary>
	public class ObjectAssertion
	{
		public ObjectAssertion()
		{
		}

		internal static void AssertPropertiesEqual(JoinedSubclassBase expected, JoinedSubclassBase actual) 
		{
			Assertion.AssertEquals(expected.Id, actual.Id);
			Assertion.AssertEquals(expected.TestDateTime, actual.TestDateTime);
			Assertion.AssertEquals(expected.TestLong, actual.TestLong);
			Assertion.AssertEquals(expected.TestString, actual.TestString);
		}

		internal static void AssertPropertiesEqual(JoinedSubclassOne expected, JoinedSubclassOne actual) 
		{
			ObjectAssertion.AssertPropertiesEqual((JoinedSubclassBase)expected, (JoinedSubclassBase)actual);
			Assertion.AssertEquals(expected.OneTestLong, actual.OneTestLong);
		}
	}
}
