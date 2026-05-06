using System;
using NHibernate.Proxy;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.ProxyValidator
{
	[TestFixture]
	public class Fixture
	{
		private readonly IProxyValidator pv = new DynProxyTypeValidator();

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
		public void ObjectIsValid()
		{
			var errors = pv.ValidateType(typeof(object));
			Assert.That(errors, Is.Null);
		}

		[Test]
		public void ValidClassTest()
		{
			var errors = pv.ValidateType(typeof(ValidClass));
			Assert.That(errors, Is.Null);
		}

		public class InvalidSealedToString : ValidClass
		{
			public sealed override string ToString()
			{
				return base.ToString();
			}
		}
		
		[Test]
		public void SealedObjectOverride()
		{
			var errors = pv.ValidateType(typeof(InvalidSealedToString));
			Assert.That(errors, Has.Count.EqualTo(1));
		}
		
		public class InvalidPrivateConstructor : ValidClass
		{
			private InvalidPrivateConstructor()
			{
			}
		}

		[Test]
		public void PrivateConstructor()
		{
			var errors = pv.ValidateType(typeof(InvalidPrivateConstructor));
			Assert.That(errors, Has.Count.EqualTo(1));
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
		public void NonVirtualProperty()
		{
			var errors = pv.ValidateType(typeof(InvalidNonVirtualProperty));
			Assert.That(errors, Has.Count.EqualTo(2));
		}

		public class InvalidPublicField : ValidClass
		{
			public int publicField;
		}

		[Test]
		public void PublicField()
		{
			var errors = pv.ValidateType(typeof(InvalidPublicField));
			Assert.That(errors, Has.Count.EqualTo(1));
		}

		public class InvalidNonVirtualEvent : ValidClass
		{
#pragma warning disable 67
			public event EventHandler NonVirtualEvent;
#pragma warning restore 67
		}

		[Test]
		public void NonVirtualEvent()
		{
			var errors = pv.ValidateType(typeof(InvalidNonVirtualEvent));
			Assert.That(errors, Has.Count.EqualTo(2));
		}

		public interface ValidInterface
		{
		}

		[Test]
		public void Interface()
		{
			var errors = pv.ValidateType(typeof(ValidInterface));
			Assert.That(errors, Is.Null);
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
			var errors = pv.ValidateType(typeof(MultipleErrors));
			Assert.That(errors, Has.Count.GreaterThan(1));
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
		public void NonVirtualInternal()
		{
			var errors = pv.ValidateType(typeof(InvalidNonVirtualInternalProperty));
			Assert.That(errors, Has.Count.EqualTo(2));
		}

		[Test]
		public void InternalField()
		{
			var errors = pv.ValidateType(typeof(InvalidInternalField));
			Assert.That(errors, Has.Count.EqualTo(1));
		}

		public class ValidNonVirtualProtectedProperty : ValidClass
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
			var errors = pv.ValidateType(typeof(ValidNonVirtualProtectedProperty));
			Assert.That(errors, Is.Null);
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
		public void NonVirtualProtectedInternal()
		{
			var errors = pv.ValidateType(typeof(InvalidNonVirtualProtectedInternalProperty));
			Assert.That(errors, Has.Count.EqualTo(2));
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
		public void VirtualPublicImplementsInterface()
		{
			var errors = pv.ValidateType(typeof(NonVirtualPublicImplementsInterface));
			Assert.That(errors, Has.Count.EqualTo(1));
		}

		public class InvalidVirtualPrivateAutoProperty : ValidClass
		{
			public virtual int NonVirtualSetterProperty
			{
				get;
				private set;
			}
		}

		[Test]
		public void PrivateSetterOnVirtualPropertyShouldThrows()
		{
			var errors = pv.ValidateType(typeof(InvalidVirtualPrivateAutoProperty));
			Assert.That(errors, Has.Count.EqualTo(1));
		}
	}
}
