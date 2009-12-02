using System;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping;
using NHibernate.Util;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public class ValuePropertyBinder : Binder
	{
		private readonly SimpleValue value;

		public ValuePropertyBinder(SimpleValue value, Mappings mappings)
			: base(mappings)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this.value = value;
		}

		//automatically makes a column with the default name if none is specified by XML
		public void BindSimpleValue(HbmProperty propertyMapping, string propertyPath, bool isNullable)
		{
			new TypeBinder(value, Mappings).Bind(propertyMapping.Type);
			var formulas = propertyMapping.Formulas.ToArray();
			if (formulas.Length > 0)
			{
				foreach (var hbmFormula in formulas)
				{
					value.AddFormula(new Formula {FormulaString = hbmFormula.Text.LinesToString()});
				}
			}
			else
			{
				new ColumnsBinder(value, Mappings).Bind(propertyMapping.Columns, isNullable,
				                                        () =>
				                                        new HbmColumn
				                                        	{
				                                        		name = mappings.NamingStrategy.PropertyToColumnName(propertyPath),
				                                        		length = propertyMapping.length,
				                                        		scale = propertyMapping.scale,
				                                        		precision = propertyMapping.precision,
				                                        		notnull = propertyMapping.notnull,
				                                        		notnullSpecified = propertyMapping.notnullSpecified,
				                                        		unique = propertyMapping.unique,
				                                        		uniqueSpecified = true,
				                                        		uniquekey = propertyMapping.uniquekey,
				                                        		index = propertyMapping.index
				                                        	});
			}
		}

		public void BindSimpleValue(HbmKey propertyMapping, string propertyPath, bool isNullable)
		{
			new ColumnsBinder(value, Mappings).Bind(propertyMapping.Columns, isNullable,
			                                        () =>
			                                        new HbmColumn
			                                        	{
			                                        		name = mappings.NamingStrategy.PropertyToColumnName(propertyPath),
			                                        		notnull = propertyMapping.notnull,
			                                        		notnullSpecified = propertyMapping.notnullSpecified,
			                                        		unique = propertyMapping.unique,
			                                        		uniqueSpecified = propertyMapping.uniqueSpecified,
			                                        	});
		}
	}
}