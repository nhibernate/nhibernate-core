using System;
using System.Text;
using System.Data;
using System.Collections;
using NHibernate.Util;
using NHibernate.Dialect;
using NHibernate.Id;
using NHibernate.Engine;

namespace NHibernate.Mapping {
	
	/// <summary>
	/// Represents a Table in a database that an object gets mapped against.
	/// </summary>
	public class Table : IRelationalModel {
		
		private string name;
		private bool quoted;
		private string schema;
		private SequencedHashMap columns = new SequencedHashMap();
		private Value idValue;
		private PrimaryKey primaryKey;
		private IDictionary indexes = new Hashtable();
		private IDictionary foreignKeys = new Hashtable();
		private IDictionary uniqueKeys = new Hashtable();
		private int uniqueInteger;
		private static int tableCounter = 0;

		public Table() {
			uniqueInteger = tableCounter++;
		}

		public string GetQualifiedName(Dialect.Dialect dialect) 
		{
			string quotedName = GetQuotedName(dialect);
			return schema==null ? quotedName : schema + StringHelper.Dot + quotedName;
		}

//		/// <summary>
//		/// Returns the QualifiedName for the table by combining the Schema and Name property.
//		/// </summary>
//		public string QualifiedName {
//			get { return (schema == null) ? name : schema + StringHelper.Dot + name; }
//		}

		
		/// <summary>
		/// Returns the QualifiedName for the table using the specified Qualifier
		/// </summary>
		/// <param name="defaultQualifier">The Qualifier to use when accessing the table.</param>
		/// <returns>A String representing the Qualified name.</returns>
		/// <remarks>If this were used with MSSQL it would return a dbo.table_name.</remarks>
		public string GetQualifiedName(Dialect.Dialect dialect, string defaultQualifier) {
			string quotedName = GetQuotedName(dialect);
			return (schema==null) ? ( (defaultQualifier==null) ? quotedName : defaultQualifier + StringHelper.Dot + name ) : GetQualifiedName(dialect);
		}

		public string Name {
			get { return name; }
			set 
			{ 
				if (value[0]=='`') 
				{
					quoted = true;
					name = value.Substring(1, value.Length-2);
				}
				else 
				{
					name = value; 
				}
			}
		}

		public string GetQuotedName(Dialect.Dialect dialect) 
		{
			return quoted ? 
				dialect.OpenQuote + name + dialect.CloseQuote :
				name;
		}
		public Column GetColumn(int n) {
			IEnumerator iter = columns.Values.GetEnumerator();
			for (int i=0; i<n; i++) iter.MoveNext();
			return (Column) iter.Current;
		}
		
		public void AddColumn(Column column) {
			Column old = (Column) columns[ column.Name ];
			if (old == null) {
				columns[column.Name] = column;
				column.uniqueInteger = columns.Count;
			} else {
				column.uniqueInteger = old.uniqueInteger;
			}
		}
		public int ColumnSpan {
			get { return columns.Count; }
		}
		public ICollection ColumnCollection {
			get { return columns.Values; }
		}
		public ICollection IndexCollection {
			get { return indexes.Values; }
		}
		public ICollection ForeignKeyCollection {
			get { return foreignKeys.Values; }
		}
		public ICollection UniqueKeyCollection {
			get { return uniqueKeys.Values; }
		}

		public string SqlAlterString(Dialect.Dialect dialect, IMapping p, DataTable tableInfo) {
			StringBuilder buf = new StringBuilder(50);
			foreach(Column col in ColumnCollection) {
				DataColumn columnInfo = tableInfo.Columns[ col.Name ];

				if (columnInfo == null) {
					// the column doesnt exist at all
					if (buf.Length != 0)
						buf.Append(StringHelper.CommaSpace);
						buf.Append(col.GetQuotedName(dialect))
							.Append(' ')
							.Append(col.GetSqlType(dialect, p));
					if (col.IsUnique && dialect.SupportsUnique) {
						buf.Append(" unique");
					}
				}
			}

			if (buf.Length == 0)
				return null;

			return new StringBuilder("alter table ").Append(GetQualifiedName(dialect)).Append(" add ").Append(buf).ToString();

		}

