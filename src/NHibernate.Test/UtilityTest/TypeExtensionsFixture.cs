using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework;

namespace NHibernate.Test.UtilityTest
{
	[TestFixture]
	public class TypeExtensionsFixture
	{
		protected Func<System.Type, IEnumerable<FieldInfo>> GetUserDeclaredFields { get; private set; }

		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			var getUserDeclaredFieldsMethod = typeof(Util.TypeExtensions).GetMethod(
				nameof(GetUserDeclaredFields),
				BindingFlags.Static | BindingFlags.NonPublic);
			Assert.That(
				getUserDeclaredFieldsMethod,
				Is.Not.Null,
				$"{nameof(Util.TypeExtensions)}.{nameof(GetUserDeclaredFields)} is not found");

			var parameter = Expression.Parameter(typeof(System.Type));
			var methodCall = Expression.Call(null, getUserDeclaredFieldsMethod, parameter);

			GetUserDeclaredFields = Expression.Lambda<Func<System.Type, IEnumerable<FieldInfo>>>(methodCall, parameter)
			                                  .Compile();
		}

		[Test]
		public virtual void GetUserDeclaredFieldsIgnoreAnonymousBackingField()
		{
			// Anonymous types are implemented with properties, their backing fields should be ignored
			Assert.That(GetUserDeclaredFields(new { a = 0 }.GetType()), Is.Empty);
		}

		[Test]
		public virtual void GetUserDeclaredFieldsIgnoreAutoPropertiesBackingField()
		{
			Assert.That(
				GetUserDeclaredFields(typeof(TestClass)),
				Has.All.Property(nameof(MemberInfo.Name)).EqualTo("_field"));
		}

		private class TestClass
		{
			public int Property { get; set; }

			private int _field;

			public void DoSomething()
			{
				_field += Property;
				if (_field > 10)
					_field = 0;
			}
		}
	}
}
