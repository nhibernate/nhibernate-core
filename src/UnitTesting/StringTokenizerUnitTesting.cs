using System;
using System.Collections;

using NUnit.Framework;

using NHibernate.Util;

namespace NHibernate.UnitTesting
{
	[TestFixture]
	public class StringTokenizerUnitTesting
	{
		[Test]
		public void SimpleStringWithoutDelimiters()
		{
			string s = "Hello world!";
			StringTokenizer st = new StringTokenizer(s);
			IEnumerator enumerator = st.GetEnumerator();
			enumerator.MoveNext();
			Assertion.AssertEquals("Can't get first token", "Hello", (string) enumerator.Current);
			enumerator.MoveNext();
			Assertion.AssertEquals("Can't get second token", "world!", (string) enumerator.Current);
			Assertion.AssertEquals("Still thinking there are more tokens", false, enumerator.MoveNext());
		}

		[Test]
		public void SimpleStringWithDelimiters()
		{
			string s = "Hello world!";
			StringTokenizer st = new StringTokenizer(s," ",true);
			IEnumerator enumerator = st.GetEnumerator();
			enumerator.MoveNext();
			Assertion.AssertEquals("Can't get first token", "Hello", (string) enumerator.Current);
			enumerator.MoveNext();
			Assertion.AssertEquals("Can't get first space", " ", (string) enumerator.Current);
			enumerator.MoveNext();
			Assertion.AssertEquals("Can't get second token", "world!", (string) enumerator.Current);
			Assertion.AssertEquals("Still thinking there are more tokens", false, enumerator.MoveNext());
		}

		[Test]
		public void NotSoSimpleWithoutDelimiters()
		{
			string s = "The lazy... I don't know ";
			StringTokenizer st = new StringTokenizer(s," .",false);
			IEnumerator enumerator = st.GetEnumerator();
			enumerator.MoveNext();
			Assertion.AssertEquals("Can't get first token", "The", (string) enumerator.Current);
			enumerator.MoveNext();
			Assertion.AssertEquals("Can't get second token", "lazy", (string) enumerator.Current);
			enumerator.MoveNext();
			Assertion.AssertEquals("Can't get third token", "I", (string) enumerator.Current);
			enumerator.MoveNext();
			Assertion.AssertEquals("Can't get fourth token", "don't", (string) enumerator.Current);
			enumerator.MoveNext();
			Assertion.AssertEquals("Can't get fifth token", "know", (string) enumerator.Current);
			Assertion.AssertEquals("Still thinking there are more tokens", false, enumerator.MoveNext());
		}

		[Test]
		public void NotSoSimpleWithDelimiters()
		{
			string s = "The lazy... I don't know ";
			StringTokenizer st = new StringTokenizer(s," .",true);
			IEnumerator enumerator = st.GetEnumerator();
			enumerator.MoveNext();
			Assertion.AssertEquals("Can't get first token", "The", (string) enumerator.Current);
			enumerator.MoveNext();
			Assertion.AssertEquals("Can't get first space", " ", (string) enumerator.Current);
			enumerator.MoveNext();
			Assertion.AssertEquals("Can't get second token", "lazy", (string) enumerator.Current);
			enumerator.MoveNext();
			Assertion.AssertEquals("Can't get first dot", ".", (string) enumerator.Current);
			enumerator.MoveNext();
			Assertion.AssertEquals("Can't get second dot", ".", (string) enumerator.Current);
			enumerator.MoveNext();
			Assertion.AssertEquals("Can't get third dot", ".", (string) enumerator.Current);
			enumerator.MoveNext();
			Assertion.AssertEquals("Can't get second space", " ", (string) enumerator.Current);
			enumerator.MoveNext();
			Assertion.AssertEquals("Can't get third token", "I", (string) enumerator.Current);
			enumerator.MoveNext();
			Assertion.AssertEquals("Can't get third space", " ", (string) enumerator.Current);
			enumerator.MoveNext();
			Assertion.AssertEquals("Can't get fourth token", "don't", (string) enumerator.Current);
			enumerator.MoveNext();
			Assertion.AssertEquals("Can't get fourth space", " ", (string) enumerator.Current);
			enumerator.MoveNext();
			Assertion.AssertEquals("Can't get fifth token", "know", (string) enumerator.Current);
			enumerator.MoveNext();
			Assertion.AssertEquals("Can't get last space", " ", (string) enumerator.Current);
			Assertion.AssertEquals("Still thinking there are more tokens", false, enumerator.MoveNext());
		}

		[Test]
		public void OnlyDelimitersWithoutDelimiters()
		{
			string s = " .,,,";
			StringTokenizer st = new StringTokenizer(s, " ,.", false);
			IEnumerator enumerator = st.GetEnumerator();
			Assertion.AssertEquals("Still thinking there are more tokens", false, enumerator.MoveNext());
		}

		[Test]
		public void OnlyDelimitersWithDelimiters()
		{
			string s = " .,,,";
			StringTokenizer st = new StringTokenizer(s, " ,.", true);
			IEnumerator enumerator = st.GetEnumerator();
			enumerator.MoveNext();
			Assertion.AssertEquals("Can't get first delimiter", " ", (string) enumerator.Current);
			enumerator.MoveNext();
			Assertion.AssertEquals("Can't get second delimiter", ".", (string) enumerator.Current);
			enumerator.MoveNext();
			Assertion.AssertEquals("Can't get third delimiter", ",", (string) enumerator.Current);
			enumerator.MoveNext();
			Assertion.AssertEquals("Can't get fourth delimiter", ",", (string) enumerator.Current);
			enumerator.MoveNext();
			Assertion.AssertEquals("Can't get fifth delimiter", ",", (string) enumerator.Current);
			Assertion.AssertEquals("Still thinking there are more tokens", false, enumerator.MoveNext());
		}

	}
}
