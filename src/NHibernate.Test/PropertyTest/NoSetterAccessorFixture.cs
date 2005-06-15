using System;

using NHibernate.Property;

using NUnit.Framework;

namespace NHibernate.Test.PropertyTest
{
	/// <summary>
	/// Base test fixture for the NoSetter Accessors.
	/// </summary>
	[TestFixture]
	public abstract class NoSetterAccessorFixture
	{
		protected IPropertyAccessor _accessor;
		protected IGetter _getter;
		protected ISetter _setter;
		protected FieldClass _instance;

		protected bool _expectedBlahGetterCalled = false;
		protected bool _expectedCamelBazGetterCalled = false;
		protected bool _expectedCamelUnderscoreFooGetterCalled = false;
		protected bool _expectedLowerUnderscoreFooGetterCalled = false;
		protected bool _expectedLowerFooGetterCalled = false;
		protected bool _expectedPascalUnderscoreFooGetterCalled = false;

		/// <summary>
		/// SetUp the local fields for the test cases.
		/// </summary>
		/// <remarks>
		/// Any classes testing out their field access should override this
		/// and setup their FieldClass instance so that whichever field is
		/// going to be reflected upon is initialized to 0.
		/// </remarks>
		[SetUp]
		public abstract void SetUp();

		[Test]
		public void GetValue() 
		{
			
			Assert.AreEqual( 0, _getter.Get(_instance) );
			_instance.Increment();
			Assert.AreEqual( 1, _getter.Get(_instance) );
			
			Assert.AreEqual( _expectedBlahGetterCalled, _instance.BlahGetterCalled, "pascalcase-m-underscore" );
			Assert.AreEqual( _expectedCamelBazGetterCalled, _instance.CamelBazGetterCalled, "camelcase" );
			Assert.AreEqual( _expectedCamelUnderscoreFooGetterCalled, _instance.CamelUnderscoreFooGetterCalled, "camelcase-underscore" );
			Assert.AreEqual( _expectedLowerUnderscoreFooGetterCalled, _instance.LowerUnderscoreFooGetterCalled, "lowercase-underscore" );
			Assert.AreEqual( _expectedLowerFooGetterCalled, _instance.LowerFooGetterCalled, "lowercase" );
			Assert.AreEqual( _expectedPascalUnderscoreFooGetterCalled, _instance.PascalUnderscoreFooCalled, "pascalcase-underscore" );

		}

		[Test]
		public void SetValue() 
		{
			Assert.AreEqual( 0, _getter.Get(_instance) );
			_setter.Set( _instance, 5 );
			Assert.AreEqual( 5, _getter.Get(_instance) );
		}
	}
}
