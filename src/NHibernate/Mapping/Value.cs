using System;
using System.Collections;
using NHibernate.Dialect;
using NHibernate.Id;
using NHibernate.Loader;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Mapping {
	/// <summary>
	/// A value represents a simple thing that maps down to a table column or columns.
	/// Higher level things like classes, properties and collection add semantics to instances
	/// of this class
	/// </summary>
	public class Value {
		private ArrayList columns = new ArrayList();
		private IType type;
		private IDictionary identifierGeneratorProperties;
		private string identifierGeneratorStrategy = "assigned";
		private string nullValue;
		private Table table;

		public Value(Table table) {
			this.table = table;
		}

		public virtual void AddColumn(Column column) {
			if ( !columns.Contains(column) ) columns.Add(column);
		}
		public virtual int ColumnSpan {
			get { return columns.Count; }
		}
		public virtual ICollection ColumnCollection {
			get { return columns; }
		}
		public virtual IList ConstraintColumns {
			get { return columns; }
		}
		public virtual IType Type {
			get { return type; }
			set {
				this.type = value;
				int count = 0;
				
				foreach(Column col in ColumnCollection) {
					col.Type = type;
					col.TypeIndex = count++;
				}
			}
		}

		public Table Table {
			get { return table; }
			set { table = value; }
		}

		public virtual void CreateForeignKey() {
		}

		public void CreateForeignKeyOfClass(System.Type persistentClass) {
			ForeignKey fk = table.CreateForeignKey( ConstraintColumns );
			fk.ReferencedClass = persistentClass;
		}

		public IIdentifierGenerator CreateIdentifierGenerator(Dialect.Dialect dialect) {
			//return IdentifierGeneratorFactory.Create(identifierGeneratorStrategy, type, identifierGeneratorProperties, dialect);
			//TODO: finish
			return null;
		}

		public virtual void SetTypeByReflection(System.Type propertyClass, string propertyName) {
			try {
				if (type==null) {
					type = ReflectHelper.ReflectedPropertyType(propertyClass, propertyName);
					int count = 0;
					foreach(Column col in ColumnCollection) {
						col.Type = type;
						col.TypeIndex = count++;
					}
				}
			} catch (HibernateException he) {
				throw new MappingException("Problem trying to set property type by reflection", he);
			}
		}

		public virtual OuterJoinLoaderType OuterJoinFetchSetting {
			get { 
				return OuterJoinLoaderType.Lazy; 
			}
			set {
				throw new NotSupportedException();
			}
		}

		public string IdentifierGeneratorStrategy {
			get { return identifierGeneratorStrategy; }
			set { identifierGeneratorStrategy = value; }
		}

		public IDictionary IdentifierGeneratorProperties {
			get { return identifierGeneratorProperties; }
			set { identifierGeneratorProperties = value; }
		}

		public virtual bool IsComposite {
			get { return false; }
		}

		public string NullValue {
			get { return nullValue; }
			set { nullValue = value; }
		}

		


		
	}
}
