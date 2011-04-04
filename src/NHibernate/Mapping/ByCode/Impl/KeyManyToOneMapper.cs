using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate.Cfg.MappingSchema;

namespace NHibernate.Mapping.ByCode.Impl
{
	public class KeyManyToOneMapper : IManyToOneMapper
	{
		private readonly IAccessorPropertyMapper entityPropertyMapper;
		private readonly HbmKeyManyToOne manyToOne;
		private readonly HbmMapping mapDoc;
		private readonly MemberInfo member;

		public KeyManyToOneMapper(MemberInfo member, HbmKeyManyToOne manyToOne, HbmMapping mapDoc)
		{
			this.member = member;
			this.manyToOne = manyToOne;
			this.mapDoc = mapDoc;
			if (member == null)
			{
				this.manyToOne.access = "none";
			}
			if (member == null)
			{
				entityPropertyMapper = new NoMemberPropertyMapper();
			}
			else
			{
				entityPropertyMapper = new AccessorPropertyMapper(member.DeclaringType, member.Name, x => manyToOne.access = x);
			}
		}

		#region Implementation of IManyToOneMapper

		public void Class(System.Type entityType)
		{
			if (!member.GetPropertyOrFieldType().IsAssignableFrom(entityType))
			{
				throw new ArgumentOutOfRangeException("entityType",
				                                      string.Format("The type is incompatible; expected assignable to {0}",
				                                                    member.GetPropertyOrFieldType()));
			}
			manyToOne.@class = entityType.GetShortClassName(mapDoc);
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
					manyToOne.lazy = HbmRestrictedLaziness.False;
					manyToOne.lazySpecified = true;
					break;
				case HbmLaziness.Proxy:
					manyToOne.lazy = HbmRestrictedLaziness.Proxy;
					manyToOne.lazySpecified = true;
					break;
				case HbmLaziness.NoProxy:
					manyToOne.lazy = HbmRestrictedLaziness.False;
					manyToOne.lazySpecified = true;
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
			manyToOne.foreignkey = foreignKeyName;
		}

		#endregion

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
			// not supported by HbmKeyManyToOne
		}

		#endregion

		#region Implementation of IColumnsMapper

		public void Column(Action<IColumnMapper> columnMapper)
		{
			if (manyToOne.Columns.Count() > 1)
			{
				throw new MappingException("Multi-columns property can't be mapped through single-column API.");
			}
			HbmColumn hbm = manyToOne.Columns.SingleOrDefault();
			hbm = hbm
			      ??
			      new HbmColumn
			      {
			      	name = manyToOne.column1,
			      };
			string defaultColumnName = member.Name;
			columnMapper(new ColumnMapper(hbm, member != null ? defaultColumnName : "unnamedcolumn"));
			if (ColumnTagIsRequired(hbm))
			{
				manyToOne.column = new[] {hbm};
				ResetColumnPlainValues();
			}
			else
			{
				manyToOne.column1 = defaultColumnName == null || !defaultColumnName.Equals(hbm.name) ? hbm.name : null;
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
				string defaultColumnName = (member != null ? member.Name : "unnamedcolumn") + i++;
				action(new ColumnMapper(hbm, defaultColumnName));
				columns.Add(hbm);
			}
			manyToOne.column = columns.ToArray();
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
			manyToOne.column1 = null;
		}

		#endregion
	}
}