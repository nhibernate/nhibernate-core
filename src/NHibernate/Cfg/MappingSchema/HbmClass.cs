using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace NHibernate.Cfg.MappingSchema
{
	partial class HbmClass : AbstractDecoratable, IEntityMapping, IEntityDiscriminableMapping
	{
		public HbmId Id
		{
			get { return Item as HbmId; }
		}

		public HbmCompositeId CompositeId
		{
			get { return Item as HbmCompositeId; }
		}

		public HbmVersion Version
		{
			get { return Item1 as HbmVersion; }
		}

		public HbmTimestamp Timestamp
		{
			get { return Item1 as HbmTimestamp; }
		}

		[XmlIgnore]
		public IEnumerable<HbmJoin> Joins
		{
			get { return Items1 != null ? Items1.OfType<HbmJoin>() : Array.Empty<HbmJoin>(); }
		}

		[XmlIgnore]
		public IEnumerable<HbmSubclass> Subclasses
		{
			get { return Items1 != null ? Items1.OfType<HbmSubclass>() : Array.Empty<HbmSubclass>(); }
		}

		[XmlIgnore]
		public IEnumerable<HbmJoinedSubclass> JoinedSubclasses
		{
			get { return Items1 != null ? Items1.OfType<HbmJoinedSubclass>() : Array.Empty<HbmJoinedSubclass>(); }
		}

		[XmlIgnore]
		public IEnumerable<HbmUnionSubclass> UnionSubclasses
		{
			get { return Items1 != null ? Items1.OfType<HbmUnionSubclass>() : Array.Empty<HbmUnionSubclass>(); }
		}

		#region Implementation of IEntityMapping

		protected override HbmMeta[] Metadatas
		{
			get { return meta ?? Array.Empty<HbmMeta>(); }
		}

		public string EntityName
		{
			get { return entityname; }
		}

		public string Name
		{
			get { return name; }
		}

		public string Node
		{
			get { return node; }
		}

		public string Proxy
		{
			get { return proxy; }
		}

		public bool? UseLazy
		{
			get { return lazySpecified ? lazy : (bool?) null; }
		}

		public HbmTuplizer[] Tuplizers
		{
			get { return tuplizer ?? Array.Empty<HbmTuplizer>(); }
		}

		public bool DynamicUpdate
		{
			get { return dynamicupdate; }
		}

		public bool DynamicInsert
		{
			get { return dynamicinsert; }
		}

		public int? BatchSize
		{
			get { return batchsizeSpecified ? batchsize : (int?) null; }
		}

		public bool SelectBeforeUpdate
		{
			get { return selectbeforeupdate; }
		}

		public string Persister
		{
			get { return persister; }
		}

		public bool? IsAbstract
		{
			get { return abstractSpecified ? @abstract : (bool?) null; }
		}

		public HbmSynchronize[] Synchronize
		{
			get { return synchronize ?? Array.Empty<HbmSynchronize>(); }
		}

		#endregion

		#region Implementation of IEntityDiscriminableMapping

		public string DiscriminatorValue
		{
			get { return discriminatorvalue; }
		}

		#endregion

		#region Implementation of IEntitySqlsMapping

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

		public string Subselect
		{
			get
			{
				return !string.IsNullOrEmpty(subselect1) ? subselect1 : (subselect != null ? subselect.Text.JoinString() : null);
			}
		}

		#endregion

		#region Implementation of IPropertiesContainerMapping

		[XmlIgnore]
		public IEnumerable<IEntityPropertyMapping> Properties
		{
			get { return Items != null ? Items.Cast<IEntityPropertyMapping>() : Array.Empty<IEntityPropertyMapping>(); }
		}

		#endregion
	}
}
