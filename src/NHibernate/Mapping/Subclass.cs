using System;
using System.Collections;
using NHibernate.Cache;
using NHibernate.Util;

namespace NHibernate.Mapping {
	
	public class Subclass : PersistentClass {
		
		private PersistentClass superclass;
		private Value key;

		public Subclass(PersistentClass superclass) {
			this.superclass = superclass;
		}

		public override ICacheConcurrencyStrategy Cache {
			get { return Superclass.Cache; }
		}

		public override RootClass RootClass {
			get { return Superclass.RootClass; }
		}

		public override PersistentClass Superclass {
			get { return superclass; }
			set { this.superclass = superclass; }
		}

		public override Property IdentifierProperty {
			get { return Superclass.IdentifierProperty; }
		}
		public override Value Identifier {
			get { return Superclass.Identifier; }
		}
		public override bool HasIdentifierProperty {
			get { return Superclass.HasIdentifierProperty; }
		}
		public override Value Discriminator {
			get { return Superclass.Discriminator; }
		}
		public override bool IsMutable {
			get { return Superclass.IsMutable; }
		}
		public override bool IsInherited {
			get { return true; }
		}
		public override bool IsPolymorphic {
			get { return true; }
		}
		public override void AddProperty(Property p) {
			base.AddProperty(p);
			Superclass.AddSubclassProperty(p);
		}
		public override Table Table {
			get { return Superclass.Table; }
			set {
				base.Table = value;
				Superclass.AddSubclassTable(value);
			}
		}
		public override ICollection PropertyClosureCollection {
			get { 
				ArrayList retVal = new ArrayList();
				retVal.AddRange( PropertyCollection );
				retVal.AddRange( Superclass.PropertyClosureCollection );
				return retVal;
			}
		}
		public override ICollection TableClosureCollection {
			get {
				ArrayList retVal = new ArrayList();
				retVal.AddRange( Superclass.TableClosureCollection );
				retVal.Add( Table );
				return retVal;
			}
		}
		public override void AddSubclassProperty(Property p) {
			base.AddSubclassProperty(p);
			Superclass.AddSubclassProperty(p);
		}
		public override void AddSubclassTable(Table table) {
			base.AddSubclassTable(table);
			Superclass.AddSubclassTable(table);
		}
		public override bool IsVersioned {
			get { return Superclass.IsVersioned; }
		}
		public override Property Version {
			get { return Superclass.Version; }
		}
		public override bool HasEmbeddedIdentifier {
			get { return Superclass.HasEmbeddedIdentifier; }
		}
		public override System.Type Persister {
			get { return Superclass.Persister; }
		}
		public override Table RootTable {
			get { return Superclass.RootTable; }
		}
		public override Value Key {
			get {
				if (key==null) {
					return Identifier;
				} else {
					return key;
				}
			}
			set { key = value; }
		}
		public override bool IsExplicitPolymorphism {
			get { return Superclass.IsExplicitPolymorphism; }
		}

	}
}
