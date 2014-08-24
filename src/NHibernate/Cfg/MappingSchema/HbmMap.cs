using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace NHibernate.Cfg.MappingSchema
{
	public partial class HbmMap : AbstractDecoratable, ICollectionPropertiesMapping
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

		public bool IsLazyProperty
		{
			get { return false; }
		}

		public bool OptimisticLock
		{
			get { return optimisticlock; }
		}

		#endregion

		#region Implementation of IReferencePropertyMapping

		public string Cascade
		{
			get { return cascade; }
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

		#region Implementation of ICollectionPropertiesMapping

		public bool Inverse
		{
			get { return inverse; }
		}

		public bool Mutable
		{
			get { return mutable; }
		}

		public string OrderBy
		{
			get { return orderby; }
		}

		public string Where
		{
			get { return where; }
		}

		public int? BatchSize
		{
			get { return batchsizeSpecified ? batchsize : (int?)null; }
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
			get { return fetchSpecified ? fetch : (HbmCollectionFetchMode?)null; }
		}

		public HbmOuterJoinStrategy? OuterJoin
		{
			get { return outerjoinSpecified ? outerjoin : (HbmOuterJoinStrategy?)null; }
		}

		public HbmCollectionLazy? Lazy
		{
			get { return lazySpecified ? lazy : (HbmCollectionLazy?)null; }
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
			get { return Item1; }
		}

		public string Sort
		{
			get { return sort; }
		}

		public bool? Generic
		{
			get { return genericSpecified ? generic : (bool?)null; }
		}

		[XmlIgnore]
		public IEnumerable<HbmFilter> Filters
		{
			get { return filter ?? new HbmFilter[0]; }
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
	}
}