using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Xml.Schema;
using NHibernate.Engine;
using NHibernate.Id;
using NHibernate.Impl;
using NHibernate.Mapping;
using NHibernate.Persister.Entity;
using NHibernate.Type;

namespace NHibernate.Driver
{
	public abstract class TableBasedBulkProvider : BulkProvider
	{
		protected virtual IEnumerable<DataTable> GetTables<T>(ISessionImplementor session, IEnumerable<T> entities)
		{
			var tables = new Dictionary<String, DataTable>();

			foreach (var entityTypes in entities.GroupBy(x => x.GetType()))
			{
				var entityType = entityTypes.Key;
				var persister = session.GetEntityPersister(entityType.FullName, null) as AbstractEntityPersister;
				var table = new DataTable();

				if (persister is SingleTableEntityPersister)
				{
					table.TableName = (persister as SingleTableEntityPersister).TableName;
				}
				else if (persister is JoinedSubclassEntityPersister)
				{
					table.TableName = (persister as JoinedSubclassEntityPersister).TableName;
				}
				else if (persister is UnionSubclassEntityPersister)
				{
					table.TableName = (persister as UnionSubclassEntityPersister).TableName;
				}

				tables[table.TableName] = table;

				var map = new Hashtable();

				if (persister.IdentifierGenerator is IPostInsertIdentifierGenerator)
				{
					throw new ArgumentException("Post insert identifier generators cannot be used for bulk inserts.");
				}

				if (String.IsNullOrWhiteSpace(persister.IdentifierPropertyName) == true)
				{
					throw new ArgumentException("Entities without an identity property cannot be used for bulk inserts.");
				}

				for (var c = 0; c < persister.IdentifierColumnNames.Length; ++c)
				{
					var columnName = persister.IdentifierColumnNames[c];

					if (persister.IdentifierType is ComponentType)
					{
						table.Columns.Add(columnName, (persister.IdentifierType as ComponentType).ReturnedClass.GetProperty((persister.IdentifierType as ComponentType).PropertyNames[c]).PropertyType).ExtendedProperties["PropertyName"] = String.Concat(persister.IdentifierPropertyName, ".", (persister.IdentifierType as ComponentType).PropertyNames[c]);
					}
					else if (persister.EntityMetamodel.Properties[c].Type is OneToOneType)
					{
						table.Columns.Add(columnName, (persister.EntityMetamodel.Properties[c].Type as OneToOneType).GetIdentifierOrUniqueKeyPropertyName(session.Factory).GetType()).ExtendedProperties["PropertyName"] = String.Concat(persister.EntityMetamodel.Properties[c].Name, ".", (persister.EntityMetamodel.Properties[c].Type as OneToOneType).GetIdentifierOrUniqueKeyPropertyName(session.Factory));
					}
					else
					{
						table.Columns.Add(columnName, persister.IdentifierType.ReturnedClass).ExtendedProperties["PropertyName"] = persister.IdentifierPropertyName;
					}
				}

				for (var i = 0; i < persister.EntityMetamodel.Properties.Length; ++i)
				{
					if (persister.EntityMetamodel.PropertyInsertability[i] == false)
					{
						continue;
					}

					if (persister.EntityMetamodel.Properties[i].Type.IsCollectionType == true)
					{
						continue;
					}

					var columnNames = persister.GetPropertyColumnNames(persister.EntityMetamodel.Properties[i].Name);

					for (var c = 0; c < columnNames.Length; ++c)
					{
						var columnName = columnNames[c];

						if (persister.EntityMetamodel.Properties[i].Type is ComponentType)
						{
							table.Columns.Add(columnName, (persister.EntityMetamodel.Properties[i].Type as ComponentType).ReturnedClass.GetProperty((persister.EntityMetamodel.Properties[i].Type as ComponentType).PropertyNames[c]).PropertyType).ExtendedProperties["PropertyName"] = String.Concat(persister.EntityMetamodel.Properties[i].Name, ".", (persister.EntityMetamodel.Properties[i].Type as ComponentType).PropertyNames[c]);
						}
						else if (persister.EntityMetamodel.Properties[i].Type is OneToOneType)
						{
							table.Columns.Add(columnName, (persister.EntityMetamodel.Properties[i].Type as OneToOneType).GetIdentifierOrUniqueKeyPropertyName(session.Factory).GetType()).ExtendedProperties["PropertyName"] = String.Concat(persister.EntityMetamodel.Properties[i].Name, ".", (persister.EntityMetamodel.Properties[i].Type as OneToOneType).GetIdentifierOrUniqueKeyPropertyName(session.Factory));
						}
						else if (persister.EntityMetamodel.Properties[i].Type is ManyToOneType)
						{
							table.Columns.Add(columnName, (persister.EntityMetamodel.Properties[i].Type as ManyToOneType).GetIdentifierOrUniqueKeyPropertyName(session.Factory).GetType()).ExtendedProperties["PropertyName"] = String.Concat(persister.EntityMetamodel.Properties[i].Name, ".", (persister.EntityMetamodel.Properties[i].Type as ManyToOneType).GetIdentifierOrUniqueKeyPropertyName(session.Factory));
						}
						else
						{
							table.Columns.Add(columnName, persister.EntityMetamodel.Properties[i].Type.ReturnedClass).ExtendedProperties["PropertyName"] = persister.EntityMetamodel.Properties[i].Name;
						}
					}
				}

				table.BeginLoadData();

				foreach (var entity in entityTypes)
				{
					var row = table.NewRow();

					for (var c = 0; c < table.Columns.Count; ++c)
					{
						var value = DataBinder.Eval(entity, table.Columns[c].ExtendedProperties["PropertyName"].ToString()) ?? DBNull.Value;
						row[c] = value;
					}

					table.Rows.Add(row);
				}

				table.EndLoadData();
			}

			return (tables.Values);
		}
	}
}
