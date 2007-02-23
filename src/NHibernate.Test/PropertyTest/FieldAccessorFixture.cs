using System;
using NHibernate.Property;
using NUnit.Framework;

namespace NHibernate.Test.PropertyTest
{
	/// <summary>
	/// Base test fixture for the Field Accessors.
	/// </summary>
	[TestFixture]
	public class FieldAccessorFixture
	{
		protected IPropertyAccessor _accessor;
		protected IGetter _getter;
		protected ISetter _setter;
		protected FieldClass _instance;

		/// <summary>
		/// SetUp the local fields for the test cases.
		/// </summary>
		/// <remarks>
		/// Any classes testing out their field access should override this
		/// and setup their FieldClass instance so that whichever field is
		/// going to be reflected upon is initialized to 0.
		/// </remarks>
		[SetUp]
		public virtual void SetUp()
		{
			_accessor = PropertyAccessorFactory.GetPropertyAccessor("field");
			_getter = _accessor.GetGetter(typeof(FieldClass), "Id");
			_setter = _accessor.GetSetter(typeof(FieldClass), "Id");
			_instance = new FieldClass();
			_instance.InitId(0);
		}

		[Test]
		public void GetValue()
		{
			Assert.AreEqual(0, _getter.Get(_instance));
			_instance.Increment();
			Assert.AreEqual(1, _getter.Get(_instance));

			Assert.IsFalse(_instance.BlahGetterCalled);
			Assert.IsFalse(_instance.CamelBazGetterCalled);
			Assert.IsFalse(_instance.CamelUnderscoreFooGetterCalled);
			Assert.IsFalse(_instance.LowerUnderscoreFooGetterCalled);
		}

		[Test]
		public void SetValue()
		{
			Assert.AreEqual(0, _getter.Get(_instance));
			_setter.Set(_instance, 5);
			Assert.AreEqual(5, _getter.Get(_instance));
		}
	}
}