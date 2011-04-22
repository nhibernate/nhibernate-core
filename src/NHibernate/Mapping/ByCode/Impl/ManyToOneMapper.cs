using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate.Cfg.MappingSchema;

namespace NHibernate.Mapping.ByCode.Impl
{
	public class ManyToOneMapper : IManyToOneMapper
	{
		private readonly IAccessorPropertyMapper entityPropertyMapper;
		private readonly HbmManyToOne manyToOne;
		private readonly HbmMapping mapDoc;
		private readonly MemberInfo member;

		public ManyToOneMapper(MemberInfo member, HbmManyToOne manyToOne, HbmMapping mapDoc)
			: this(member, new AccessorPropertyMapper(member.DeclaringType, member.Name, x => manyToOne.access = x), manyToOne, mapDoc) {}

		public ManyToOneMapper(MemberInfo member, IAccessorPropertyMapper accessorPropertyMapper, HbmManyToOne manyToOne, HbmMapping mapDoc)
		{
			this.member = member;
			this.manyToOne = manyToOne;
			this.mapDoc = mapDoc;
			if (member == null)
			{
				this.manyToOne.access = "none";
			}
			entityPropertyMapper = member == null ? new NoMemberPropertyMapper() : accessorPropertyMapper;
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
			manyToOne.cascade = (cascadeStyle.Exclude(ByCode.Cascade.DeleteOrphans)).ToCascadeString();
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
			manyToOne.fetch = fetchMode.ToHbm();
			manyToOne.fetchSpecified = manyToOne.fetch == HbmFetchMode.Join;
		}

		public void Formula(string formula)
		{
			if (formula == null)
			{
				return;
			}

			ResetColumnPlainValues();
			manyToOne.Items = null;
			string[] formulaLines = formula.Split(new[] {Environment.NewLine}, StringSplitOptions.None);
			if (formulaLines.Length > 1)
			{
				manyToOne.Items = new[] {new HbmFormula {Text = formulaLines}};
			}
			else
			{
				manyToOne.formula = formula;
			}
		}

		public void Lazy(LazyRelation lazyRelation)
		{
			manyToOne.lazy = lazyRelation.ToHbm();
			manyToOne.lazySpecified = manyToOne.lazy != HbmLaziness.Proxy;
		}

		public void Update(bool consideredInUpdateQuery)
		{
			manyToOne.update = consideredInUpdateQuery;
		}

		public void Insert(bool consideredInInsertQuery)
		{
			manyToOne.insert = consideredInInsertQuery;
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
			manyToOne.optimisticlock = takeInConsiderationForOptimisticLock;
		}

		#endregion

		#region Implementation of IColumnsMapper

		public void Column(Action<IColumnMapper> columnMapper)
		{
			if (manyToOne.Columns.Count() > 1)
			{
				throw new MappingException("Multi-columns property can't be mapped through single-column API.");
			}
			manyToOne.formula = null;
			HbmColumn hbm = manyToOne.Columns.SingleOrDefault();
			hbm = hbm
			      ??
			      new HbmColumn
			      {
			      	name = manyToOne.column,
			      	notnull = manyToOne.notnull,
			      	notnullSpecified = manyToOne.notnullSpecified,
			      	unique = manyToOne.unique,
			      	uniqueSpecified = manyToOne.unique,
			      	uniquekey = manyToOne.uniquekey,
			      	index = manyToOne.index
			      };
			string defaultColumnName = member.Name;
			columnMapper(new ColumnMapper(hbm, member != null ? defaultColumnName : "unnamedcolumn"));
			if (hbm.sqltype != null || hbm.@default != null || hbm.check != null)
			{
				manyToOne.Items = new[] {hbm};
				ResetColumnPlainValues();
			}
			else
			{
				manyToOne.column = defaultColumnName == null || !defaultColumnName.Equals(hbm.name) ? hbm.name : null;
				manyToOne.notnull = hbm.notnull;
				manyToOne.notnullSpecified = hbm.notnullSpecified;
				manyToOne.unique = hbm.unique;
				manyToOne.uniquekey = hbm.uniquekey;
				manyToOne.index = hbm.index;
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
			manyToOne.Items = columns.ToArray();
		}

		public void Column(string name)
		{
			Column(x => x.Name(name));
		}

		private void ResetColumnPlainValues()
		{
			manyToOne.column = null;
			manyToOne.notnull = false;
			manyToOne.notnullSpecified = false;
			manyToOne.unique = false;
			manyToOne.uniquekey = null;
			manyToOne.index = null;
			manyToOne.formula = null;
		}

		#endregion
	}
}