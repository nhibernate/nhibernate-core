using System;
using System.Collections;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.UtilityTest.EnumerableExtensionsTests
{
	//Since v5.1
	[Obsolete]
	[TestFixture]
	public class AnyExtensionTests
	{
		[Test]
		public void WhenEmptyListThenReturnFalse()
		{
			Assert.That((Array.Empty<object>()).Any(), Is.False);
		}

		[Test]
		public void WhenNoEmptyListThenReturnTrue()
		{
			Assert.That((new object[1]).Any(), Is.True);
		}

		private class MyDisposableList : IEnumerable
		{
			private readonly System.Action enumeratorDisposeCallback;

			public MyDisposableList(System.Action enumeratorDisposeCallback)
			{
				this.enumeratorDisposeCallback = enumeratorDisposeCallback;
			}

			public IEnumerator GetEnumerator()
			{
				return new EmptyEnumerator(enumeratorDisposeCallback);
			}
		}

		private class EmptyEnumerator : IEnumerator, IDisposable
		{
			private readonly System.Action disposeCallback;

			public EmptyEnumerator(System.Action disposeCallback)
			{
				this.disposeCallback = disposeCallback;
			}

			public void Reset()
			{
			}

			public object Current
			{
				get { throw new InvalidOperationException("EmptyEnumerator"); }
			}

			public bool MoveNext()
			{
				return false;
			}

			public void Dispose()
			{
				disposeCallback();
			}
		}

		[Test]
		public void WhenDisposableListThenCallDispose()
		{
			var disposeCalled = false;
			(new MyDisposableList(() => disposeCalled = true)).Any();
			Assert.That(disposeCalled, Is.True);
		}
	}
}
