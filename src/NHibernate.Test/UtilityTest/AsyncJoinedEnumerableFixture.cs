using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Util;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace NHibernate.Test.UtilityTest
{
	/// <summary>
	/// Test cases for the JoinedAsyncEnumerable class.
	/// </summary>
	// 6.0 TODO: Remove
	[TestFixture]
	public class AsyncJoinedEnumerableFixture
	{
		/// <summary>
		/// Test that the JoinedAsyncEnumerable works with a single wrapped
		/// IAsyncEnumerable as expected when fully enumerating the collection.
		/// </summary>
		[Test]
		public async Task WrapsSingleAsync()
		{
			var expected = new int[] {1, 2, 3};
			var joined = InitSingle(out var first).GetAsyncEnumerator(default);
			var index = 0;

			// Simulate await foreach by using MoveNextAsync and DisposeAsync
			while (await joined.MoveNextAsync())
			{
				Assert.That(joined.Current, Is.EqualTo(expected[index]), "Failure at " + index.ToString());
				index++;
			}

			await joined.DisposeAsync();

			Assert.AreEqual(expected.Length, index, "Every expected value was found");
			Assert.IsTrue(first.EnumeratorDisposed, "Should have been disposed of.");
		}

		/// <summary>
		/// Test that the wrapped IAsyncEnumerable has DisposeAsync called even when
		/// the JoinedAsyncEnumerable doesn't enumerate all the way through the
		/// collection.
		/// </summary>
		[Test]
		public async Task WrapsSingleBreakAsync()
		{
			int[] expected = new int[] {1, 2, 3};
			var joined = InitSingle(out var first).GetAsyncEnumerator(default);

			await joined.MoveNextAsync();
			Assert.That(joined.Current, Is.EqualTo(expected[0]), "First one was not what was expected.");
			await joined.DisposeAsync();


			// ensure that the first was disposed of even though we didn't enumerate
			// all the way through
			Assert.IsTrue(first.EnumeratorDisposed, "Should have been disposed of.");
		}

		/// <summary>
		/// Test that the JoinedAsyncEnumerable works with multiple wrapped
		/// IAsyncEnumerable as expected when fully enumerating the collections.
		/// </summary>
		[Test]
		public async Task WrapsMultipleAsync()
		{
			var expected = new int[] {1, 2, 3, 4, 5, 6};
			var joined = InitMultiple(out var first, out var second).GetAsyncEnumerator(default);
			var index = 0;

			// Simulate await foreach by using MoveNextAsync and DisposeAsync
			while (await joined.MoveNextAsync())
			{
				Assert.That(joined.Current, Is.EqualTo(expected[index]), "Failure at " + index.ToString());
				index++;
			}

			await joined.DisposeAsync();

			Assert.AreEqual(expected.Length, index, "Every expected value was found");
			Assert.IsTrue(first.EnumeratorDisposed, "First should have been disposed of.");
			Assert.IsTrue(second.EnumeratorDisposed, "Second should have been disposed of.");
		}

		/// <summary>
		/// Test that the JoinedAsyncEnumerable works with multiple wrapped
		/// IAsyncEnumerable as expected when breaking out.
		/// </summary>
		[Test]
		public async Task WrapsMultipleBreak()
		{
			// break in the first IEnumerator
			await WrapsMultipleBreakAsync(2);

			// ensure behavior is consistent if break in 2nd IEnumerator
			await WrapsMultipleBreakAsync(4);
		}

		private async Task WrapsMultipleBreakAsync(int breakIndex)
		{
			var expected = new int[] {1, 2, 3, 4, 5, 6};
			var joined = InitMultiple(out var first, out var second).GetAsyncEnumerator(default);
			var index = 0;

			// Simulate await foreach by using MoveNextAsync and DisposeAsync
			while (index != breakIndex && await joined.MoveNextAsync())
			{
				Assert.That(joined.Current, Is.EqualTo(expected[index]), "Failure at " + index.ToString());
				index++;
			}

			await joined.DisposeAsync();

			Assert.That(first.EnumeratorDisposed, Is.True, "First should have been disposed of.");
			Assert.That(second.EnumeratorCreated, breakIndex > 3 ? (IConstraint) Is.True : Is.False, "Second should have been created.");
			Assert.That(second.EnumeratorDisposed, breakIndex > 3 ? (IConstraint) Is.True : Is.False, "Second should have been disposed of.");
		}

		private IAsyncEnumerable<int> InitSingle(out AsyncEnumerableTester<int> first)
		{
			first = new AsyncEnumerableTester<int>(new List<int>{1, 2, 3});
			return CreateJoinedAsyncEnumerable(new List<IAsyncEnumerable<int>> { first });
		}

		private IAsyncEnumerable<int> InitMultiple(out AsyncEnumerableTester<int> first, out AsyncEnumerableTester<int> second)
		{
			first = new AsyncEnumerableTester<int>(new List<int>{1, 2, 3});
			second = new AsyncEnumerableTester<int>(new List<int>{4, 5, 6});
			return CreateJoinedAsyncEnumerable(new List<IAsyncEnumerable<int>> { first, second });
		}

		private IAsyncEnumerable<T> CreateJoinedAsyncEnumerable<T>(IEnumerable<IAsyncEnumerable<T>> asyncEnumerables)
		{
			var type = typeof(ISession).Assembly.GetType("NHibernate.Util.JoinedAsyncEnumerable`1").MakeGenericType(typeof(T));
			return (IAsyncEnumerable<T>) type.GetConstructors()
				.First()
				.Invoke(new object[] { asyncEnumerables });
		}
	}

	/// <summary>
	/// Simple class that wraps an array list for testing purposes.
	/// </summary>
	internal class AsyncEnumerableTester<T> : IAsyncEnumerable<T>
	{
		private readonly List<T> _list;
		private AsyncEnumerableTesterEnumerator _enumerator;

		public AsyncEnumerableTester(List<T> wrappedList)
		{
			_list = wrappedList;
		}

		public bool EnumeratorCreated => _enumerator != null;

		public bool EnumeratorDisposed => _enumerator?.IsDisposed == true;

		public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
		{
			_enumerator = new AsyncEnumerableTesterEnumerator(_list.GetEnumerator());
			return _enumerator;
		}

		private sealed class AsyncEnumerableTesterEnumerator : IAsyncEnumerator<T>
		{
			private readonly IEnumerator<T> _enumerator;

			public AsyncEnumerableTesterEnumerator(IEnumerator<T> enumerator)
			{
				_enumerator = enumerator;
			}

			public bool IsDisposed { get; private set; }

			public T Current => _enumerator.Current;

			public ValueTask<bool> MoveNextAsync()
			{
				return new ValueTask<bool>(_enumerator.MoveNext());
			}

			public ValueTask DisposeAsync()
			{
				IsDisposed = true;
				return default;
			}
		}
	}
}
