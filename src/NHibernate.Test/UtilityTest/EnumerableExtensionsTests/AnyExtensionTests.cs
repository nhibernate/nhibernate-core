using System;
using System.Collections;
using NUnit.Framework;
using NHibernate.Util;
using SharpTestsEx;

namespace NHibernate.Test.UtilityTest.EnumerableExtensionsTests
{
	[TestFixture]
	public class AnyExtensionTests
	{
		[Test]
		public void WhenEmptyListThenReturnFalse()
		{
			(new object[0]).Any().Should().Be.False();
		}

		[Test]
		public void WhenNoEmptyListThenReturnTrue()
		{
			(new object[1]).Any().Should().Be.True();
		}

		private class MyDisposableList: IEnumerable
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
			(new MyDisposableList(()=> disposeCalled = true)).Any();
			disposeCalled.Should().Be.True();
		}
	}
}