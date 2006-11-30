using System;
using NUnit.Framework;
using NHibernate.Test.Assertions;

namespace NHibernate.Test
{
	public class NHAssert
	{
		#region Serializable
		public static void HaveSerializableAttribute(System.Type clazz)
		{
			NHAssert.HaveSerializableAttribute(clazz, null, null);
		}

		public static void HaveSerializableAttribute(System.Type clazz, string message, params object[] args)
		{
			Assert.DoAssert(new HaveSerializableAttributeAsserter(clazz, message, args));
		}

		public static void InheritedAreMarkedSerializable(System.Type clazz)
		{
			NHAssert.InheritedAreMarkedSerializable(clazz, null, null);
		}

		public static void InheritedAreMarkedSerializable(System.Type clazz, string message, params object[] args)
		{
			Assert.DoAssert(new InheritedAreMarkedSerializable(clazz, message, args));
		}

		public static void IsSerializable(object obj)
		{
			NHAssert.IsSerializable(obj, null, null);
		}

		public static void IsSerializable(object obj, string message, params object[] args)
		{
			Assert.DoAssert(new IsSerializable(obj, message, args));
		}

		#endregion
	}
}
