using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Type;
using NHibernate.UserTypes;
using NHibernate.Util;

namespace NHibernate.Mapping.ByCode.Impl
{
	// 6.0 TODO: remove IColumnsAndFormulasMapper once IPropertyMapper inherits it.
	public class PropertyMapper : IPropertyMapper, IColumnsAndFormulasMapper
	{
		private readonly IAccessorPropertyMapper entityPropertyMapper;
		private readonly MemberInfo member;
		private readonly HbmProperty propertyMapping;

		public PropertyMapper(MemberInfo member, HbmProperty propertyMapping, IAccessorPropertyMapper accessorMapper)
		{
			if (propertyMapping == null)
			{
				throw new ArgumentNullException("propertyMapping");
			}
			this.member = member;
			this.propertyMapping = propertyMapping;
			if (member == null)
			{
				this.propertyMapping.access = "none";
			}
			entityPropertyMapper = accessorMapper;
		}

		public PropertyMapper(MemberInfo member, HbmProperty propertyMapping)
		{
			if (propertyMapping == null)
			{
				throw new ArgumentNullException("propertyMapping");
			}
			this.member = member;
			this.propertyMapping = propertyMapping;
			if (member == null)
			{
				this.propertyMapping.access = "none";
			}
			if (member == null)
			{
				entityPropertyMapper = new NoMemberPropertyMapper();
			}
			else
			{
				entityPropertyMapper = new AccessorPropertyMapper(member.DeclaringType, member.Name, x => propertyMapping.access = x);
			}
		}

		#region Implementation of IEntityPropertyMapper

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
			propertyMapping.optimisticlock = takeInConsiderationForOptimisticLock;
		}

		#endregion

		#region Implementation of IPropertyMapper

		public void Type(IType persistentType)
		{
			if (persistentType != null)
			{
				propertyMapping.type1 = persistentType.Name;
				propertyMapping.type = null;
			}
		}

		public void Type<TPersistentType>()
		{
			Type(typeof (TPersistentType), null);
		}

		public void Type<TPersistentType>(object parameters)
		{
			Type(typeof (TPersistentType), parameters);
		}

		public void Type(System.Type persistentType, object parameters)
		{
			if (persistentType == null)
			{
				throw new ArgumentNullException("persistentType");
			}
			if (!typeof (IUserType).IsAssignableFrom(persistentType) && !typeof (IType).IsAssignableFrom(persistentType) && !typeof (ICompositeUserType).IsAssignableFrom(persistentType))
			{
				throw new ArgumentOutOfRangeException("persistentType", "Expected type implementing IUserType, ICompositeUserType or IType.");
			}
			if (parameters != null)
			{
				propertyMapping.type1 = null;
				var hbmType = new HbmType
				{
					name = persistentType.AssemblyQualifiedName,
					param = parameters.GetType().GetProperties().ToArray(
						pi =>
						{
							var pvalue = pi.GetValue(parameters, null);
							return new HbmParam {name = pi.Name, Text = new[] {ReferenceEquals(pvalue, null) ? "null" : pvalue.ToString()}};
						})
				};
				propertyMapping.type = hbmType;
			}
			else
			{
				propertyMapping.type1 = persistentType.AssemblyQualifiedName;
				propertyMapping.type = null;
			}
		}

		public void Column(Action<IColumnMapper> columnMapper)
		{
			if (propertyMapping.Columns.Count() > 1)
			{
				throw new MappingException("Multi-columns property can't be mapped through singlr-column API.");
			}
			propertyMapping.formula = null;
			HbmColumn hbm = propertyMapping.Columns.SingleOrDefault();
			hbm = hbm
			      ??
			      new HbmColumn
			      {
			      	name = propertyMapping.column,
			      	length = propertyMapping.length,
			      	scale = propertyMapping.scale,
			      	precision = propertyMapping.precision,
			      	notnull = propertyMapping.notnull,
			      	notnullSpecified = propertyMapping.notnullSpecified,
			      	unique = propertyMapping.unique,
			      	uniqueSpecified = propertyMapping.unique,
			      	uniquekey = propertyMapping.uniquekey,
			      	index = propertyMapping.index
			      };
			string defaultColumnName = member.Name;
			columnMapper(new ColumnMapper(hbm, member != null ? defaultColumnName : "unnamedcolumn"));
			if (hbm.sqltype != null || hbm.@default != null || hbm.check != null)
			{
				propertyMapping.Items = new[] {hbm};
				ResetColumnPlainValues();
			}
			else
			{
				propertyMapping.column = defaultColumnName == null || !defaultColumnName.Equals(hbm.name) ? hbm.name : null;
				propertyMapping.length = hbm.length;
				propertyMapping.precision = hbm.precision;
				propertyMapping.scale = hbm.scale;
				propertyMapping.notnull = hbm.notnull;
				propertyMapping.notnullSpecified = hbm.notnullSpecified;
				propertyMapping.unique = hbm.unique;
				propertyMapping.uniquekey = hbm.uniquekey;
				propertyMapping.index = hbm.index;
			}
		}

