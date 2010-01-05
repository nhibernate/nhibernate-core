using System;
using System.Collections.Generic;
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
				BindFormula(formulas);
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

		public void BindSimpleValue(HbmElement element, string propertyPath, bool isNullable)
		{
			new TypeBinder(value, Mappings).Bind(element.Type);
			var formulas = element.Formulas.ToArray();
			if (formulas.Length > 0)
			{
				BindFormula(formulas);
			}
			else
			{
				new ColumnsBinder(value, Mappings).Bind(element.Columns, isNullable,
																								() =>
																								new HbmColumn
																								{
																									name = mappings.NamingStrategy.PropertyToColumnName(propertyPath),
																									length = element.length,
																									scale = element.scale,
																									precision = element.precision,
																									notnull = element.notnull,
																									notnullSpecified = true,
																									unique = element.unique,
																									uniqueSpecified = true,
																								});
			}
		}

		private void BindFormula(IEnumerable<HbmFormula> formulas)
		{
			foreach (var hbmFormula in formulas)
			{
				value.AddFormula(new Formula { FormulaString = hbmFormula.Text.LinesToString() });
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

		public void BindSimpleValue(HbmManyToMany manyToManyMapping, string propertyPath, bool isNullable)
		{
			var formulas = manyToManyMapping.Formulas.ToArray();
			if (formulas.Length > 0)
			{
				BindFormula(formulas);
			}
			else
			{
				new ColumnsBinder(value, Mappings).Bind(manyToManyMapping.Columns, isNullable,
																								() =>
																								new HbmColumn
																								{
																									name = mappings.NamingStrategy.PropertyToColumnName(propertyPath),
																									unique = manyToManyMapping.unique,
																									uniqueSpecified = true
																								});
			}
		}

		public void BindSimpleValue(HbmCollectionId collectionIdMapping, string propertyPath)
		{
			new TypeBinder(value, Mappings).Bind(collectionIdMapping.Type);
			new ColumnsBinder(value, Mappings).Bind(collectionIdMapping.Columns, false,
																								() =>
																								new HbmColumn
																								{
																									name = mappings.NamingStrategy.PropertyToColumnName(propertyPath),
																									length = collectionIdMapping.length,
																								});
		}

		public void BindSimpleValue(HbmListIndex listIndexMapping, string propertyPath, bool isNullable)
		{
			new TypeBinder(value, Mappings).Bind(NHibernateUtil.Int32.Name);
			new ColumnsBinder(value, Mappings).Bind(listIndexMapping.Columns, false,
																								() =>
																								new HbmColumn
																								{
																									name = mappings.NamingStrategy.PropertyToColumnName(propertyPath),
																								});
		}

		public void BindSimpleValue(HbmIndex indexMapping, string propertyPath, bool isNullable)
		{
			new TypeBinder(value, Mappings).Bind(indexMapping.Type);
			new ColumnsBinder(value, Mappings).Bind(indexMapping.Columns, isNullable,
																								() =>
																								new HbmColumn
																								{
																									name = mappings.NamingStrategy.PropertyToColumnName(propertyPath),
																									length = indexMapping.length,
																								});
		}

		public void BindSimpleValue(HbmMapKey mapKeyMapping, string propertyPath, bool isNullable)
		{
			new TypeBinder(value, Mappings).Bind(mapKeyMapping.Type);
			var formulas = mapKeyMapping.Formulas.ToArray();
			if (formulas.Length > 0)
			{
				BindFormula(formulas);
			}
			else
			{
				new ColumnsBinder(value, Mappings).Bind(mapKeyMapping.Columns, isNullable,
				                                        () =>
				                                        new HbmColumn
				                                        	{
				                                        		name = mappings.NamingStrategy.PropertyToColumnName(propertyPath),
				                                        		length = mapKeyMapping.length
				                                        	});
			}
		}

		public void BindSimpleValue(HbmManyToOne manyToManyMapping, string propertyPath, bool isNullable)
		{
			var formulas = manyToManyMapping.Formulas.ToArray();
			if (formulas.Length > 0)
			{
				BindFormula(formulas);
			}
			else
			{
				new ColumnsBinder(value, Mappings).Bind(manyToManyMapping.Columns, isNullable,
																								() =>
																								new HbmColumn
																								{
																									name = mappings.NamingStrategy.PropertyToColumnName(propertyPath),
																									notnull = manyToManyMapping.notnull,
																									notnullSpecified = manyToManyMapping.notnullSpecified,
																									unique = manyToManyMapping.unique,
																									uniqueSpecified = true,
																									uniquekey = manyToManyMapping.uniquekey,
																									index = manyToManyMapping.index
																								});
			}
		}

		public void BindSimpleValue(HbmIndexManyToMany indexManyToManyMapping, string propertyPath, bool isNullable)
		{
			new ColumnsBinder(value, Mappings).Bind(indexManyToManyMapping.Columns, isNullable,
																								() =>
																								new HbmColumn
																								{
																									name = mappings.NamingStrategy.PropertyToColumnName(propertyPath),
																								});
		}

		public void BindSimpleValue(HbmMapKeyManyToMany mapKeyManyToManyMapping, string propertyPath, bool isNullable)
		{
			var formulas = mapKeyManyToManyMapping.Formulas.ToArray();
			if (formulas.Length > 0)
			{
				BindFormula(formulas);
			}
			else
			{
				new ColumnsBinder(value, Mappings).Bind(mapKeyManyToManyMapping.Columns, isNullable,
																								() =>
																								new HbmColumn
																								{
																									name = mappings.NamingStrategy.PropertyToColumnName(propertyPath),
																								});
			}
		}

		public void BindSimpleValue(HbmKeyProperty mapKeyManyToManyMapping, string propertyPath, bool isNullable)
		{
			new TypeBinder(value, Mappings).Bind(mapKeyManyToManyMapping.Type);
			new ColumnsBinder(value, Mappings).Bind(mapKeyManyToManyMapping.Columns, isNullable,
			                                        () =>
			                                        new HbmColumn
			                                        	{
			                                        		name = mappings.NamingStrategy.PropertyToColumnName(propertyPath),
			                                        		length = mapKeyManyToManyMapping.length,
			                                        	});

		}

		public void BindSimpleValue(HbmKeyManyToOne mapKeyManyToManyMapping, string propertyPath, bool isNullable)
		{
			new ColumnsBinder(value, Mappings).Bind(mapKeyManyToManyMapping.Columns, isNullable,
			                                        () =>
			                                        new HbmColumn
			                                        	{name = mappings.NamingStrategy.PropertyToColumnName(propertyPath),});
		}
	}
}