using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Type;
using NHibernate.UserTypes;
using NHibernate.Util;

namespace NHibernate.Mapping.ByCode.Impl
{
	public class MapKeyMapper : IMapKeyMapper
	{
		private readonly HbmMapKey hbmMapKey;

		public MapKeyMapper(HbmMapKey hbmMapKey)
		{
			this.hbmMapKey = hbmMapKey;
		}

		public HbmMapKey MapKeyMapping
		{
			get { return hbmMapKey; }
		}

		#region IMapKeyMapper Members

		public void Column(Action<IColumnMapper> columnMapper)
		{
			if (hbmMapKey.Columns.Count() > 1)
			{
				throw new MappingException("Multi-columns property can't be mapped through singlr-column API.");
			}
			hbmMapKey.formula = null;
			HbmColumn hbm = hbmMapKey.Columns.SingleOrDefault();
			hbm = hbm
			      ??
			      new HbmColumn
			      {
			      	name = hbmMapKey.column,
			      	length = hbmMapKey.length,
			      };
			string defaultColumnName = "mapKey";
			columnMapper(new ColumnMapper(hbm, defaultColumnName));
			if (ColumnTagIsRequired(hbm))
			{
				hbmMapKey.Items = new[] {hbm};
				ResetColumnPlainValues();
			}
			else
			{
				hbmMapKey.column = !defaultColumnName.Equals(hbm.name) ? hbm.name : null;
				hbmMapKey.length = hbm.length;
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
				string defaultColumnName = "mapKey" + i++;
				action(new ColumnMapper(hbm, defaultColumnName));
				columns.Add(hbm);
			}
			hbmMapKey.Items = columns.ToArray();
		}

		public void Column(string name)
		{
			Column(x => x.Name(name));
		}

		public void Type(IType persistentType)
		{
			if (persistentType != null)
			{
				hbmMapKey.type = persistentType.Name;
			}
		}

		public void Type<TPersistentType>()
		{
			Type(typeof (TPersistentType));
		}

		public void Type(System.Type persistentType)
		{
			if (persistentType == null)
			{
				throw new ArgumentNullException("persistentType");
			}
			if (!typeof (IUserType).IsAssignableFrom(persistentType) && !typeof (IType).IsAssignableFrom(persistentType))
			{
				throw new ArgumentOutOfRangeException("persistentType", "Expected type implementing IUserType or IType.");
			}
			hbmMapKey.type = persistentType.AssemblyQualifiedName;
		}

		public void Length(int length)
		{
			Column(x => x.Length(length));
		}

		public void Formula(string formula)
		{
			if (formula == null)
			{
				return;
			}

			ResetColumnPlainValues();
			hbmMapKey.Items = null;
			string[] formulaLines = formula.Split(StringHelper.LineSeparators, StringSplitOptions.None);
			if (formulaLines.Length > 1)
			{
				hbmMapKey.Items = new[] {new HbmFormula {Text = formulaLines}};
			}
			else
			{
				hbmMapKey.formula = formula;
			}
		}

		#endregion

		private void ResetColumnPlainValues()
		{
			hbmMapKey.column = null;
			hbmMapKey.length = null;
			hbmMapKey.formula = null;
		}

		private bool ColumnTagIsRequired(HbmColumn hbm)
		{
			return hbm.precision != null || hbm.scale != null || hbm.notnull || hbm.unique
			       || hbm.uniquekey != null || hbm.sqltype != null || hbm.index != null || hbm.@default != null
			       || hbm.check != null;
		}
	}
}