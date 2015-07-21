using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using NHibernate.Mapping;
using NHibernate.Util;

namespace NHibernate.Cfg.MappingSchema
{
	[Serializable]
	public abstract class AbstractDecoratable : IDecoratable
	{
		private static readonly IDictionary<string, MetaAttribute> EmptyMetaData = new CollectionHelper.EmptyMapClass<string, MetaAttribute>();

		[NonSerialized]
		[XmlIgnore]
		private IDictionary<string, MetaAttribute> mappedMetaData;

		[NonSerialized]
		[XmlIgnore]
		private IDictionary<string, MetaAttribute> inheritableMetaData;

		[XmlIgnore]
		public virtual IDictionary<string, MetaAttribute> MappedMetaData
		{
			get
			{
				if (mappedMetaData == null)
				{
					CreateMappedMetadata(Metadatas);
				}
				return mappedMetaData;
			}
		}

		[XmlIgnore]
		public IDictionary<string, MetaAttribute> InheritableMetaData
		{
			get
			{
				if (mappedMetaData == null)
				{
					CreateMappedMetadata(Metadatas);
				}
				return inheritableMetaData;
			}
		}

		protected void CreateMappedMetadata(HbmMeta[] metadatas)
		{
			if (metadatas == null)
			{
				mappedMetaData = EmptyMetaData;
				inheritableMetaData = EmptyMetaData;
				return;
			}
			mappedMetaData = new Dictionary<string, MetaAttribute>(10);
			inheritableMetaData = new Dictionary<string, MetaAttribute>(10);

			foreach (var hbmMeta in metadatas)
			{
				MetaAttribute attribute;
				if (!mappedMetaData.TryGetValue(hbmMeta.attribute, out attribute))
				{
					attribute = new MetaAttribute(hbmMeta.attribute);
					mappedMetaData[hbmMeta.attribute] = attribute;
					if(hbmMeta.inherit)
					{
						inheritableMetaData[hbmMeta.attribute] = attribute;
					}
				}
				if (hbmMeta.Text != null)
				{
					attribute.AddValue(string.Concat(hbmMeta.Text));
				}
			}
		}

		protected abstract HbmMeta[] Metadatas { get; }
	}
}