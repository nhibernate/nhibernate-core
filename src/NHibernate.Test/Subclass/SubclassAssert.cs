using System;

using NUnit.Framework;

namespace NHibernate.Test.Subclass
{
	/// <summary>
	/// Summary description for SubclassAssert.
	/// </summary>
	public sealed class SubclassAssert
	{
		private SubclassAssert() {}
		
		internal static void AreEqual(SubclassBase expected, SubclassBase actual) 
		{
			Assert.AreEqual( expected.Id, actual.Id );
			Assert.AreEqual( expected.TestDateTime, actual.TestDateTime );
			Assert.AreEqual( expected.TestLong, actual.TestLong );
			Assert.AreEqual( expected.TestString, actual.TestString );
		}

		internal static void AreEqual(SubclassOne expected, SubclassOne actual) 
		{
			AreEqual( (SubclassBase)expected, (SubclassBase)actual );
			Assert.AreEqual( expected.OneTestLong, actual.OneTestLong );
		}
	}
}
