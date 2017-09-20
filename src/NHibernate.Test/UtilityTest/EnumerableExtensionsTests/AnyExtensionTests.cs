using System;
using System.Collections;
using NUnit.Framework;
using NHibernate.Util;

namespace NHibernate.Test.UtilityTest.EnumerableExtensionsTests
{
	[TestFixture]
	public class AnyExtensionTests
	{
		[Test]
		public void WhenEmptyListThenReturnFalse()
		{
#pragma warning disable 618
			Assert.That((new object[0]).Any(), Is.False);
#pragma warning restore 618
		}

		[Test]
		public void WhenNoEmptyListThenReturnTrue()
		{
#pragma warning disable 618
			Assert.That((new object[1]).Any(), Is.True);
#pragma warning restore 618
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
#pragma warning disable 618
			(new MyDisposableList(()=> disposeCalled = true)).Any();
#pragma warning restore 618
			Assert.That(disposeCalled, Is.True);
		}
	}
}
