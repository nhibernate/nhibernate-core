using System;
using System.Collections;
using NHibernate.Cache;
using NHibernate.Util;

namespace NHibernate.Mapping {
	
	public abstract class PersistentClass {
		private System.Type persistentClass;
		private string discriminatorValue;
		private ArrayList properties = new ArrayList();
		private Table table;
		private System.Type proxyInterface;
		private bool dynamicUpdate;
		private ArrayList subclasses = new ArrayList();
		private ArrayList subclassProperties = new ArrayList();
		private ArrayList subclassTables = new ArrayList();

		public bool DynamicUpdate {
			get { return dynamicUpdate; }
			set { dynamicUpdate = value ; }
		}

		public string DiscriminatorValue {
			get { return discriminatorValue; }
			set { discriminatorValue = value; }
		}

		public virtual void AddSubclass(Subclass subclass) {
			subclasses.Add(subclass);
		}

		public bool HasSubclasses {
			get { return subclasses.Count > 0; }
		}

		public int SubclassSpan {
			get {
				int n = subclasses.Count;
				foreach(Subclass sc in subclasses) {
					n += sc.SubclassSpan;
				}
				return n;
			}
		}

		public ICollection SubclassCollection {
			get { 
				ArrayList retVal = new ArrayList();
				foreach(Subclass sc in subclasses) {
					retVal.AddRange(sc.SubclassCollection);
				}
				return retVal;
			}
		}

		public ICollection DirectSubclasses {
			get { return subclasses; }
		}

		public virtual void AddProperty(Property p) {
			properties.Add(p);
		}

		public virtual Table Table {
			get { return table; }
			set { table = value; }
		}

		public ICollection PropertyCollection {
			get { return properties; }
		}

		public System.Type PersistentClazz {
			get { return persistentClass; }
			set { persistentClass = value; }
		}

		public string Name {
			get { return persistentClass.Name; }
		}

		public abstract bool IsMutable { get; set;}
		public abstract bool HasIdentifierProperty { get; }
		public abstract Property IdentifierProperty { get; set; }
		public abstract Value Identifier { get; set; }
		public abstract Property Version { get; set; }
		public abstract Value Discriminator { get; set; }
		public abstract bool IsInherited { get;  }
		public abstract bool IsPolymorphic { get; set; }
		public abstract bool IsVersioned { get;}
		public abstract ICacheConcurrencyStrategy Cache { get; set;}
		public abstract PersistentClass Superclass { get; set; }
		public abstract bool IsExplicitPolymorphism { get; set;}

		public abstract ICollection PropertyClosureCollection { get; }
		public abstract ICollection TableClosureCollection { get; }

		public virtual void AddSubclassProperty(Property p) {
			subclassProperties.Add(p);
		}
		public virtual void AddSubclassTable(Table table) {
			subclassTables.Add(table);
		}
		public ICollection SubclassPropertyClosureCollection {
			get {
				ArrayList retVal = new ArrayList();
				retVal.AddRange( PropertyClosureCollection );
				retVal.AddRange( subclassProperties );
				return retVal;
			}
		}
		public ICollection SubclassTableClosureCollection {
			get {
				ArrayList retVal = new ArrayList();
				retVal.AddRange( TableClosureCollection );
				retVal.AddRange( subclassTables );
				return retVal;
			}
		}
		public System.Type ProxyInterface {
			get { return proxyInterface; }
			set { proxyInterface = value ; }
		}

		public virtual bool IsForceDiscriminator {
			get {
				return false;
			}
			set {
			}
		}

		public abstract bool HasEmbeddedIdentifier { get; set;}
		public abstract System.Type Persister { get; set;}
		public abstract Table RootTable { get; }
		public abstract RootClass RootClazz { get; }
		public abstract Value Key { get; set; }

		public void CreatePrimaryKey() {
			PrimaryKey pk = new PrimaryKey();
			pk.Table = table;
			pk.Name = StringHelper.Suffix( table.Name, "PK" );
			table.PrimaryKey = pk;

			foreach(Column col in Key.ColumnCollection) {
				pk.AddColumn(col);
			}
		}


	}
}
