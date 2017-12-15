using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace NHibernate.Cfg.MappingSchema
{
	public partial class HbmSubclass : AbstractDecoratable, IEntityMapping, IEntityDiscriminableMapping
	{
		[XmlIgnore]
		public IEnumerable<HbmJoin> Joins
		{
			get { return join ?? Array.Empty<HbmJoin>(); }
		}

		[XmlIgnore]
		public IEnumerable<HbmSubclass> Subclasses
		{
			get { return subclass1 ?? Array.Empty<HbmSubclass>(); }
		}

		#region Overrides of AbstractDecoratable

		protected override HbmMeta[] Metadatas
		{
			get { return meta ?? Array.Empty<HbmMeta>(); }
		}

		#endregion

		#region Implementation of IEntityMapping

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
			get { return lazySpecified ? lazy : (bool?)null; }
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
			get { return !string.IsNullOrEmpty(batchsize) ? int.Parse(batchsize) : (int?)null; }
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
			get { return abstractSpecified ? @abstract : (bool?)null; }
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
			get { return null; }
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