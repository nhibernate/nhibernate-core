using System;
using NHibernate.Cfg.MappingSchema;

namespace NHibernate.Mapping.ByCode.Impl
{
	public class MapKeyRelation : IMapKeyRelation
	{
		private readonly System.Type dictionaryKeyType;
		private readonly HbmMapping mapDoc;
		private readonly HbmMap mapMapping;
		private ComponentMapKeyMapper componentMapKeyMapper;
		private MapKeyManyToManyMapper mapKeyManyToManyMapper;
		private MapKeyMapper mapKeyMapper;

		public MapKeyRelation(System.Type dictionaryKeyType, HbmMap mapMapping, HbmMapping mapDoc)
		{
			if (dictionaryKeyType == null)
			{
				throw new ArgumentNullException("dictionaryKeyType");
			}
			if (mapMapping == null)
			{
				throw new ArgumentNullException("mapMapping");
			}
			if (mapDoc == null)
			{
				throw new ArgumentNullException("mapDoc");
			}
			this.dictionaryKeyType = dictionaryKeyType;
			this.mapMapping = mapMapping;
			this.mapDoc = mapDoc;
		}

		#region IMapKeyRelation Members

		public void Element(Action<IMapKeyMapper> mapping)
		{
			if (mapKeyMapper == null)
			{
				var hbm = new HbmMapKey {type = dictionaryKeyType.GetNhTypeName()};
				mapKeyMapper = new MapKeyMapper(hbm);
			}
			mapping(mapKeyMapper);
			mapMapping.Item = mapKeyMapper.MapKeyMapping;
		}

		public void ManyToMany(Action<IMapKeyManyToManyMapper> mapping)
		{
			if (mapKeyManyToManyMapper == null)
			{
				var hbm = new HbmMapKeyManyToMany {@class = dictionaryKeyType.GetShortClassName(mapDoc)};
				mapKeyManyToManyMapper = new MapKeyManyToManyMapper(hbm);
			}
			mapping(mapKeyManyToManyMapper);
			mapMapping.Item = mapKeyManyToManyMapper.MapKeyManyToManyMapping;
		}

		public void Component(Action<IComponentMapKeyMapper> mapping)
		{
			if (componentMapKeyMapper == null)
			{
				var hbm = new HbmCompositeMapKey();
				componentMapKeyMapper = new ComponentMapKeyMapper(dictionaryKeyType, hbm, mapDoc);
			}
			mapping(componentMapKeyMapper);
			mapMapping.Item = componentMapKeyMapper.CompositeMapKeyMapping;
		}

		#endregion
	}
}