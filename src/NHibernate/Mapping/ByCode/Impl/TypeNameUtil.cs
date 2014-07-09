using NHibernate.Cfg.MappingSchema;
using NHibernate.Type;

namespace NHibernate.Mapping.ByCode.Impl
{
	public static class TypeNameUtil
	{
		public static string GetNhTypeName(this System.Type type)
		{
			string typeName;
			IType nhType = TypeFactory.HeuristicType(type.AssemblyQualifiedName);
			if (nhType != null)
			{
				typeName = nhType.Name;
			}
			else
			{
				typeName = type.FullName + ", " + type.Assembly.GetName().Name;
			}
			return typeName;
		}

		public static string GetShortClassName(this System.Type type, HbmMapping mapDoc)
		{
			if (type == null)
			{
				return null;
			}
			if (mapDoc == null)
			{
				return type.AssemblyQualifiedName;
			}
			string typeAssembly = type.Assembly.GetName().Name;
			string typeAssemblyFullName = type.Assembly.FullName;
			string typeNameSpace = type.Namespace;
			string assembly = null;
			if (typeAssembly != mapDoc.assembly && typeAssemblyFullName != mapDoc.assembly)
			{
				assembly = typeAssembly;
			}
			string @namespace = null;
			if (typeNameSpace != mapDoc.@namespace)
			{
				@namespace = typeNameSpace;
			}
			if (!string.IsNullOrEmpty(assembly) && !string.IsNullOrEmpty(@namespace))
			{
				return type.AssemblyQualifiedName;
			}
			if (!string.IsNullOrEmpty(assembly) && string.IsNullOrEmpty(@namespace))
			{
				return string.Concat(GetTypeNameForMapping(type), ", ", assembly);
			}
			if (string.IsNullOrEmpty(assembly) && !string.IsNullOrEmpty(@namespace))
			{
				return type.FullName;
			}

			return GetTypeNameForMapping(type);
		}

		private static string GetTypeNameForMapping(System.Type type)
		{
			if (type.IsGenericType || type.IsNested)
			{
				return type.FullName;
			}
			return type.Name;
		}
	}
}