using System;
using System.Collections;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.UtilityTest
{
	/// <summary>
	/// Test cases for the <see cref="JoinedEnumerable"/> class.
	/// </summary>
	[TestFixture]
	public class JoinedEnumerableFixture
	{
		/// <summary>
		/// Test that the JoinedEnumerable works with a single wrapped
		/// IEnumerable as expected when fully enumerating the collection.
		/// </summary>
		[Test]
		public void WrapsSingle()
		{
			int[] expected = new int[] {1, 2, 3};

			EnumerableTester first;
			JoinedEnumerable joined = InitSingle(out first);

			int index = 0;

			foreach (int actual in joined)
			{
				Assert.AreEqual(expected[index], actual, "Failure at " + index.ToString());
				index++;
			}

			Assert.AreEqual(expected.Length, index, "Every expected value was found");
			Assert.IsTrue(first.WasDisposed, "should have been disposed of.");
		}

		/// <summary>
		/// Test that the wrapped IEnumerator has Dispose called even when
		/// the JoinedEnumerator doesn't enumerate all the way through the
		/// collection.
		/// </summary>
		[Test]
		public void WrapsSingleBreak()
		{
			int[] expected = new int[] {1, 2, 3};

			EnumerableTester first;
			JoinedEnumerable joined = InitSingle(out first);

			foreach (int actual in joined)
			{
				Assert.AreEqual(expected[0], actual, "first one was not what was expected.");
				break;
			}

			// ensure that the first was disposed of even though we didn't enumerate
			// all the way through
			Assert.IsTrue(first.WasDisposed, "should have been disposed of.");
		}

		/// <summary>
		/// Test that the JoinedEnumerable works with multiple wrapped
		/// IEnumerable as expected when fully enumerating the collections.
		/// </summary>
		[Test]
		public void WrapsMultiple()
		{
			int[] expected = new int[] {1, 2, 3, 4, 5, 6};

			EnumerableTester first;
			EnumerableTester second;
			JoinedEnumerable joined = InitMultiple(out first, out second);

			int index = 0;

			foreach (int actual in joined)
			{
				Assert.AreEqual(expected[index], actual, "Failure at " + index.ToString());
				index++;
			}

			Assert.AreEqual(expected.Length, index, "Every expected value was found");
			Assert.IsTrue(first.WasDisposed, "first should have been disposed of.");
			Assert.IsTrue(second.WasDisposed, "second should have been disposed of. ");
		}

		/// <summary>
		/// Test that the JoinedEnumerable works with multiple wrapped
		/// IEnumerable as expected when breaking out.
		/// </summary>
		[Test]
		public void WrapsMultipleBreak()
		{
			// break in the first IEnumerator
			WrapsMultipleBreak(2);

			// ensure behavior is consistent if break in 2nd IEnumerator
			WrapsMultipleBreak(4);
		}

		private void WrapsMultipleBreak(int breakIndex)
		{
			int[] expected = new int[] {1, 2, 3, 4, 5, 6};

			EnumerableTester first;
			EnumerableTester second;
			JoinedEnumerable joined = InitMultiple(out first, out second);

			int index = 0;

			foreach (int actual in joined)
			{
				Assert.AreEqual(expected[index], actual, "Failure at " + index.ToString());
				index++;
				if (index == breakIndex)
				{
					break;
				}
			}

			Assert.IsTrue(first.WasDisposed, "first should have been disposed of.");
			Assert.IsTrue(second.WasDisposed, "second should have been disposed of. ");
		}


		private JoinedEnumerable InitSingle(out EnumerableTester first)
		{
			first = new EnumerableTester(new ArrayList(new int[] {1, 2, 3}));
			return new JoinedEnumerable(new IEnumerable[] {first});
		}

		private JoinedEnumerable InitMultiple(out EnumerableTester first, out EnumerableTester second)
		{
			first = new EnumerableTester(new ArrayList(new int[] {1, 2, 3}));
			second = new EnumerableTester(new ArrayList(new int[] {4, 5, 6}));
			return new JoinedEnumerable(new IEnumerable[] {first, second});
		}
	}

	/// <summary>
	/// Simple class that wraps an array list for testing purposes.
	/// </summary>
	internal class EnumerableTester : IEnumerable
	{
		private ArrayList _list;
		private EnumerableTesterEnumerator _enumerator;

		public EnumerableTester(ArrayList wrappedList)
		{
			_list = wrappedList;
			_enumerator = new EnumerableTesterEnumerator(_list.GetEnumerator());
		}

		public bool WasDisposed
		{
			get { return _enumerator.IsDisposed; }
		}

		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return _enumerator;
		}

		#endregion

		private class EnumerableTesterEnumerator : IEnumerator, IDisposable
		{
			private IEnumerator _enumerator;
			private bool _isDisposed;

			public EnumerableTesterEnumerator(IEnumerator enumerator)
			{
				_enumerator = enumerator;
			}

			public bool IsDisposed
			{
				get { return _isDisposed; }
			}

			#region IEnumerator Members

			public void Reset()
			{
				_isDisposed = false;
				_enumerator.Reset();
			}

			public object Current
			{
				get { return _enumerator.Current; }
			}

			public bool MoveNext()
			{
				return _enumerator.MoveNext();
			}

			#endregion

			#region IDisposable Members

			public void Dispose()
			{
				_isDisposed = true;
			}

			#endregion
		}
	}
}