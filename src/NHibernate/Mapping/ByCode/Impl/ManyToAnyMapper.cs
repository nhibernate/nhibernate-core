using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Type;

namespace NHibernate.Mapping.ByCode.Impl
{
	public class ManyToAnyMapper: IManyToAnyMapper
	{
		private const string DefaultIdColumnNameWhenNoProperty = "ReferencedId";
		private const string DefaultMetaColumnNameWhenNoProperty = "ReferencedClass";
		private readonly System.Type elementType;
		private readonly System.Type foreignIdType;
		private readonly ColumnMapper classColumnMapper;
		private readonly ColumnMapper idColumnMapper;

		private readonly HbmManyToAny manyToAny;
		private readonly HbmMapping mapDoc;

		public ManyToAnyMapper(System.Type elementType, System.Type foreignIdType, HbmManyToAny manyToAny, HbmMapping mapDoc)
		{
			if (elementType == null)
			{
				throw new ArgumentNullException("elementType");
			}
			if (foreignIdType == null)
			{
				throw new ArgumentNullException("foreignIdType");
			}
			if (manyToAny == null)
			{
				throw new ArgumentNullException("manyToAny");
			}
			this.elementType = elementType;
			this.foreignIdType = foreignIdType;
			this.manyToAny = manyToAny;
			this.mapDoc = mapDoc;
			this.manyToAny.idtype = this.foreignIdType.GetNhTypeName();
			var idHbmColumn = new HbmColumn();
			idColumnMapper = new ColumnMapper(idHbmColumn, DefaultIdColumnNameWhenNoProperty);
			var classHbmColumn = new HbmColumn();
			classColumnMapper = new ColumnMapper(classHbmColumn, DefaultMetaColumnNameWhenNoProperty);
			manyToAny.column = new[] { classHbmColumn, idHbmColumn };
		}

		public void MetaType(IType metaType)
		{
			if (metaType != null)
			{
				CheckMetaTypeImmutability(metaType.Name);
				manyToAny.metatype = metaType.Name;
			}
		}

		public void MetaType<TMetaType>()
		{
			MetaType(typeof(TMetaType));
		}

		public void MetaType(System.Type metaType)
		{
			if (metaType != null)
			{
				string nhTypeName = metaType.GetNhTypeName();
				CheckMetaTypeImmutability(nhTypeName);
				manyToAny.metatype = nhTypeName;
			}
		}

		public void IdType(IType idType)
		{
			if (idType != null)
			{
				CheckIdTypeImmutability(idType.Name);
				manyToAny.idtype = idType.Name;
			}
		}

		public void IdType<TIdType>()
		{
			IdType(typeof(TIdType));
		}

		public void IdType(System.Type idType)
		{
			if (idType != null)
			{
				string nhTypeName = idType.GetNhTypeName();
				CheckIdTypeImmutability(nhTypeName);
				manyToAny.idtype = nhTypeName;
			}
		}

		public void Columns(Action<IColumnMapper> idColumnMapping, Action<IColumnMapper> classColumnMapping)
		{
			idColumnMapping(idColumnMapper);
			classColumnMapping(classColumnMapper);
		}

		public void MetaValue(object value, System.Type entityType)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (entityType == null)
			{
				throw new ArgumentNullException("entityType");
			}
			if (value is System.Type)
			{
				throw new ArgumentOutOfRangeException("value", "System.Type is invalid meta-type (you don't need to set meta-values).");
			}
			if(!elementType.IsAssignableFrom(entityType))
			{
				throw new ArgumentOutOfRangeException("entityType", string.Format("A {0} is not assignable to the collection's elements which type is {1}", entityType, elementType));
			}
			System.Type metavalueType = value.GetType();
			if (manyToAny.metavalue == null)
			{
				manyToAny.metavalue = new HbmMetaValue[0];
			}
			Dictionary<string, string> values = manyToAny.metavalue.ToDictionary(mv => mv.value, mv => mv.@class);
			MetaType(metavalueType);
			string newClassMetavalue = entityType.GetShortClassName(mapDoc);
			string metavalueKey = value.ToString();
			string existingClassMetavalue;
			if (values.TryGetValue(metavalueKey, out existingClassMetavalue) && existingClassMetavalue != newClassMetavalue)
			{
				throw new ArgumentException(
					string.Format(
						"Can't set two different classes for same meta-value (meta-value='{0}' old-class:'{1}' new-class='{2}')",
						metavalueKey, existingClassMetavalue, newClassMetavalue));
			}
			values[metavalueKey] = newClassMetavalue;
			manyToAny.metavalue = values.Select(vd => new HbmMetaValue { value = vd.Key, @class = vd.Value }).ToArray();
		}

		private void CheckMetaTypeImmutability(string nhTypeName)
		{
			if (manyToAny.metavalue != null && manyToAny.metavalue.Length > 0 && manyToAny.metatype != nhTypeName)
			{
				throw new ArgumentException(string.Format("Can't change the meta-type (was '{0}' trying to change to '{1}')", manyToAny.metatype, nhTypeName));
			}
		}

		private void CheckIdTypeImmutability(string nhTypeName)
		{
			if (manyToAny.metavalue != null && manyToAny.metavalue.Length > 0 && manyToAny.idtype != nhTypeName)
			{
				throw new ArgumentException(string.Format("Can't change the id-type after add meta-values (was '{0}' trying to change to '{1}')", manyToAny.idtype, nhTypeName));
			}
		}
	}
}