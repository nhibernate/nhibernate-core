using System.Collections.Generic;

namespace NHibernate.Cfg.MappingSchema
{
	public interface ICollectionPropertiesMapping : IEntityPropertyMapping, IReferencePropertyMapping, ICollectionSqlsMapping
	{
		bool Inverse { get; }
		bool Mutable { get; }
		string OrderBy { get; }
		string Where { get; }
		int? BatchSize { get; }
		string PersisterQualifiedName { get; }
		string CollectionType { get; }
		HbmCollectionFetchMode? FetchMode { get; }
		HbmOuterJoinStrategy? OuterJoin { get; }
		HbmCollectionLazy? Lazy { get; }
		string Table { get; }
		string Schema { get; }
		string Catalog { get; }
		string Check { get; }

		/// <summary>
		/// The relation of the element of the collection.
		/// </summary>
		/// <remarks>
		/// Can be one of: HbmCompositeElement, HbmElement, HbmManyToAny, HbmManyToMany, HbmOneToMany...
		/// according to the type of the collection.
		/// </remarks>
		object ElementRelationship { get; }
		string Sort { get; }
		bool? Generic { get; }
		IEnumerable<HbmFilter> Filters { get; }
		HbmKey Key { get; }
		HbmCache Cache { get; }
	}
}