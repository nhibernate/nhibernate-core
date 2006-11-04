using System;

using NHibernate.Proxy;

using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.ProxyValidator
{
	[TestFixture]
	public class Fixture
	{
		private void Validate( System.Type type )
		{
			ProxyTypeValidator.ValidateType( type );
		}

		public class ValidClass
		{
#pragma warning disable 169
			private int privateField;
#pragma warning restore 169
			protected int protectedField;

			public virtual int SomeProperty
			{
				get { return 0; }
				set { }
			}

			public virtual void SomeMethod( int arg1, object arg2 )
			{
			}
#pragma warning disable 67
			public virtual event EventHandler VirtualEvent;
#pragma warning restore 67
		}

		[Test]
		public void ValidClassTest()
		{
			Validate( typeof( ValidClass ) );
		}

		public class InvalidPrivateConstructor : ValidClass
		{
			private InvalidPrivateConstructor()
			{
			}
		}

		[Test]
		[ExpectedException( typeof( InvalidProxyTypeException ) )]
		public void PrivateConstructor()
		{
			Validate( typeof( InvalidPrivateConstructor ) );
		}

		public class InvalidNonVirtualProperty : ValidClass
		{
			public int NonVirtualProperty
			{
				get { return 1; }
				set { }
			}
		}

		[Test]
		[ExpectedException( typeof( InvalidProxyTypeException ) )]
		public void NonVirtualProperty()
		{
			Validate( typeof( InvalidNonVirtualProperty ) );
		}

		public class InvalidPublicField : ValidClass
		{
			public int publicField;
		}

		[Test]
		[ExpectedException( typeof( InvalidProxyTypeException ) )]
		public void PublicField()
		{
			Validate( typeof( InvalidPublicField ) );
		}

		public class InvalidNonVirtualEvent : ValidClass
		{
#pragma warning disable 67
			public event EventHandler NonVirtualEvent;
#pragma warning restore 67
		}

		[Test]
		[ExpectedException( typeof( InvalidProxyTypeException ) )]
		public void NonVirtualEvent()
		{
			Validate( typeof( InvalidNonVirtualEvent ) );
		}

		public interface ValidInterface
		{
		}

		[Test]
		public void Interface()
		{
			Validate( typeof( ValidInterface ) );
		}
	}
}
