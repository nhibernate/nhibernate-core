using System;
using System.Security;
using System.Security.Permissions;
using NHibernate.Proxy;
using NUnit.Framework;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.ProxyValidator
{
	[TestFixture]
	public class Fixture
	{
		private readonly IProxyValidator pv = new DynProxyTypeValidator();

        private static void Validate(IProxyValidator validator, System.Type type)
        {
            var errors = validator.ValidateType(type);
            if (errors != null)
            {
                throw new InvalidProxyTypeException(errors);
            }
        }

	    private void Validate(System.Type type)
	    {
	        Validate(pv, type);
	    }

	    private void ValidateInNonTrustedDomain(System.Type type)
        {

            var setup = new AppDomainSetup { ApplicationBase = AppDomain.CurrentDomain.BaseDirectory, ShadowCopyFiles = "true"};
            var permissions = new PermissionSet(null);
            permissions.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
            permissions.AddPermission(new SecurityPermission(SecurityPermissionFlag.SerializationFormatter));
            permissions.AddPermission(new FileIOPermission(PermissionState.Unrestricted));
            var domain = AppDomain.CreateDomain("Non Trust AppDomain", null, setup, permissions);
	        var validatorType = typeof (DynProxyTypeValidatorCrossDomainDecorator);
            var validator = (DynProxyTypeValidatorCrossDomainDecorator)
                    domain.CreateInstanceAndUnwrap(validatorType.Assembly.FullName, validatorType.FullName);
            validator.ValidateType(type);
        }



		public class ValidClass
		{
			private int privateField;
			protected int protectedField;

		    public ValidClass(int privateField)
		    {
		        this.privateField = privateField;
		    }

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

		public class ValidClassWithNonPrivateConstructor
		{
			
		}

		[Test]
		public void PrivateConstructorInTrustedEnvironment()
		{
			Assert.DoesNotThrow(() => Validate(typeof(ValidClass)));
		}


        [Test]
        public void NonPrivateConstructorInTrustedEnvironment()
        {
            Assert.DoesNotThrow(() => Validate(typeof(ValidClassWithNonPrivateConstructor)));
        }

        [Test]
        public void PrivateConstructorInNonTrustedEnvironment()
        {
            Assert.Throws<InvalidProxyTypeException>(() => ValidateInNonTrustedDomain(typeof(ValidClass)));
        }

        [Test]
        public void NonPrivateConstructorInNonTrustedEnvironment()
        {
            Assert.DoesNotThrow(() => ValidateInNonTrustedDomain(typeof(ValidClassWithNonPrivateConstructor)));
        }

		public class InvalidNonVirtualProperty : ValidClass
		{
		    public InvalidNonVirtualProperty(int privateField) : base(privateField)
		    {
		    }

		    public int NonVirtualProperty
			{
				get { return 1; }
				set { }
			}
		}

		[Test]
		public void NonVirtualProperty()
		{
			Assert.Throws<InvalidProxyTypeException>(() => Validate(typeof(InvalidNonVirtualProperty)));
		}

		public class InvalidPublicField : ValidClass
		{
			public int publicField;

		    public InvalidPublicField(int privateField) : base(privateField)
		    {
		    }
		}

		[Test]
		public void PublicField()
		{
			Assert.Throws<InvalidProxyTypeException>(() => Validate(typeof(InvalidPublicField)));
		}

		public class InvalidNonVirtualEvent : ValidClass
		{
            public InvalidNonVirtualEvent(int privateField)
                : base(privateField)
            {
            }
#pragma warning disable 67
		    public event EventHandler NonVirtualEvent;
#pragma warning restore 67
		}

		[Test]
		public void NonVirtualEvent()
		{
			Assert.Throws<InvalidProxyTypeException>(() => Validate(typeof(InvalidNonVirtualEvent)));
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
		    public InvalidNonVirtualInternalProperty(int privateField) : base(privateField)
		    {
		    }

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
		    public InvalidInternalField(int privateField) : base(privateField)
		    {
		    }
		}

		[Test]
		public void NonVirtualInternal()
		{
			Assert.Throws<InvalidProxyTypeException>(() => Validate(typeof(InvalidNonVirtualInternalProperty)));
		}

		[Test]
		public void InternalField()
		{
			Assert.Throws<InvalidProxyTypeException>(() => Validate(typeof(InvalidInternalField)));
		}

		public class InvalidNonVirtualProtectedProperty : ValidClass
		{
		    public InvalidNonVirtualProtectedProperty(int privateField) : base(privateField)
		    {
		    }

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
		    public InvalidNonVirtualProtectedInternalProperty(int privateField) : base(privateField)
		    {
		    }

		    protected internal int NonVirtualProperty
			{
				get { return 1; }
				set { }
			}
		}

		[Test]
		public void NonVirtualProtectedInternal()
		{
			Assert.Throws<InvalidProxyTypeException>(() => Validate(typeof(InvalidNonVirtualProtectedInternalProperty)));
		}

		interface INonVirtualPublicImplementsInterface
		{
			int NonVirtualMethodImplementsInterface { get; }
		}

		public class NonVirtualPublicImplementsInterface : ValidClass, INonVirtualPublicImplementsInterface
		{
		    public NonVirtualPublicImplementsInterface(int privateField) : base(privateField)
		    {
		    }

		    public int NonVirtualMethodImplementsInterface
			{
				get { return 0; }
			}
		}

		[Test]
		public void VirtualPublicImplementsInterface()
		{
			Assert.Throws<InvalidProxyTypeException>(() => Validate(typeof(NonVirtualPublicImplementsInterface)));
		}

		public class InvalidVirtualPrivateAutoProperty : ValidClass
		{
		    public InvalidVirtualPrivateAutoProperty(int privateField) : base(privateField)
		    {
		    }

		    public virtual int NonVirtualSetterProperty
			{
				get;
				private set;
			}
		}

		[Test]
		public void PrivateSetterOnVirtualPropertyShouldThrows()
		{
			Assert.Throws<InvalidProxyTypeException>(() => Validate(typeof(InvalidVirtualPrivateAutoProperty)));
		}

        public class DynProxyTypeValidatorCrossDomainDecorator : MarshalByRefObject
        {
            private readonly IProxyValidator pv;

            public DynProxyTypeValidatorCrossDomainDecorator()
            {
                pv = new DynProxyTypeValidator();
            }

            public void ValidateType(System.Type type)
            {
                Validate(pv, type);
            }
        }
	}
}
