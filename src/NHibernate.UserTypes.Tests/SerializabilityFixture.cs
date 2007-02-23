using System;
using System.Reflection;

using NHibernate.Type;
using NHibernate.UserTypes.SqlTypes;

using Nullables.NHibernate;

using NUnit.Framework;

namespace NHibernate.UserTypes.Tests
{
	[TestFixture]
	public class SerializabilityFixture
	{
		public static void CheckITypesInAssembly(Assembly assembly)
		{
			foreach (System.Type type in assembly.GetTypes())
			{
				if (type.IsClass && !type.IsAbstract && typeof(IType).IsAssignableFrom(type))
				{
					Assert.IsTrue(type.IsSerializable, "Type {0} should be serializable", type);
				}
			}
		}

		[Test]
		public void SqlTypes()
		{
			CheckITypesInAssembly(typeof(SqlInt32Type).Assembly);
		}

		[Test]
		public void NullableTypes()
		{
			CheckITypesInAssembly(typeof(NullableInt32Type).Assembly);
		}
	}
}