using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Util;

namespace NHibernate.Mapping.ByCode.Impl
{
	// 6.0 TODO: remove IColumnsAndFormulasMapper once IManyToOneMapper inherits it.
	public class ManyToOneMapper : IManyToOneMapper, IColumnsAndFormulasMapper
	{
		private readonly IAccessorPropertyMapper _entityPropertyMapper;
		private readonly HbmManyToOne _manyToOne;
		private readonly HbmMapping _mapDoc;
		private readonly MemberInfo _member;

		public ManyToOneMapper(MemberInfo member, HbmManyToOne manyToOne, HbmMapping mapDoc)
			: this(member, member == null ? (IAccessorPropertyMapper) new NoMemberPropertyMapper() : new AccessorPropertyMapper(member.DeclaringType, member.Name, x => manyToOne.access = x), manyToOne, mapDoc) { }

		public ManyToOneMapper(MemberInfo member, IAccessorPropertyMapper accessorPropertyMapper, HbmManyToOne manyToOne, HbmMapping mapDoc)
		{
			_member = member;
			_manyToOne = manyToOne;
			_mapDoc = mapDoc;

			if (member == null)
			{
				_manyToOne.access = "none";
			}

			_entityPropertyMapper = member == null ? new NoMemberPropertyMapper() : accessorPropertyMapper;
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

		public void EntityName(string entityName)
		{
			_manyToOne.entityname = entityName;
		}

		public void Cascade(Cascade cascadeStyle)
		{
			_manyToOne.cascade = cascadeStyle.ToCascadeString();
		}

		public void NotNullable(bool notnull)
		{
			Column(x => x.NotNullable(notnull));
		}

		public void Unique(bool unique)
		{
			_manyToOne.unique = unique;
		}

		public void UniqueKey(string uniquekeyName)
		{
			_manyToOne.uniquekey = uniquekeyName;
		}

		public void Index(string indexName)
		{
			_manyToOne.index = indexName;
		}

		public void Fetch(FetchKind fetchMode)
		{
			_manyToOne.fetch = fetchMode.ToHbm();
			_manyToOne.fetchSpecified = _manyToOne.fetch == HbmFetchMode.Join;
		}

		public void Lazy(LazyRelation lazyRelation)
		{
			_manyToOne.lazy = lazyRelation.ToHbm();
			_manyToOne.lazySpecified = _manyToOne.lazy != HbmLaziness.Proxy;
		}

		public void Update(bool consideredInUpdateQuery)
		{
			_manyToOne.update = consideredInUpdateQuery;
		}

		public void Insert(bool consideredInInsertQuery)
		{
			_manyToOne.insert = consideredInInsertQuery;
		}

		public void ForeignKey(string foreignKeyName)
		{
			_manyToOne.foreignkey = foreignKeyName;
		}

		public void PropertyRef(string propertyReferencedName)
		{
			_manyToOne.propertyref = propertyReferencedName;
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
			_manyToOne.optimisticlock = takeInConsiderationForOptimisticLock;
		}

		#endregion

		#region Implementation of IColumnsAndFormulasMapper

		/// <inheritdoc />
		public void ColumnsAndFormulas(params Action<IColumnOrFormulaMapper>[] columnOrFormulaMapper)
		{
			ResetColumnPlainValues();

			_manyToOne.Items = ColumnOrFormulaMapper.GetItemsFor(
				columnOrFormulaMapper,
				_member != null ? _member.Name : "unnamedcolumn");
		}

		/// <inheritdoc cref="IColumnsAndFormulasMapper.Formula" />
		public void Formula(string formula)
		{
			if (formula == null)
			{
				return;
			}

			ResetColumnPlainValues();
			_manyToOne.Items = null;
			string[] formulaLines = formula.Split(StringHelper.LineSeparators, StringSplitOptions.None);
			if (formulaLines.Length > 1)
			{
				_manyToOne.Items = new object[] { new HbmFormula { Text = formulaLines } };
			}
			else
			{
				_manyToOne.formula = formula;
			}
		}

		/// <inheritdoc />
		public void Formulas(params string[] formulas)
		{
			if (formulas == null)
				throw new ArgumentNullException(nameof(formulas));

			ResetColumnPlainValues();
			_manyToOne.Items =
				formulas
					.ToArray(
						f => (object) new HbmFormula { Text = f.Split(StringHelper.LineSeparators, StringSplitOptions.None) });
		}

		#endregion

		#region Implementation of IColumnsMapper

		public void Column(Action<IColumnMapper> columnMapper)
		{
			if (_manyToOne.Columns.Count() > 1)
			{
				throw new MappingException("Multi-columns property can't be mapped through single-column API.");
			}
			_manyToOne.formula = null;
			HbmColumn hbm = _manyToOne.Columns.SingleOrDefault();
			hbm = hbm
				  ??
				  new HbmColumn
				  {
					  name = _manyToOne.column,
					  notnull = _manyToOne.notnull,
					  notnullSpecified = _manyToOne.notnullSpecified,
					  unique = _manyToOne.unique,
					  uniqueSpecified = _manyToOne.unique,
					  uniquekey = _manyToOne.uniquekey,
					  index = _manyToOne.index
				  };
			string defaultColumnName = _member.Name;
			columnMapper(new ColumnMapper(hbm, _member != null ? defaultColumnName : "unnamedcolumn"));
			if (hbm.sqltype != null || hbm.@default != null || hbm.check != null)
			{
				_manyToOne.Items = new[] { hbm };
				ResetColumnPlainValues();
			}
			else
			{
				_manyToOne.column = defaultColumnName == null || !defaultColumnName.Equals(hbm.name) ? hbm.name : null;
				_manyToOne.notnull = hbm.notnull;
				_manyToOne.notnullSpecified = hbm.notnullSpecified;
				_manyToOne.unique = hbm.unique;
				_manyToOne.uniquekey = hbm.uniquekey;
				_manyToOne.index = hbm.index;
			}
		}

		public void Columns(params Action<IColumnMapper>[] columnMapper)
		{
			ResetColumnPlainValues();

			var columns = new HbmColumn[columnMapper.Length];
			for (var i = 0; i < columnMapper.Length; i++)
			{
				var hbm = new HbmColumn();
				string defaultColumnName = (_member != null ? _member.Name : "unnamedcolumn") + i + 1;
				columnMapper[i](new ColumnMapper(hbm, defaultColumnName));
				columns[i] = hbm;
			}
			_manyToOne.Items = columns;
		}

		public void Column(string name)
		{
			Column(x => x.Name(name));
		}

		private void ResetColumnPlainValues()
		{
			_manyToOne.column = null;
			_manyToOne.notnull = false;
			_manyToOne.notnullSpecified = false;
			_manyToOne.unique = false;
			_manyToOne.uniquekey = null;
			_manyToOne.index = null;
			_manyToOne.formula = null;
		}

		#endregion
	}
}
