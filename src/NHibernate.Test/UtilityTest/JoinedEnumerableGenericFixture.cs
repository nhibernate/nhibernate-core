using System.Collections;
using System.Collections.Generic;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.UtilityTest
{
	// This test is the same of JoinedEnumerableFixture but for generic version.
	[TestFixture]
	public class JoinedEnumerableGenericFixture
	{
		[Test]
		public void WrapsSingle()
		{
			int[] expected = new int[] {1, 2, 3};

			EnumerableTester<int> first;
			JoinedEnumerable<int> joined = InitSingle(out first);

			int index = 0;

			foreach (int actual in joined)
			{
				Assert.AreEqual(expected[index], actual, "Failure at " + index);
				index++;
			}

			Assert.AreEqual(expected.Length, index, "Every expected value was found");
			Assert.IsTrue(first.WasDisposed, "should have been disposed of.");
		}

		[Test]
		public void WrapsSingleBreak()
		{
			int[] expected = new int[] {1, 2, 3};

			EnumerableTester<int> first;
			JoinedEnumerable<int> joined = InitSingle(out first);

			foreach (int actual in joined)
			{
				Assert.AreEqual(expected[0], actual, "first one was not what was expected.");
				break;
			}

			Assert.IsTrue(first.WasDisposed, "should have been disposed of.");
		}

		[Test]
		public void WrapsMultiple()
		{
			int[] expected = new int[] {1, 2, 3, 4, 5, 6};

			EnumerableTester<int> first;
			EnumerableTester<int> second;
			JoinedEnumerable<int> joined = InitMultiple(out first, out second);

			int index = 0;

			foreach (int actual in joined)
			{
				Assert.AreEqual(expected[index], actual, "Failure at " + index);
				index++;
			}

			Assert.AreEqual(expected.Length, index, "Every expected value was found");
			Assert.IsTrue(first.WasDisposed, "first should have been disposed of.");
			Assert.IsTrue(second.WasDisposed, "second should have been disposed of. ");
		}

		[Test]
		public void WrapsMultipleBreak()
		{
			// break in the first IEnumerator
			WrapsMultipleBreak(2);

			// ensure behavior is consistent if break in 2nd IEnumerator
			WrapsMultipleBreak(4);
		}

		private static void WrapsMultipleBreak(int breakIndex)
		{
			int[] expected = new int[] {1, 2, 3, 4, 5, 6};

			EnumerableTester<int> first;
			EnumerableTester<int> second;
			JoinedEnumerable<int> joined = InitMultiple(out first, out second);

			int index = 0;

			foreach (int actual in joined)
			{
				Assert.AreEqual(expected[index], actual, "Failure at " + index);
				index++;
				if (index == breakIndex)
				{
					break;
				}
			}

			Assert.IsTrue(first.WasDisposed, "first should have been disposed of.");
			Assert.IsTrue(second.WasDisposed, "second should have been disposed of. ");
		}


		private static JoinedEnumerable<int> InitSingle(out EnumerableTester<int> first)
		{
			first = new EnumerableTester<int>(new List<int>(new int[] { 1, 2, 3 }));
			return new JoinedEnumerable<int>(new IEnumerable<int>[] { first });
		}

		private static JoinedEnumerable<int> InitMultiple(out EnumerableTester<int> first, out EnumerableTester<int> second)
		{
			first = new EnumerableTester<int>(new List<int>(new int[] { 1, 2, 3 }));
			second = new EnumerableTester<int>(new List<int>(new int[] { 4, 5, 6 }));
			return new JoinedEnumerable<int>(new IEnumerable<int>[] { first, second });
		}
	}

	internal class EnumerableTester<T> : IEnumerable<T>
	{
		private readonly IList<T> list;
		private readonly EnumerableTesterEnumerator<T> enumerator;

		public EnumerableTester(IList<T> wrappedList)
		{
			list = wrappedList;
			enumerator = new EnumerableTesterEnumerator<T>(list.GetEnumerator());
		}

		public bool WasDisposed
		{
			get { return enumerator.IsDisposed; }
		}

		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return ((IEnumerable<T>)this).GetEnumerator();
		}

		#endregion

		#region IEnumerable<T2> Members

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return enumerator;
		}

		#endregion

		private class EnumerableTesterEnumerator<T2> : IEnumerator<T2>
		{
			private readonly IEnumerator<T2> enumerator;
			private bool disposed;

			public EnumerableTesterEnumerator(IEnumerator<T2> enumerator)
			{
				this.enumerator = enumerator;
			}

			public bool IsDisposed
			{
				get { return disposed; }
			}

			#region IEnumerator Members

			public void Reset()
			{
				disposed = false;
				enumerator.Reset();
			}

			public object Current
			{
				get { return ((IEnumerator<T2>)this).Current; }
			}

			#region IEnumerator<T2> Members

			T2 IEnumerator<T2>.Current
			{
				get { return enumerator.Current; }
			}

			#endregion

			public bool MoveNext()
			{
				return enumerator.MoveNext();
			}

			#endregion

			#region IDisposable Members

			public void Dispose()
			{
				disposed = true;
			}

			#endregion
		}
	}
}
