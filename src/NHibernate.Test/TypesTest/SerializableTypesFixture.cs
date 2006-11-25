using System;
using System.Reflection;
using NUnit.Framework;
using NHibernate.Type;

namespace NHibernate.Test.TypesTest
{
	[TestFixture]
	public class SerializableTypesFixture
	{
		[Test]
		public void AllEmbeddedTypesAreMarkedSerializable()
		{
			NHAssert.InheritedAreMarkedSerializable(typeof(NHibernate.Type.IType));
		}

		[Test]
		public void EachEmbeddedBasicTypeIsSerializable()
		{
			FieldInfo[] builtInCustomTypes = typeof(NHibernateUtil).GetFields(BindingFlags.Public | BindingFlags.Static);
			foreach (FieldInfo fieldInfo in builtInCustomTypes)
			{
				IType ntp = (IType)fieldInfo.GetValue(null);
				NHAssert.IsSerializable(ntp, fieldInfo.Name + " is not serializable");
			}
		}
	}
}
