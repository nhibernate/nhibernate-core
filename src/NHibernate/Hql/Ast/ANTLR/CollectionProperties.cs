using System;
using System.Collections.Generic;
using NHibernate.Persister.Collection;

namespace NHibernate.Hql.Ast.ANTLR
{
	/// <summary>
	/// Provides a map of collection function names to the corresponding property names.
	/// Author: josh
	/// Ported by: Steve Strong
	/// </summary>
	internal static class CollectionProperties
	{

		public static readonly Dictionary<string, string> HQL_COLLECTION_PROPERTIES = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

		static CollectionProperties()
		{
			Init(
				CollectionPropertyNames.Elements,
				CollectionPropertyNames.Indices,
				CollectionPropertyNames.Size,
				CollectionPropertyNames.MaxIndex,
				CollectionPropertyNames.MinIndex,
				CollectionPropertyNames.MaxElement,
				CollectionPropertyNames.MinElement,
				CollectionPropertyNames.Index);
		}

		private static void Init(params string[] names)
		{
			foreach (var name in names)
			{
				HQL_COLLECTION_PROPERTIES[name] = name;
			}
		}

		public static bool IsCollectionProperty(string name)
		{
			// CollectionPropertyMapping processes everything except 'index'.
			if (string.Equals(CollectionPropertyNames.Index, name, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			else
			{
				return HQL_COLLECTION_PROPERTIES.ContainsKey(name);
			}
		}

		public static string GetNormalizedPropertyName(string name)
		{
			return HQL_COLLECTION_PROPERTIES[name];
		}

		public static bool IsAnyCollectionProperty(string name)
		{
			return HQL_COLLECTION_PROPERTIES.ContainsKey(name);
		}
	}
}
