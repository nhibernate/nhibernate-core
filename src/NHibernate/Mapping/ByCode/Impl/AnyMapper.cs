using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Type;

namespace NHibernate.Mapping.ByCode.Impl
{
	public class AnyMapper : IAnyMapper
	{
		private const string DefaultIdColumnNameWhenNoProperty = "ReferencedId";
		private const string DefaultMetaColumnNameWhenNoProperty = "ReferencedClass";
		private readonly HbmAny any;
		private readonly ColumnMapper classColumnMapper;
		private readonly IAccessorPropertyMapper entityPropertyMapper;
		private readonly System.Type foreignIdType;
		private readonly ColumnMapper idColumnMapper;
		private readonly HbmMapping mapDoc;
		private readonly MemberInfo member;

		public AnyMapper(MemberInfo member, System.Type foreignIdType, HbmAny any, HbmMapping mapDoc)
			: this(member, foreignIdType, member == null ? (IAccessorPropertyMapper)new NoMemberPropertyMapper() : new AccessorPropertyMapper(member.DeclaringType, member.Name, x => any.access = x), any, mapDoc) { }

		public AnyMapper(MemberInfo member, System.Type foreignIdType, IAccessorPropertyMapper accessorMapper, HbmAny any, HbmMapping mapDoc)
		{
			this.member = member;
			this.foreignIdType = foreignIdType;
			this.any = any;
			this.mapDoc = mapDoc;
			if (member == null)
			{
				this.any.access = "none";
			}
			entityPropertyMapper = member == null ? new NoMemberPropertyMapper() : accessorMapper;
			if (foreignIdType == null)
			{
				throw new ArgumentNullException("foreignIdType");
			}
			if (any == null)
			{
				throw new ArgumentNullException("any");
			}

			this.any.idtype = this.foreignIdType.GetNhTypeName();
			var idHbmColumn = new HbmColumn();
			string idColumnName = member == null ? DefaultIdColumnNameWhenNoProperty : member.Name + "Id";
			idColumnMapper = new ColumnMapper(idHbmColumn, idColumnName);
			var classHbmColumn = new HbmColumn();
			string classColumnName = member == null ? DefaultMetaColumnNameWhenNoProperty : member.Name + "Class";
			classColumnMapper = new ColumnMapper(classHbmColumn, classColumnName);
			any.column = new[] { classHbmColumn, idHbmColumn };
		}

		#region Implementation of IAccessorPropertyMapper

		public void Access(Accessor accessor)
		{
			entityPropertyMapper.Access(accessor);
		}

		public void Access(System.Type accessorType)
		{
			entityPropertyMapper.Access(accessorType);
		}

		public void OptimisticLock(bool takeInConsiderationForOptimisticLock)
		{
			any.optimisticlock = takeInConsiderationForOptimisticLock;
		}

		#endregion

		#region Implementation of IAnyMapper

		public void MetaType(IType metaType)
		{
			if (metaType != null)
			{
				CheckMetaTypeImmutability(metaType.Name);
				any.metatype = metaType.Name;
			}
		}

		public void MetaType<TMetaType>()
		{
			MetaType(typeof (TMetaType));
		}

		public void MetaType(System.Type metaType)
		{
			if (metaType != null)
			{
				string nhTypeName = metaType.GetNhTypeName();
				CheckMetaTypeImmutability(nhTypeName);
				any.metatype = nhTypeName;
			}
		}

		public void IdType(IType idType)
		{
			if (idType != null)
			{
				CheckIdTypeImmutability(idType.Name);
				any.idtype = idType.Name;
			}
		}

		public void IdType<TIdType>()
		{
			IdType(typeof (TIdType));
		}

		public void IdType(System.Type idType)
		{
			if (idType != null)
			{
				string nhTypeName = idType.GetNhTypeName();
				CheckIdTypeImmutability(nhTypeName);
				any.idtype = nhTypeName;
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
			System.Type metavalueType = value.GetType();
			if (any.metavalue == null)
			{
				any.metavalue = new HbmMetaValue[0];
			}
			Dictionary<string, string> values = any.metavalue.ToDictionary(mv => mv.value, mv => mv.@class);
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
			any.metavalue = values.Select(vd => new HbmMetaValue {value = vd.Key, @class = vd.Value}).ToArray();
		}

		public void Cascade(Cascade cascadeStyle)
		{
			any.cascade = (cascadeStyle.Exclude(ByCode.Cascade.DeleteOrphans)).ToCascadeString();
		}

		public void Index(string indexName)
		{
			any.index = indexName;
		}

		public void Lazy(bool isLazy)
		{
			any.lazy = isLazy;
		}

		public void Update(bool consideredInUpdateQuery)
		{
			any.update = consideredInUpdateQuery;
		}

		public void Insert(bool consideredInInsertQuery)
		{
			any.insert = consideredInInsertQuery;
		}

		private void CheckMetaTypeImmutability(string nhTypeName)
		{
			if (any.metavalue != null && any.metavalue.Length > 0 && any.metatype != nhTypeName)
			{
				throw new ArgumentException(string.Format("Can't change the meta-type (was '{0}' trying to change to '{1}')", any.metatype, nhTypeName));
			}
		}

		private void CheckIdTypeImmutability(string nhTypeName)
		{
			if (any.metavalue != null && any.metavalue.Length > 0 && any.idtype != nhTypeName)
			{
				throw new ArgumentException(string.Format("Can't change the id-type after add meta-values (was '{0}' trying to change to '{1}')", any.idtype, nhTypeName));
			}
		}

		#endregion
	}
}