using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace NHibernate.Cfg.MappingSchema
{
	public partial class HbmUnionSubclass : AbstractDecoratable, IEntityMapping
	{
		[XmlIgnore]
		public IEnumerable<HbmUnionSubclass> UnionSubclasses
		{
			get { return unionsubclass1 ?? new HbmUnionSubclass[0]; }
		}

		#region Overrides of AbstractDecoratable

		protected override HbmMeta[] Metadatas
		{
			get { return meta ?? new HbmMeta[0]; }
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
			get { return tuplizer ?? new HbmTuplizer[0]; }
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
			get { return synchronize ?? new HbmSynchronize[0]; }
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
			get { return Items != null ? Items.Cast<IEntityPropertyMapping>() : new IEntityPropertyMapping[0]; }
		}

		#endregion
	}
}