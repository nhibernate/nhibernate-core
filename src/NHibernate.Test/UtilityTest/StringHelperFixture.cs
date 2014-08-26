using System;
using System.Collections.Generic;
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
			const string typeName = "ns1.ns2.classname, as1.as2., pk, lang";
			const string expected = "classname";

			Assert.AreEqual(expected, StringHelper.GetClassname(typeName));
		}

		[Test]
		public void GetClassnameFromFQClass()
		{
			const string typeName = "ns1.ns2.classname";
			const string expected = "classname";

			Assert.AreEqual(expected, StringHelper.GetClassname(typeName));
		}

		[Test]
		public void GetFullClassNameForGenericType()
		{
			string typeName = typeof (IDictionary<int, string>).AssemblyQualifiedName;
			string expected = typeof (IDictionary<int, string>).FullName;
			Assert.AreEqual(expected, StringHelper.GetFullClassname(typeName));
			typeName = "some.namespace.SomeType`1[[System.Int32, mscorlib], System.Int32], some.assembly";
			expected = "some.namespace.SomeType`1[[System.Int32, mscorlib],[System.Int32]]";
			Assert.AreEqual(expected, StringHelper.GetFullClassname(typeName));
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

		[Test]
		/// <summary>
		/// Try to locate single quotes which isn't allowed
		/// </summary>
		public void CantCountQuotes()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => StringHelper.CountUnquoted("abcd eftf", StringHelper.SingleQuote));
		}

		[Test]
		/// <summary>
		/// Qualify a name with a prefix
		/// </summary>
		public void Qualify()
		{
			Assert.AreEqual("a.b", StringHelper.Qualify("a", "b"), "Qualified names differ");
		}

		[Test]
		/// <summary>
		/// Qualify an array of names with a prefix
		/// </summary>
		public void QualifyArray()
		{
			string[] simple = { "b", "c" };
			string[] qual = { "a.b", "a.c" };

			string[] result = StringHelper.Qualify("a", simple);

			for (int i = 0; i < result.Length; i++)
			{
				Assert.AreEqual(qual[i], result[i], "Qualified names differ");
			}
		}

		[Test]
		public void GenerateAliasForGenericTypeName()
		{
			const string typeName = "A`1[B]";
			string alias = StringHelper.GenerateAlias(typeName, 10);

			Assert.IsFalse(alias.Contains("`"), "alias '{0}' should not contain backticks", alias);
			Assert.IsFalse(alias.Contains("["), "alias '{0}' should not contain left bracket", alias);
			Assert.IsFalse(alias.Contains("]"), "alias '{0}' should not contain right bracket", alias);
		}

		[Test]
		public void GetClassnameFromGenericType()
		{
			const string typeName = "classname`1[innerns1.innerClass]";
			const string expected = "classname`1[[innerns1.innerClass]]";

			Assert.AreEqual(expected, StringHelper.GetClassname(typeName));
		}

		[Test]
		public void GetClassnameFromGenericFQClass()
		{
			const string typeName = "ns1.ns2.classname`1[innerns1.innerClass]";
			const string expected = "classname`1[[innerns1.innerClass]]";

			Assert.AreEqual(expected, StringHelper.GetClassname(typeName));
		}

		[Test]
		public void IsBackticksEnclosed()
		{
			Assert.That(!StringHelper.IsBackticksEnclosed(null));
			Assert.That(!StringHelper.IsBackticksEnclosed("`something"));
			Assert.That(!StringHelper.IsBackticksEnclosed("something`"));
			Assert.That(StringHelper.IsBackticksEnclosed("`something`"));
		}

		[Test]
		public void PurgeBackticksEnclosing()
		{
			Assert.That(StringHelper.PurgeBackticksEnclosing(null), Is.Null);
			Assert.That(StringHelper.PurgeBackticksEnclosing("`something"), Is.EqualTo("`something"));
			Assert.That(StringHelper.PurgeBackticksEnclosing("something`"), Is.EqualTo("something`"));
			Assert.That(StringHelper.PurgeBackticksEnclosing("`something`"), Is.EqualTo("something"));
		}


		[TestCase("ab", 0, -1, 0)]
		[TestCase("a\r\nb", 0, 1, 2)]
		[TestCase("a\nb", 0, 1, 1)]
		[TestCase("ab\r\nfoo\r\n", 4, 7, 2)]
		public void IndexOfAnyNewLineReturnsIndexAndLength(string str, int startIndex, int expectedIndex,
		                                                   int expectedMatchLength)
		{
			int matchLength;
			var matchIndex = str.IndexOfAnyNewLine(startIndex, out matchLength);

			Assert.That(matchIndex, Is.EqualTo(expectedIndex));
			Assert.That(matchLength, Is.EqualTo(expectedMatchLength));
		}


		[TestCase("ab", 0, false, 0)]
		[TestCase("a\r\nb", 0, false, 0)]
		[TestCase("a\nb", 1, true, 1)]
		[TestCase("a\r\nb", 1, true, 2)]
		public void IsAnyNewLineMatchAndLength(string str, int startIndex, bool expectNewLine,
		                                       int expectedMatchLength)
		{
			int matchLength;
			var isNewLine = str.IsAnyNewLine(startIndex, out matchLength);

			Assert.That(isNewLine, Is.EqualTo(expectNewLine));
			Assert.That(matchLength, Is.EqualTo(expectedMatchLength));
		}
	}
}
