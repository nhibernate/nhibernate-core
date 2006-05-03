using System;

using NHibernate.Property;
using NHibernate.Util;

using NUnit.Framework;

namespace NHibernate.Test.PropertyTest
{
	/// <summary>
	/// This is more of a test of ReflectHelper.GetGetter() to make sure that
	/// it will find the correct IGetter when a Property does not exist.
	/// </summary>
	[TestFixture]
	public class FieldGetterFixture
	{
		FieldGetterFixture.FieldGetterClass obj = new FieldGetterFixture.FieldGetterClass();

		[Test]
		public void NoNamingStrategy() 
		{
			
			IGetter fieldGetter = ReflectHelper.GetGetter( typeof(FieldGetterFixture.FieldGetterClass), "Id", "field" );

			Assert.IsNotNull( fieldGetter, "should have found getter" );
			Assert.AreEqual( typeof(FieldGetter), fieldGetter.GetType(), "IGetter should be for a field." );
			Assert.AreEqual( typeof(Int32), fieldGetter.ReturnType, "returns Int32." );
			Assert.IsNull( fieldGetter.Method, "no MethodInfo for fields." );
			Assert.IsNull( fieldGetter.PropertyName, "no Property Names for fields." );
			Assert.AreEqual( 7, fieldGetter.Get( obj ), "Get() for Int32" );
		}

		[Test]
		public void CamelCaseNamingStrategy() 
		{
			IGetter fieldGetter = ReflectHelper.GetGetter( typeof(FieldGetterFixture.FieldGetterClass), "PropertyOne", "field.camelcase" );

			Assert.IsNotNull( fieldGetter, "should have found getter" );
			Assert.AreEqual( typeof(FieldGetter), fieldGetter.GetType(), "IGetter should be for a field." );
			Assert.AreEqual( typeof(DateTime), fieldGetter.ReturnType, "returns DateTime." );
			Assert.IsNull( fieldGetter.Method, "no MethodInfo for fields." );
			Assert.IsNull( fieldGetter.PropertyName, "no Property Names for fields." );
			Assert.AreEqual( DateTime.Parse( "2000-01-01" ), fieldGetter.Get( obj ), "Get() for DateTime" );
		}

		[Test]
		public void CamelCaseUnderscoreNamingStrategy() 
		{
			IGetter fieldGetter = ReflectHelper.GetGetter( typeof(FieldGetterFixture.FieldGetterClass), "PropertyTwo", "field.camelcase-underscore" );

			Assert.IsNotNull( fieldGetter, "should have found getter" );
			Assert.AreEqual( typeof(FieldGetter), fieldGetter.GetType(), "IGetter should be for a field." );
			Assert.AreEqual( typeof(Boolean), fieldGetter.ReturnType, "returns Boolean." );
			Assert.IsNull( fieldGetter.Method, "no MethodInfo for fields." );
			Assert.IsNull( fieldGetter.PropertyName, "no Property Names for fields." );
			Assert.AreEqual( true, fieldGetter.Get( obj ), "Get() for Boolean" );
		}

		[Test]
		public void LowerCaseUnderscoreNamingStrategy() 
		{
			IGetter fieldGetter = ReflectHelper.GetGetter( typeof(FieldGetterFixture.FieldGetterClass), "PropertyFour", "field.lowercase-underscore" );

			Assert.IsNotNull( fieldGetter, "should have found getter" );
			Assert.AreEqual( typeof(FieldGetter), fieldGetter.GetType(), "IGetter should be for a field." );
			Assert.AreEqual( typeof(Int64), fieldGetter.ReturnType, "returns Int64." );
			Assert.IsNull( fieldGetter.Method, "no MethodInfo for fields." );
			Assert.IsNull( fieldGetter.PropertyName, "no Property Names for fields." );
			Assert.AreEqual( Int64.MaxValue, fieldGetter.Get( obj ), "Get() for Int64" );
		}

		[Test]
		public void PascalCaseMUnderscoreNamingStrategy() 
		{
			IGetter fieldGetter = ReflectHelper.GetGetter( typeof(FieldGetterFixture.FieldGetterClass), "PropertyThree", "field.pascalcase-m-underscore" );

			Assert.IsNotNull( fieldGetter, "should have found getter" );
			Assert.AreEqual( typeof(FieldGetter), fieldGetter.GetType(), "IGetter should be for a field." );
			Assert.AreEqual( typeof(TimeSpan), fieldGetter.ReturnType, "returns DateTime." );
			Assert.IsNull( fieldGetter.Method, "no MethodInfo for fields." );
			Assert.IsNull( fieldGetter.PropertyName, "no Property Names for fields." );
			Assert.AreEqual( new TimeSpan( DateTime.Parse( "2001-01-01" ).Ticks ), fieldGetter.Get( obj ), "Get() for TimeSpan" );
		}

		public class FieldGetterClass 
		{
			private int Id = 7;
			private DateTime propertyOne = DateTime.Parse( "2000-01-01" );
			private bool _propertyTwo = true;
			private TimeSpan m_PropertyThree = new TimeSpan( DateTime.Parse("2001-01-01" ).Ticks );
			private long _propertyfour = Int64.MaxValue;
		}
	}

	
}