		public void Columns(params Action<IColumnMapper>[] columnMapper)
		{
			ResetColumnPlainValues();
			var columns = new HbmColumn[columnMapper.Length];
			for (var i = 0; i < columnMapper.Length; i++)
			{
				var hbm = new HbmColumn();
				string defaultColumnName = (member != null ? member.Name : "unnamedcolumn") + i + 1;
				columnMapper[i](new ColumnMapper(hbm, defaultColumnName));
				columns[i] = hbm;
			}
			propertyMapping.Items = columns;
		}

		public void Column(string name)
		{
			Column(x => x.Name(name));
		}

		public void Length(int length)
		{
			Column(x => x.Length(length));
		}

		public void Precision(short precision)
		{
			Column(x => x.Precision(precision));
		}

		public void Scale(short scale)
		{
			Column(x => x.Scale(scale));
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

		public void Update(bool consideredInUpdateQuery)
		{
			propertyMapping.update = consideredInUpdateQuery;
			propertyMapping.updateSpecified = !consideredInUpdateQuery;
		}

		public void Insert(bool consideredInInsertQuery)
		{
			propertyMapping.insert = consideredInInsertQuery;
			propertyMapping.insertSpecified = !consideredInInsertQuery;
		}

		public void Lazy(bool isLazy)
		{
			propertyMapping.lazy = isLazy;
		}

		public void FetchGroup(string name)
		{
			propertyMapping.lazygroup = name;
		}

		public void Generated(PropertyGeneration generation)
		{
			if (generation == null)
			{
				return;
			}
			propertyMapping.generated = generation.ToHbm();
		}

		private void ResetColumnPlainValues()
		{
			propertyMapping.column = null;
			propertyMapping.length = null;
			propertyMapping.precision = null;
			propertyMapping.scale = null;
			propertyMapping.notnull = propertyMapping.notnullSpecified = false;
			propertyMapping.unique = false;
			propertyMapping.uniquekey = null;
			propertyMapping.index = null;
			propertyMapping.formula = null;
		}

		#endregion

		#region Implementation of IColumnsAndFormulasMapper

		/// <inheritdoc />
		public void ColumnsAndFormulas(params Action<IColumnOrFormulaMapper>[] columnOrFormulaMapper)
		{
			ResetColumnPlainValues();

			propertyMapping.Items = ColumnOrFormulaMapper.GetItemsFor(
				columnOrFormulaMapper,
				member != null ? member.Name : "unnamedcolumn");
		}

		/// <inheritdoc cref="IColumnsAndFormulasMapper.Formula" />
		public void Formula(string formula)
		{
			if (formula == null)
			{
				return;
			}

			ResetColumnPlainValues();
			propertyMapping.Items = null;
			string[] formulaLines = formula.Split(StringHelper.LineSeparators, StringSplitOptions.None);
			if (formulaLines.Length > 1)
			{
				propertyMapping.Items = new object[] {new HbmFormula {Text = formulaLines}};
			}
			else
			{
				propertyMapping.formula = formula;
			}
		}

		/// <inheritdoc />
		public void Formulas(params string[] formulas)
		{
			if (formulas == null)
				throw new ArgumentNullException(nameof(formulas));

			ResetColumnPlainValues();
			propertyMapping.Items =
				formulas
					.ToArray(
						f => (object) new HbmFormula { Text = f.Split(StringHelper.LineSeparators, StringSplitOptions.None) });
		}

		#endregion
	}
}
