using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Util;

namespace NHibernate.Mapping.ByCode.Impl
{
	// 6.0 TODO: remove IColumnsAndFormulasMapper once IManyToManyMapper inherits it.
	public class ManyToManyMapper : IManyToManyMapper, IColumnsAndFormulasMapper
	{
		private readonly System.Type elementType;
		private readonly HbmManyToMany manyToMany;
		private readonly HbmMapping mapDoc;

		public ManyToManyMapper(System.Type elementType, HbmManyToMany manyToMany, HbmMapping mapDoc)
		{
			if (elementType == null)
			{
				throw new ArgumentNullException("elementType");
			}
			if (manyToMany == null)
			{
				throw new ArgumentNullException("manyToMany");
			}
			this.elementType = elementType;
			this.manyToMany = manyToMany;
			this.mapDoc = mapDoc;
		}

		#region Implementation of IColumnsMapper

		public void Column(Action<IColumnMapper> columnMapper)
		{
			if (manyToMany.Columns.Count() > 1)
			{
				throw new MappingException("Multi-columns property can't be mapped through single-column API.");
			}
			manyToMany.formula = null;
			HbmColumn hbm = manyToMany.Columns.SingleOrDefault();
			hbm = hbm
			      ??
			      new HbmColumn
			      {
			      	name = manyToMany.column,
			      	unique = manyToMany.unique,
			      	uniqueSpecified = manyToMany.unique,
			      };
			columnMapper(new ColumnMapper(hbm, Collection.DefaultElementColumnName));
			if (ColumnTagIsRequired(hbm))
			{
				manyToMany.Items = new[] {hbm};
				ResetColumnPlainValues();
			}
			else
			{
				manyToMany.column = Collection.DefaultElementColumnName.Equals(hbm.name) ? null : hbm.name;
				manyToMany.unique = hbm.unique;
			}
		}

		public void Columns(params Action<IColumnMapper>[] columnMapper)
		{
			ResetColumnPlainValues();
			var columns = new HbmColumn[columnMapper.Length];
			for (var i = 0; i < columnMapper.Length; i++)
			{
				var hbm = new HbmColumn();
				string defaultColumnName = elementType.Name + i + 1;
				columnMapper[i](new ColumnMapper(hbm, defaultColumnName));
				columns[i] = hbm;
			}
			manyToMany.Items = columns;
		}

		public void Column(string name)
		{
			Column(x => x.Name(name));
		}

		private bool ColumnTagIsRequired(HbmColumn hbm)
		{
			return hbm.length != null || hbm.precision != null || hbm.scale != null || hbm.notnull || hbm.uniquekey != null
			       || hbm.sqltype != null || hbm.index != null || hbm.@default != null || hbm.check != null;
		}

		private void ResetColumnPlainValues()
		{
			manyToMany.column = null;
			manyToMany.unique = false;
			manyToMany.formula = null;
		}

		#endregion

		#region Implementation of IColumnsAndFormulasMapper

		/// <inheritdoc />
		public void ColumnsAndFormulas(params Action<IColumnOrFormulaMapper>[] columnOrFormulaMapper)
		{
			ResetColumnPlainValues();

			manyToMany.Items = ColumnOrFormulaMapper.GetItemsFor(columnOrFormulaMapper, elementType.Name);
		}

		/// <inheritdoc cref="IColumnsAndFormulasMapper.Formula" />
		public void Formula(string formula)
		{
			if (formula == null)
			{
				return;
			}

			ResetColumnPlainValues();
			manyToMany.Items = null;
			string[] formulaLines = formula.Split(StringHelper.LineSeparators, StringSplitOptions.None);
			if (formulaLines.Length > 1)
			{
				manyToMany.Items = new object[] {new HbmFormula {Text = formulaLines}};
			}
			else
			{
				manyToMany.formula = formula;
			}
		}

		/// <inheritdoc />
		public void Formulas(params string[] formulas)
		{
			if (formulas == null)
				throw new ArgumentNullException(nameof(formulas));

			ResetColumnPlainValues();
			manyToMany.Items =
				formulas
					.ToArray(
						f => (object) new HbmFormula {Text = f.Split(StringHelper.LineSeparators, StringSplitOptions.None)});
		}

		#endregion

		#region IManyToManyMapper Members

		public void Class(System.Type entityType)
		{
			if (!elementType.IsAssignableFrom(entityType))
			{
				throw new ArgumentOutOfRangeException("entityType",
				                                      string.Format("The type is incompatible; expected assignable to {0}",
				                                                    elementType));
			}
			manyToMany.@class = entityType.GetShortClassName(mapDoc);
		}

		public void EntityName(string entityName)
		{
			manyToMany.entityname = entityName;
		}

		public void NotFound(NotFoundMode mode)
		{
			if (mode == null)
			{
				return;
			}
			manyToMany.notfound = mode.ToHbm();
		}

		public void Lazy(LazyRelation lazyRelation)
		{
			switch (lazyRelation.ToHbm())
			{
				case HbmLaziness.False:
					manyToMany.lazy = HbmRestrictedLaziness.False;
					manyToMany.lazySpecified = true;
					break;
				case HbmLaziness.Proxy:
					manyToMany.lazy = HbmRestrictedLaziness.Proxy;
					manyToMany.lazySpecified = true;
					break;
				case HbmLaziness.NoProxy:
					manyToMany.lazy = HbmRestrictedLaziness.False;
					manyToMany.lazySpecified = true;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public void ForeignKey(string foreignKeyName)
		{
			manyToMany.foreignkey = foreignKeyName;
		}

		public void Where(string sqlWhereClause)
		{
			manyToMany.where = sqlWhereClause;
		}

		#endregion
	}
}
