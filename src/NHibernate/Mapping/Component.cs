using System;
using System.Data;
using System.Collections;

namespace NHibernate.Mapping {
	
	public class Component : Value {
		private ArrayList properties = new ArrayList();
		private System.Type componentClass;
		private bool embedded;
		private string parentProperty;
		private PersistentClass owner;

		public int PropertySpan {
			get { return properties.Count; }
		}
		public ICollection PropertyCollection {
			get { return properties; }
		}
		public void AddProperty(Property p) {
			properties.Add(p);
		}
		public override void AddColumn(Column column) {
			throw new NotSupportedException("Cant add a column to a component");
		}
		public override int ColumnSpan {
			get {
				int n=0;
				foreach(Property p in PropertyCollection) {
					n+= p.ColumnSpan;
				}
				return n;
			}
		}
		
		public override ICollection ColumnCollection {
			get {
				ArrayList retVal = new ArrayList();
				foreach(Property prop in PropertyCollection) {
					retVal.AddRange(prop.ColumnCollection);
				}
				return retVal;
			}
		}

		public Component(PersistentClass owner) : base(owner.Table) {
			this.owner = owner;
		}

		public Component(Table table) : base(table) {
			this.owner = null;
		}

		public override void SetTypeByReflection(System.Type propertyClass, string propertyName) {
		}

		public bool IsEmbedded {
			get { return embedded; }
			set { embedded = value; }
		}

		public override bool IsComposite {
			get { return true; }
		}

		public System.Type ComponentClass {
			get { return componentClass; }
			set { componentClass = value; }
		}

		public PersistentClass Owner {
			get { return owner; }
			set { owner = value; }
		}

		public string ParentProperty {
			get { return parentProperty; }
			set { parentProperty = value; }
		}

		public ArrayList Properties {
			get { return properties; }
		}


	}
}
