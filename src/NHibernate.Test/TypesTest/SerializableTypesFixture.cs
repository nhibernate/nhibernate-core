using System;
using System.Reflection;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	[TestFixture]
	public class SerializableTypesFixture
	{
		[Test]
		public void AllEmbeddedTypesAreMarkedSerializable()
		{
			NHAssert.InheritedAreMarkedSerializable(typeof(IType));
		}

		[Test]
		public void EachEmbeddedBasicTypeIsSerializable()
		{
			FieldInfo[] builtInCustomTypes = typeof(NHibernateUtil).GetFields(BindingFlags.Public | BindingFlags.Static);
			foreach (FieldInfo fieldInfo in builtInCustomTypes)
			{
				IType ntp = (IType) fieldInfo.GetValue(null);
				NHAssert.IsSerializable(ntp, fieldInfo.Name + " is not serializable");
			}
		}
	}
}