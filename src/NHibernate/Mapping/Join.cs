using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Engine;
using NHibernate.SqlCommand;

namespace NHibernate.Mapping
{
	[Serializable]
	public class Join : ISqlCustomizable
	{
		private static readonly Alias PK_ALIAS = new Alias(15, "PK");

		private readonly List<Property> properties = new List<Property>();
		private Table table;
		private IKeyValue key;
		private PersistentClass persistentClass;
		private bool isSequentialSelect;
		private bool isInverse;
		private bool isOptional;
		private bool? isLazy;

		// Custom SQL
		private SqlString customSQLInsert;
		private bool customInsertCallable;
		private ExecuteUpdateResultCheckStyle insertCheckStyle;
		private SqlString customSQLUpdate;
		private bool customUpdateCallable;
		private ExecuteUpdateResultCheckStyle updateCheckStyle;
		private SqlString customSQLDelete;
		private bool customDeleteCallable;
		private ExecuteUpdateResultCheckStyle deleteCheckStyle;

		public void AddProperty(Property prop)
		{
			properties.Add(prop);
			prop.PersistentClass = PersistentClass;
		}

		//if we are joining to a non pk, this is the property of the class that serves as id
		public Property RefIdProperty { get; set; }

		public bool ContainsProperty(Property prop)
		{
			return properties.Contains(prop);
		}

		public IEnumerable<Property> PropertyIterator
		{
			get { return properties; }
		}

		public virtual Table Table
		{
			get { return table; }
			set { table = value; }
		}

		public virtual IKeyValue Key
		{
			get { return key; }
			set { key = value; }
		}

		public virtual PersistentClass PersistentClass
		{
			get { return persistentClass; }
			set { persistentClass = value; }
		}

		public void CreateForeignKey()
		{
			// TODO: The "if (!IsInverse)" condition is not in H3.2.
			// Not sure if this H3.2 does it in a different way.  The reason
			// this condition is put in is because when a <join> is mapped with
			// inverse="true", the joined rows should not be deleted (see 
			// comments in NH-466).  In other words, the joined row is
			// outside of the parent row's life cycle.  If this foreign
			// key is added, the parent row cannot be deleted independent
			// of the joined row.  Perhaps we need more clarification
			// on how this should behave.
			if (!IsInverse)
			{
				Key.CreateForeignKeyOfEntity(persistentClass.EntityName);
			}
		}

		public void CreatePrimaryKey(Dialect.Dialect dialect)
		{
			//Primary key constraint
			PrimaryKey pk = new PrimaryKey();
			pk.Table = table;
			pk.Name = PK_ALIAS.ToAliasString(table.Name, dialect);
			table.PrimaryKey = pk;

			pk.AddColumns(Key.ColumnIterator.OfType<Column>());
		}

		public int PropertySpan
		{
			get { return properties.Count; }
		}

		public SqlString CustomSQLInsert { get { return customSQLInsert; } }
		public SqlString CustomSQLDelete { get { return customSQLDelete; } }
		public SqlString CustomSQLUpdate { get { return customSQLUpdate; } }

		public bool IsCustomInsertCallable { get { return customInsertCallable; } }
		public bool IsCustomDeleteCallable { get { return customDeleteCallable; } }
		public bool IsCustomUpdateCallable { get { return customUpdateCallable; } }

		public ExecuteUpdateResultCheckStyle CustomSQLInsertCheckStyle { get { return insertCheckStyle; } }
		public ExecuteUpdateResultCheckStyle CustomSQLDeleteCheckStyle { get { return deleteCheckStyle; } }
		public ExecuteUpdateResultCheckStyle CustomSQLUpdateCheckStyle { get { return updateCheckStyle; } }

		public void SetCustomSQLInsert(string sql, bool callable, ExecuteUpdateResultCheckStyle checkStyle)
		{
			customSQLInsert = SqlString.Parse(sql);
			customInsertCallable = callable;
			insertCheckStyle = checkStyle;
		}

		public void SetCustomSQLDelete(string sql, bool callable, ExecuteUpdateResultCheckStyle checkStyle)
		{
			customSQLDelete = SqlString.Parse(sql);
			customDeleteCallable = callable;
			deleteCheckStyle = checkStyle;
		}

		public void SetCustomSQLUpdate(string sql, bool callable, ExecuteUpdateResultCheckStyle checkStyle)
		{
			customSQLUpdate = SqlString.Parse(sql);
			customUpdateCallable = callable;
			updateCheckStyle = checkStyle;
		}

		public virtual bool IsSequentialSelect
		{
			get { return isSequentialSelect; }
			set { isSequentialSelect = value; }
		}

		public virtual bool IsInverse
		{
			get { return isInverse; }
			set { isInverse = value; }
		}

		public bool IsLazy
		{
			get
			{
				if (!isLazy.HasValue)
				{
					var hasAllLazyProperties = !PropertyIterator.Any(property=> property.IsLazy == false);
					isLazy = hasAllLazyProperties;
				}
				return isLazy.Value;
			}

		}

		public virtual bool IsOptional
		{
			get { return isOptional; }
			set { isOptional = value; }
		}
	}
}