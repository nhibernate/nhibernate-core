using System;

using NHibernate.Util;

using NUnit.Framework;

namespace NHibernate.Test.UtilityTest
{
	/// <summary>
	/// Summary description for StringHelperFixture.
	/// </summary>
	[TestFixture]
	public class StringHelperFixture
	{
		[Test]
		public void GetClassnameFromFQType() 
		{
			string typeName = "ns1.ns2.classname, as1.as2., pk, lang";
			string expected = "classname";

			Assert.AreEqual(expected, StringHelper.GetClassname(typeName));
		}

		[Test]
		public void GetClassnameFromFQClass() 
		{
			string typeName = "ns1.ns2.classname";
			string expected = "classname";

			Assert.AreEqual(expected, StringHelper.GetClassname(typeName));
		}

		[Test]
		public void CountUnquotedParams()
		{
			// Base case, no values
			Assert.AreEqual(0, StringHelper.CountUnquoted("abcd eftf", '?'));

			// Simple case 
			Assert.AreEqual(1, StringHelper.CountUnquoted("abcd ? eftf", '?'));

			// Multiple values
			Assert.AreEqual(2, StringHelper.CountUnquoted("abcd ? ef ? tf", '?'));

			// Quoted values
			Assert.AreEqual(1, StringHelper.CountUnquoted("abcd ? ef '?' tf", '?'));
		}

		[Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
		/// <summary>
		/// Try to locate single quotes which isn't allowed
		/// </summary>
		public void CantCountQuotes()
		{
			Assert.AreEqual(0, StringHelper.CountUnquoted("abcd eftf", StringHelper.SingleQuote));
		}

		[Test]
		/// <summary>
		/// Qualify a name with a prefix
		/// </summary>
		public void Qualify()
		{
			Assert.AreEqual( "a.b", StringHelper.Qualify( "a", "b" ), "Qualified names differ" );
		}

		[Test]
		/// <summary>
		/// Qualify an array of names with a prefix
		/// </summary>
		public void QualifyArray()
		{
			string[] simple = { "b", "c" };
			string[] qual = { "a.b", "a.c" };

			string[] result = StringHelper.Qualify( "a", simple );

			for (int i = 0; i < result.Length; i++ )
			{
				Assert.AreEqual( qual[i], result[i], "Qualified names differ" );
			}
		}
	}
}
