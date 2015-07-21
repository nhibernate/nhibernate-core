using System.Reflection;

namespace NHibernate.Mapping.ByCode
{
	public static class ForClass<T>
	{
		private const BindingFlags DefaultFlags =
			BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

		public static FieldInfo Field(string fieldName)
		{
			if (fieldName == null)
			{
				return null;
			}

			return GetField(typeof(T), fieldName);
		}

		private static FieldInfo GetField(System.Type type, string fieldName)
		{
			if(type == typeof(object) || type == null)
			{
				return null;
			}
			FieldInfo member = type.GetField(fieldName, DefaultFlags) ?? GetField(type.BaseType, fieldName);
			return member;
		}
	}
}