using System;

using NUnit.Framework;

namespace NHibernate.Test.ExceptionsTest
{
	[TestFixture]
	public class PropertyAccessExceptionFixture
	{
		/// <summary>
		/// Verifying that NH-358 has been fixed.
		/// </summary>
		[Test]
		public void MessageWithoutTypeCtor()
		{
			PropertyAccessException exc = new PropertyAccessException( "notype" );
			Assert.AreEqual( "notype", exc.Message );

			exc = new PropertyAccessException();
			Assert.AreEqual( "A problem occurred accessing a mapped property of an instance of a persistent class by reflection.", exc.Message );

		}
	}
}
