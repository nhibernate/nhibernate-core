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

		public static Dictionary<string, string> HQL_COLLECTION_PROPERTIES;

		private static readonly string COLLECTION_INDEX_LOWER = CollectionPropertyNames.Index.ToLowerInvariant();

		static CollectionProperties()
		{
			HQL_COLLECTION_PROPERTIES = new Dictionary<string, string>();
			HQL_COLLECTION_PROPERTIES.Add(CollectionPropertyNames.Elements.ToLowerInvariant(), CollectionPropertyNames.Elements);
			HQL_COLLECTION_PROPERTIES.Add(CollectionPropertyNames.Indices.ToLowerInvariant(), CollectionPropertyNames.Indices);
			HQL_COLLECTION_PROPERTIES.Add(CollectionPropertyNames.Size.ToLowerInvariant(), CollectionPropertyNames.Size);
			HQL_COLLECTION_PROPERTIES.Add(CollectionPropertyNames.MaxIndex.ToLowerInvariant(), CollectionPropertyNames.MaxIndex);
			HQL_COLLECTION_PROPERTIES.Add(CollectionPropertyNames.MinIndex.ToLowerInvariant(), CollectionPropertyNames.MinIndex);
			HQL_COLLECTION_PROPERTIES.Add(CollectionPropertyNames.MaxElement.ToLowerInvariant(), CollectionPropertyNames.MaxElement);
			HQL_COLLECTION_PROPERTIES.Add(CollectionPropertyNames.MinElement.ToLowerInvariant(), CollectionPropertyNames.MinElement);
			HQL_COLLECTION_PROPERTIES.Add(COLLECTION_INDEX_LOWER, CollectionPropertyNames.Index);
		}

		public static bool IsCollectionProperty(string name)
		{
			string key = name.ToLowerInvariant();
			// CollectionPropertyMapping processes everything except 'index'.
			if (COLLECTION_INDEX_LOWER == key)
			{
				return false;
			}
			else
			{
				return HQL_COLLECTION_PROPERTIES.ContainsKey(key);
			}
		}

		public static string GetNormalizedPropertyName(string name)
		{
			return HQL_COLLECTION_PROPERTIES[name];
		}

		public static bool IsAnyCollectionProperty(string name)
		{
			string key = name.ToLowerInvariant();
			return HQL_COLLECTION_PROPERTIES.ContainsKey(key);
		}
	}
}
