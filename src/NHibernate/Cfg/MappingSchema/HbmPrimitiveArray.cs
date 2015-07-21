using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace NHibernate.Cfg.MappingSchema
{
	public partial class HbmPrimitiveArray : AbstractDecoratable, ICollectionPropertiesMapping, IIndexedCollectionMapping
	{
		#region Implementation of IEntityPropertyMapping

		public string Name
		{
			get { return name; }
		}

		public string Access
		{
			get { return access; }
		}

		public bool OptimisticLock
		{
			get { return optimisticlock; }
		}

		public bool IsLazyProperty
		{
			get { return false; }
		}

		#endregion

		#region Implementation of IReferencePropertyMapping

		public string Cascade
		{
			get { return "none"; }
		}

		#endregion

		#region Overrides of AbstractDecoratable

		protected override HbmMeta[] Metadatas
		{
			get { return meta ?? new HbmMeta[0]; }
		}

		#endregion

		#region Implementation of ICollectionSqlsMapping

		public HbmLoader SqlLoader
		{
			get { return loader; }
		}

		public HbmCustomSQL SqlInsert
		{
			get { return sqlinsert; }
		}

		public HbmCustomSQL SqlUpdate
		{
			get { return sqlupdate; }
		}

		public HbmCustomSQL SqlDelete
		{
			get { return sqldelete; }
		}

		public HbmCustomSQL SqlDeleteAll
		{
			get { return sqldeleteall; }
		}

		public string Subselect
		{
			get
			{
				return !string.IsNullOrEmpty(subselect1) ? subselect1 : (subselect != null ? subselect.Text.JoinString() : null);
			}
		}

		#endregion

		#region Implementation of ICollectionPropertyMapping

		public bool Inverse
		{
			get { return false; }
		}

		public bool Mutable
		{
			get { return mutable; }
		}

		public string OrderBy
		{
			get { return null; }
		}

		public string Where
		{
			get { return @where; }
		}

		public int? BatchSize
		{
			get { return null; } // TODO NH: change schema following the same schema of others collections
		}

		public string PersisterQualifiedName
		{
			get { return persister; }
		}

		public string CollectionType
		{
			get { return collectiontype; }
		}

		public HbmCollectionFetchMode? FetchMode
		{
			get { return fetchSpecified ? (HbmCollectionFetchMode?)fetch : null; }
		}

		public HbmOuterJoinStrategy? OuterJoin
		{
			get { return outerjoinSpecified ? (HbmOuterJoinStrategy?)outerjoin : null; }
		}

		public HbmCollectionLazy? Lazy
		{
			get { return null; }
		}

		public string Table
		{
			get { return table; }
		}

		public string Schema
		{
			get { return schema; }
		}

		public string Catalog
		{
			get { return catalog; }
		}

		public string Check
		{
			get { return check; }
		}

		public object ElementRelationship
		{
			get { return element; }
		}

		public string Sort
		{
			get { return null; }
		}

		public bool? Generic
		{
			get { return null; }
		}

		[XmlIgnore]
		public IEnumerable<HbmFilter> Filters
		{
			get { yield break; }
		}

		public HbmKey Key
		{
			get { return key; }
		}

		public HbmCache Cache
		{
			get { return cache; }
		}

		#endregion

		#region Implementation of IIndexedCollection

		public HbmListIndex ListIndex
		{
			get { return Item as HbmListIndex; }
		}

		public HbmIndex Index
		{
			get { return Item as HbmIndex; }
		}

		#endregion
	}
}