		public string SqlCreateString(Dialect.Dialect dialect, IMapping p) {
			StringBuilder buf = new StringBuilder("create table ")
				.Append( GetQualifiedName(dialect) )
				.Append( " (");

			bool identityColumn = idValue!=null && idValue.CreateIdentifierGenerator(dialect) is IdentityGenerator;

			// try to find out the name of the pk to create it as identity if the identitygenerator is used
			string pkname = null;
			if (primaryKey != null && identityColumn ) {
				foreach(Column col in primaryKey.ColumnCollection) {
					pkname = col.GetQuotedName(dialect); //should only go through this loop once
				}
			}
			int i = 0;
			foreach(Column col in ColumnCollection) {
				i++;
				buf.Append( col.GetQuotedName(dialect) )
					.Append(' ')
					.Append( col.GetSqlType(dialect, p) );

				if ( identityColumn && col.GetQuotedName(dialect).Equals(pkname) ) {
					buf.Append(' ')
						.Append( dialect.IdentityColumnString);
				} else {
					if (col.IsNullable) {
						buf.Append( dialect.NullColumnString );
					} else {
						buf.Append(" not null" );
					}
				}

				if ( col.IsUnique && dialect.SupportsUnique ) {
					buf.Append(" unique");
				}
				if ( i < ColumnCollection.Count ) buf.Append(StringHelper.CommaSpace);
			}
			if (primaryKey != null) {
				//if ( dialect is HSQLDialect && identityColumn ) {
				//	//ugly hack...
				//} else {
				buf.Append(',').Append( primaryKey.SqlConstraintString(dialect) );
				//}
			}
			if ( dialect.SupportsUnique ) {
				foreach(UniqueKey uk in UniqueKeyCollection) {
					buf.Append(',').Append( uk.SqlConstraintString(dialect) );
				}
			}
			buf.Append(StringHelper.ClosedParen);

			return buf.ToString();
		}

		public string SqlDropString(Dialect.Dialect dialect) {
			return "drop table " + GetQualifiedName(dialect) + dialect.CascadeConstraintsString;
		}

		public PrimaryKey PrimaryKey {
			get { return primaryKey; }
			set { primaryKey = value; }
		}

		public Index GetIndex(string name) {
			Index index = (Index) indexes[name];

			if (index == null) {
				index = new Index();
				index.Name = name;
				index.Table = this;
				indexes.Add(name, index);
			}

			return index;
		}

		public UniqueKey GetUniqueKey(string name) {
			UniqueKey uk = (UniqueKey) uniqueKeys[name];

			if (uk == null) {
				uk = new UniqueKey();
				uk.Name = name;
				uk.Table = this;
				uniqueKeys.Add(name, uk);
			}
			return uk;
		}

		public ForeignKey CreateForeignKey(IList columns) {
			string name = "FK" + UniqueColumnString( columns );
			ForeignKey fk = (ForeignKey) foreignKeys[name];

			if (fk == null) {
				fk = new ForeignKey();
				fk.Name = name;
				fk.Table = this;
				foreignKeys.Add(name, fk);
			}
			foreach(Column col in columns) {
				fk.AddColumn( col );
			}
			return fk;
		}

		public string UniqueColumnString(ICollection col) {
			
			int result = 0;
			
			foreach(object obj in col) {
				
				// this is marked as unchecked because the GetHashCode could potentially
				// cause an integer overflow.  This way if there is an overflow it will
				// just roll back over.
				unchecked{ result += obj.GetHashCode(); }
			}
			
			return ( name.GetHashCode().ToString("X") + result.GetHashCode().ToString("X") );
		}

		public string Schema {
			get { return schema; }
			set { schema = value; }
		}

		public int UniqueInteger {
			get { return uniqueInteger; }
		}

		public void SetIdentifierValue(Value idValue) {
			this.idValue = idValue;
		}

	

	}
}
