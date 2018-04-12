using System;
using System.Collections.Generic;
using System.Linq;
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
			var testExplicit = new HashSet<string>
			{
				"Serializable",
				"Object"
			};
			
			var builtInCustomTypes =
				typeof(NHibernateUtil)
					.GetFields(BindingFlags.Public | BindingFlags.Static)
					.Where(f => !testExplicit.Contains(f.Name));
			
			foreach (var fieldInfo in builtInCustomTypes)
			{
				var ntp = (IType) fieldInfo.GetValue(null);
				NHAssert.IsSerializable(ntp, fieldInfo.Name + " is not serializable");
			}

			if (typeof(System.Type).IsSerializable)
			{
				NHAssert.IsSerializable(NHibernateUtil.Serializable);
				NHAssert.IsSerializable(NHibernateUtil.Object);
			}
		}
	}
}