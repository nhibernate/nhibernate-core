using System;
using NHibernate.Proxy;
using NUnit.Framework;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.ProxyValidator
{
	[TestFixture]
	public class Fixture
	{
		private readonly IProxyValidator pv = new DynProxyTypeValidator();

		private void Validate(System.Type type)
		{
			ICollection<string> errors = pv.ValidateType(type);
			if (errors != null)
			{
				throw new InvalidProxyTypeException(errors);
			}
		}

		public class ValidClass
		{
			private int privateField;
			protected int protectedField;

			public virtual int SomeProperty
			{
				get { return privateField; }
				set { privateField = value; }
			}

			public virtual void SomeMethod(int arg1, object arg2)
			{
			}

#pragma warning disable 67
			public virtual event EventHandler VirtualEvent;
#pragma warning restore 67

			protected void NonVirtualProtectedMethod()
			{
			}

			protected int NonVirtualProtectedProperty
			{
				get { return 0; }
				set { }
			}
#pragma warning disable 67
			protected event EventHandler NonVirtualProtectedEvent;
#pragma warning restore 67
			protected void NonVirtualPrivateMethod()
			{
			}

			protected int NonVirtualPrivateProperty
			{
				get { return 0; }
				set { }
			}
#pragma warning disable 67
			protected event EventHandler NonVirtualPrivateEvent;
#pragma warning restore 67
		}

		[Test]
		public void ValidClassTest()
		{
			Validate(typeof(ValidClass));
		}

		public class InvalidPrivateConstructor : ValidClass
		{
			private InvalidPrivateConstructor()
			{
			}
		}

		[Test]
		[ExpectedException(typeof(InvalidProxyTypeException))]
		public void PrivateConstructor()
		{
			Validate(typeof(InvalidPrivateConstructor));
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
		[ExpectedException(typeof(InvalidProxyTypeException))]
		public void NonVirtualProperty()
		{
			Validate(typeof(InvalidNonVirtualProperty));
		}

		public class InvalidPublicField : ValidClass
		{
			public int publicField;
		}

		[Test]
		[ExpectedException(typeof(InvalidProxyTypeException))]
		public void PublicField()
		{
			Validate(typeof(InvalidPublicField));
		}

		public class InvalidNonVirtualEvent : ValidClass
		{
#pragma warning disable 67
			public event EventHandler NonVirtualEvent;
#pragma warning restore 67
		}

		[Test]
		[ExpectedException(typeof(InvalidProxyTypeException))]
		public void NonVirtualEvent()
		{
			Validate(typeof(InvalidNonVirtualEvent));
		}

		public interface ValidInterface
		{
		}

		[Test]
		public void Interface()
		{
			Validate(typeof(ValidInterface));
		}

		public class MultipleErrors
		{
			private MultipleErrors()
			{
			}

			public int publicField;
#pragma warning disable 67
			public event EventHandler NonVirtualEvent;
#pragma warning restore 67

			public int NonVirtualProperty
			{
				get { return 1; }
				set { }
			}
		}

		[Test]
		public void MultipleErrorsReported()
		{
			try
			{
				Validate(typeof(MultipleErrors));
				Assert.Fail("Should have failed validation");
			}
			catch (InvalidProxyTypeException e)
			{
				Assert.IsTrue(e.Errors.Count > 1);
			}
		}

		public class InvalidNonVirtualInternalProperty : ValidClass
		{
			internal int NonVirtualProperty
			{
				get { return 1; }
				set { }
			}
		}

		public class InvalidInternalField : ValidClass
		{
#pragma warning disable 649
			internal int internalField;
#pragma warning restore 649
		}

		[Test]
		[ExpectedException(typeof(InvalidProxyTypeException))]
		public void NonVirtualInternal()
		{
			Validate(typeof(InvalidNonVirtualInternalProperty));
		}

		[Test]
		[ExpectedException(typeof(InvalidProxyTypeException))]
		public void InternalField()
		{
			Validate(typeof(InvalidInternalField));
		}

		public class InvalidNonVirtualProtectedProperty : ValidClass
		{
			protected int NonVirtualProperty
			{
				get { return 1; }
				set { }
			}
		}

		[Test]
		public void NonVirtualProtected()
		{
			Validate(typeof(InvalidNonVirtualProtectedProperty));
			Assert.IsTrue(true, "Always should pass, protected members do not need to be virtual.");
		}

		public class InvalidNonVirtualProtectedInternalProperty : ValidClass
		{
			protected internal int NonVirtualProperty
			{
				get { return 1; }
				set { }
			}
		}

		[Test]
		[ExpectedException(typeof(InvalidProxyTypeException))]
		public void NonVirtualProtectedInternal()
		{
			Validate(typeof(InvalidNonVirtualProtectedInternalProperty));
		}

		interface INonVirtualPublicImplementsInterface
		{
			int NonVirtualMethodImplementsInterface { get; }
		}

		public class NonVirtualPublicImplementsInterface : ValidClass, INonVirtualPublicImplementsInterface
		{
			public int NonVirtualMethodImplementsInterface
			{
				get { return 0; }
			}
		}

		[Test]
		[ExpectedException(typeof(InvalidProxyTypeException))]
		public void VirtualPublicImplementsInterface()
		{
			Validate(typeof(NonVirtualPublicImplementsInterface));
		} 
	}
}
