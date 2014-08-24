using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate.Cfg.MappingSchema;

namespace NHibernate.Mapping.ByCode.Impl
{
	public class KeyManyToOneMapper : IManyToOneMapper
	{
		private readonly IAccessorPropertyMapper _entityPropertyMapper;
		private readonly HbmKeyManyToOne _manyToOne;
		private readonly HbmMapping _mapDoc;
		private readonly MemberInfo _member;

		public KeyManyToOneMapper(MemberInfo member, HbmKeyManyToOne manyToOne, HbmMapping mapDoc)
		{
			_member = member;
			_manyToOne = manyToOne;
			_mapDoc = mapDoc;

			if (member == null)
			{
				_manyToOne.access = "none";
			}

			if (member == null)
			{
				_entityPropertyMapper = new NoMemberPropertyMapper();
			}
			else
			{
				_entityPropertyMapper = new AccessorPropertyMapper(member.DeclaringType, member.Name, x => manyToOne.access = x);
			}
		}

		#region Implementation of IManyToOneMapper

		public void Class(System.Type entityType)
		{
			if (!_member.GetPropertyOrFieldType().IsAssignableFrom(entityType))
			{
				throw new ArgumentOutOfRangeException("entityType",
					String.Format("The type is incompatible; expected assignable to {0}", _member.GetPropertyOrFieldType()));
			}
			_manyToOne.@class = entityType.GetShortClassName(_mapDoc);
		}

		public void Cascade(Cascade cascadeStyle)
		{
			// not supported by HbmKeyManyToOne
		}

		public void NotNullable(bool notnull)
		{
			Column(x => x.NotNullable(notnull));
		}

		public void Unique(bool unique)
		{
			Column(x => x.Unique(unique));
		}

		public void UniqueKey(string uniquekeyName)
		{
			Column(x => x.UniqueKey(uniquekeyName));
		}

		public void Index(string indexName)
		{
			Column(x => x.Index(indexName));
		}

		public void Fetch(FetchKind fetchMode)
		{
			// not supported by HbmKeyManyToOne
		}

		public void Formula(string formula)
		{
			// not supported by HbmKeyManyToOne
		}

		public void Lazy(LazyRelation lazyRelation)
		{
			switch (lazyRelation.ToHbm())
			{
				case HbmLaziness.False:
					_manyToOne.lazy = HbmRestrictedLaziness.False;
					_manyToOne.lazySpecified = true;
					break;
				case HbmLaziness.Proxy:
					_manyToOne.lazy = HbmRestrictedLaziness.Proxy;
					_manyToOne.lazySpecified = true;
					break;
				case HbmLaziness.NoProxy:
					_manyToOne.lazy = HbmRestrictedLaziness.False;
					_manyToOne.lazySpecified = true;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public void Update(bool consideredInUpdateQuery)
		{
			// not supported by HbmKeyManyToOne
		}

		public void Insert(bool consideredInInsertQuery)
		{
			// not supported by HbmKeyManyToOne
		}

		public void ForeignKey(string foreignKeyName)
		{
			_manyToOne.foreignkey = foreignKeyName;
		}

		public void PropertyRef(string propertyReferencedName)
		{
			//Not supported
		}

		public void NotFound(NotFoundMode mode)
		{
			_manyToOne.notfound = mode.ToHbm();
		}

		#endregion

		#region Implementation of IAccessorPropertyMapper

		public void Access(Accessor accessor)
		{
			_entityPropertyMapper.Access(accessor);
		}

		public void Access(System.Type accessorType)
		{
			_entityPropertyMapper.Access(accessorType);
		}

		public void OptimisticLock(bool takeInConsiderationForOptimisticLock)
		{
			// not supported by HbmKeyManyToOne
		}

		#endregion

		#region Implementation of IColumnsMapper

		public void Column(Action<IColumnMapper> columnMapper)
		{
			if (_manyToOne.Columns.Count() > 1)
			{
				throw new MappingException("Multi-columns property can't be mapped through single-column API.");
			}
			HbmColumn hbm = _manyToOne.Columns.SingleOrDefault();
			hbm = hbm
				  ??
				  new HbmColumn
				  {
					  name = _manyToOne.column1,
				  };
			string defaultColumnName = _member.Name;
			columnMapper(new ColumnMapper(hbm, _member != null ? defaultColumnName : "unnamedcolumn"));
			if (ColumnTagIsRequired(hbm))
			{
				_manyToOne.column = new[] { hbm };
				ResetColumnPlainValues();
			}
			else
			{
				_manyToOne.column1 = defaultColumnName == null || !defaultColumnName.Equals(hbm.name) ? hbm.name : null;
			}
		}

		public void Columns(params Action<IColumnMapper>[] columnMapper)
		{
			ResetColumnPlainValues();
			int i = 1;
			var columns = new List<HbmColumn>(columnMapper.Length);
			foreach (var action in columnMapper)
			{
				var hbm = new HbmColumn();
				string defaultColumnName = (_member != null ? _member.Name : "unnamedcolumn") + i++;
				action(new ColumnMapper(hbm, defaultColumnName));
				columns.Add(hbm);
			}
			_manyToOne.column = columns.ToArray();
		}

		public void Column(string name)
		{
			Column(x => x.Name(name));
		}

		private bool ColumnTagIsRequired(HbmColumn hbm)
		{
			return hbm.length != null || hbm.precision != null || hbm.scale != null || hbm.notnull || hbm.unique
				   || hbm.uniquekey != null || hbm.sqltype != null || hbm.index != null || hbm.@default != null
				   || hbm.check != null;
		}

		private void ResetColumnPlainValues()
		{
			_manyToOne.column1 = null;
		}

		#endregion
	}
}