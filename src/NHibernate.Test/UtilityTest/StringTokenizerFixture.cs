using System;
using System.Collections;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.UnitTesting
{
	[TestFixture]
	public class StringTokenizerFixture
	{
		[Test]
		public void SimpleStringWithoutDelimiters()
		{
			string s = "Hello world!";
			StringTokenizer st = new StringTokenizer(s);
			IEnumerator enumerator = st.GetEnumerator();
			enumerator.MoveNext();
			Assert.AreEqual("Hello", (string) enumerator.Current, "Can't get first token");
			enumerator.MoveNext();
			Assert.AreEqual("world!", (string) enumerator.Current, "Can't get second token");
			Assert.AreEqual(false, enumerator.MoveNext(), "Still thinking there are more tokens");
		}

		[Test]
		public void SimpleStringWithDelimiters()
		{
			string s = "Hello world!";
			StringTokenizer st = new StringTokenizer(s, " ", true);
			IEnumerator enumerator = st.GetEnumerator();
			enumerator.MoveNext();
			Assert.AreEqual("Hello", (string) enumerator.Current, "Can't get first token");
			enumerator.MoveNext();
			Assert.AreEqual(" ", (string) enumerator.Current, "Can't get first space");
			enumerator.MoveNext();
			Assert.AreEqual("world!", (string) enumerator.Current, "Can't get second token");
			Assert.AreEqual(false, enumerator.MoveNext(), "Still thinking there are more tokens");
		}

		[Test]
		public void NotSoSimpleWithoutDelimiters()
		{
			string s = "The lazy... I don't know ";
			StringTokenizer st = new StringTokenizer(s, " .", false);
			IEnumerator enumerator = st.GetEnumerator();
			enumerator.MoveNext();
			Assert.AreEqual("The", (string) enumerator.Current, "Can't get first token");
			enumerator.MoveNext();
			Assert.AreEqual("lazy", (string) enumerator.Current, "Can't get second token");
			enumerator.MoveNext();
			Assert.AreEqual("I", (string) enumerator.Current, "Can't get third token");
			enumerator.MoveNext();
			Assert.AreEqual("don't", (string) enumerator.Current, "Can't get fourth token");
			enumerator.MoveNext();
			Assert.AreEqual("know", (string) enumerator.Current, "Can't get fifth token");
			Assert.AreEqual(false, enumerator.MoveNext(), "Still thinking there are more tokens");
		}

		[Test]
		public void NotSoSimpleWithDelimiters()
		{
			string s = "The lazy... I don't know ";
			StringTokenizer st = new StringTokenizer(s, " .", true);
			IEnumerator enumerator = st.GetEnumerator();
			enumerator.MoveNext();
			Assert.AreEqual("The", (string) enumerator.Current, "Can't get first token");
			enumerator.MoveNext();
			Assert.AreEqual(" ", (string) enumerator.Current, "Can't get first space");
			enumerator.MoveNext();
			Assert.AreEqual("lazy", (string) enumerator.Current, "Can't get second token");
			enumerator.MoveNext();
			Assert.AreEqual(".", (string) enumerator.Current, "Can't get first dot");
			enumerator.MoveNext();
			Assert.AreEqual(".", (string) enumerator.Current, "Can't get second dot");
			enumerator.MoveNext();
			Assert.AreEqual(".", (string) enumerator.Current, "Can't get third dot");
			enumerator.MoveNext();
			Assert.AreEqual(" ", (string) enumerator.Current, "Can't get second space");
			enumerator.MoveNext();
			Assert.AreEqual("I", (string) enumerator.Current, "Can't get third token");
			enumerator.MoveNext();
			Assert.AreEqual(" ", (string) enumerator.Current, "Can't get third space");
			enumerator.MoveNext();
			Assert.AreEqual("don't", (string) enumerator.Current, "Can't get fourth token");
			enumerator.MoveNext();
			Assert.AreEqual(" ", (string) enumerator.Current, "Can't get fourth space");
			enumerator.MoveNext();
			Assert.AreEqual("know", (string) enumerator.Current, "Can't get fifth token");
			enumerator.MoveNext();
			Assert.AreEqual(" ", (string) enumerator.Current, "Can't get last space");
			Assert.AreEqual(false, enumerator.MoveNext(), "Still thinking there are more tokens");
		}

		[Test]
		public void OnlyDelimitersWithoutDelimiters()
		{
			string s = " .,,,";
			StringTokenizer st = new StringTokenizer(s, " ,.", false);
			IEnumerator enumerator = st.GetEnumerator();
			Assert.AreEqual(false, enumerator.MoveNext(), "Still thinking there are more tokens");
		}

		[Test]
		public void OnlyDelimitersWithDelimiters()
		{
			string s = " .,,,";
			StringTokenizer st = new StringTokenizer(s, " ,.", true);
			IEnumerator enumerator = st.GetEnumerator();
			enumerator.MoveNext();
			Assert.AreEqual(" ", (string) enumerator.Current, "Can't get first delimiter");
			enumerator.MoveNext();
			Assert.AreEqual(".", (string) enumerator.Current, "Can't get second delimiter");
			enumerator.MoveNext();
			Assert.AreEqual(",", (string) enumerator.Current, "Can't get third delimiter");
			enumerator.MoveNext();
			Assert.AreEqual(",", (string) enumerator.Current, "Can't get fourth delimiter");
			enumerator.MoveNext();
			Assert.AreEqual(",", (string) enumerator.Current, "Can't get fifth delimiter");
			Assert.AreEqual(false, enumerator.MoveNext(), "Still thinking there are more tokens");
		}
	}
}