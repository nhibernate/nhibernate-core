using System;
using System.Data;
using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.Util;
using NHibernate.Sql;

namespace NHibernate.Mapping {
	
	public class Column {
		
		private const int DefaultPropertyLength = 255;

		private int length = DefaultPropertyLength;
		private IType type;
		private int typeIndex;
		private string name;
		private bool nullable = true;
		private bool unique = false;
		private string sqlType;
		internal int uniqueInteger;

		public int Length {
			get { return length; }
			set { length = value; }
		}

		public IType Type {
			get { return type; }
			set { type = value; }
		}

		public string Name {
			get { return name; }
			set { name = value; }
		}

		public string Alias {
			get {
				if ( name.Length < 11 )
					return name;
				else
					return (new Alias(10, uniqueInteger.ToString() + StringHelper.Underscore)).ToAliasString(name);
			}
		}

		public bool IsNullable { 
			get { return nullable; }
			set { nullable = value; }
		}

		public Column(IType type, int typeIndex) {
			this.type = type;
			this.typeIndex = typeIndex;
		}

		public int TypeIndex {
			get { return typeIndex; }
			set { typeIndex = value; }
		}

		public DbType GetAutoSqlType(IMapping mapping) {
			try {
				return Type.SqlTypes(mapping)[ TypeIndex ];
			} catch (Exception e) {
				throw new MappingException(
					"Could not determine type for column " + name + " of type " + type.GetType().FullName + ": " + e.GetType().FullName, e);
			}
		}

		public bool IsUnique {
			get { return unique; }
			set { unique = value; }
		}

		public string GetSqlType(Dialect.Dialect dialect, IMapping mapping) {
			return (sqlType==null) ? dialect.GetTypeName( GetAutoSqlType(mapping), Length ) : sqlType;
		}

		public DbType GetSqlType(IMapping mapping) {
			try {
				return Type.SqlTypes(mapping)[TypeIndex];
			}
			catch(Exception e) {
				throw new MappingException(
					"Could not determine type for column " +
					name +
					" of type " +
					type.GetType().Name +
					": " +
					e.GetType().Name, e);
			}
		}

		public override bool Equals(object obj) {
			return obj is Column && Equals( (Column) obj );
		}

		public bool Equals(Column column) {
			if (null == column) return false;
			if (this == column) return true;

			return name.Equals(column.name);
		}

		public override int GetHashCode() {
			return name.GetHashCode();
		}

		public string SqlType {
			get { return sqlType; }
			set { sqlType = value; }
		}
	}
}
