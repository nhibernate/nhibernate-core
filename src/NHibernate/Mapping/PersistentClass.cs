using System;
using System.Collections;
using NHibernate.Cache;
using NHibernate.Util;
using NHibernate.Sql;

namespace NHibernate.Mapping {
	
	public abstract class PersistentClass {
		private System.Type persistentClass;
		private string discriminatorValue;
		private ArrayList properties = new ArrayList();
		private Table table;
		private System.Type proxyInterface;
		private bool dynamicUpdate;
		private readonly ArrayList subclasses = new ArrayList();
		private readonly ArrayList subclassProperties = new ArrayList();
		private readonly ArrayList subclassTables = new ArrayList();

		public virtual bool DynamicUpdate {
			get { return dynamicUpdate; }
			set { dynamicUpdate = value ; }
		}

		public virtual string DiscriminatorValue {
			get { return discriminatorValue; }
			set { discriminatorValue = value; }
		}

		public virtual void AddSubclass(Subclass subclass) {
			subclasses.Add(subclass);
		}

		public virtual bool HasSubclasses {
			get { return subclasses.Count > 0; }
		}

		public virtual int SubclassSpan {
			get {
				int n = subclasses.Count;
				foreach(Subclass sc in subclasses) {
					n += sc.SubclassSpan;
				}
				return n;
			}
		}

		/// <summary>
		/// Gets the Collection of Subclasses for this PersistentClass.  It will
		/// recursively go through Subclasses so that if a Subclass has Subclasses
		/// it will pick those up also.
		/// </summary>
		public virtual ICollection SubclassCollection {
			get { 
				ArrayList retVal = new ArrayList();
				
				// check to see if there are any subclass in our subclasses 
				// and add them into the collection
				foreach(Subclass sc in subclasses) {
					retVal.AddRange(sc.SubclassCollection);
				}

				// finally add the subclasses from this PersistentClass into
				// the collection to return
				retVal.AddRange(subclasses);

				return retVal;
			}
		}

		public virtual ICollection DirectSubclasses {
			get { return subclasses; }
		}

		public virtual void AddProperty(Property p) {
			properties.Add(p);
		}

		public virtual Table Table {
			get { return table; }
			set { table = value; }
		}

		public virtual ICollection PropertyCollection {
			get { return properties; }
		}

		public virtual System.Type PersistentClazz {
			get { return persistentClass; }
			set { persistentClass = value; }
		}

		public virtual string Name {
			get { return persistentClass.Name; }
		}

		public abstract bool IsMutable { get; set;}
		public abstract bool HasIdentifierProperty { get; }
		public abstract Property IdentifierProperty { get; set; }
		public abstract Value Identifier { get; set; }
		public abstract Property Version { get; set; }
		public abstract Value Discriminator { get; set; }
		public abstract bool IsInherited { get;  }
		// see the comment in RootClass about why the polymorphic setter is commented out
		public abstract bool IsPolymorphic { get; } //set; }
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
		public virtual ICollection SubclassPropertyClosureCollection {
			get {
				ArrayList retVal = new ArrayList();
				retVal.AddRange( PropertyClosureCollection );
				retVal.AddRange( subclassProperties );
				return retVal;
			}
		}
		
		/// <summary>
		/// Returns an ICollection of all of the Tables that the subclass finds its information
		/// in.  
		/// </summary>
		/// <remarks>It adds the TableClosureCollection and the subclassTables into the ICollection.</remarks>
		public virtual ICollection SubclassTableClosureCollection {
			get {
				ArrayList retVal = new ArrayList();
				retVal.AddRange( TableClosureCollection );
				retVal.AddRange( subclassTables );
				return retVal;
			}
		}
		public virtual System.Type ProxyInterface {
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

		public virtual void CreatePrimaryKey() {
			PrimaryKey pk = new PrimaryKey();
			pk.Table = table;
			pk.Name = StringHelper.Suffix( table.Name, "PK" );
			table.PrimaryKey = pk;

			foreach(Column col in Key.ColumnCollection) {
				pk.AddColumn(col);
			}
		}

		private static readonly Alias PKAlias = new Alias(15, "PK");
	}
}
