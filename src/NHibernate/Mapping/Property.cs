using System;
using System.Collections;
using NHibernate.Type;
using NHibernate.Engine;

namespace NHibernate.Mapping {
	
	public class Property {
		private string name;
		private Value value;
		private string cascade;
		private bool updateable;
		private bool insertable;

		public Property(Value value) {
			this.value = value;
		}

		public IType Type {
			get { return value.Type; }
		}

		public int ColumnSpan {
			get { return value.ColumnSpan; }
		}

		public ICollection ColumnCollection {
			get { return value.ColumnCollection; }
		}

		public string Name {
			get { return name; }
			set { name = value; }
		}

		public bool IsUpdateable {
			get { return updateable; }
			set { updateable = value; }
		}

		public bool IsComposite {
			get { return value is Component; }
		}

		public Value Value {
			get { return value; }
			set { this.value = value; }
		}

		public Cascades.CascadeStyle CascadeStyle {
			get {
				IType type = value.Type;
				if (type.IsComponentType && !type.IsObjectType) {
					IAbstractComponentType actype = (IAbstractComponentType) value.Type;
					int length = actype.Subtypes.Length;
					for (int i=0; i<length; i++) {
						if ( actype.Cascade(i)!= Cascades.CascadeStyle.StyleNone ) return Cascades.CascadeStyle.StyleAll;
					}
					return Cascades.CascadeStyle.StyleNone;
				} else {
					if (cascade.Equals("all")) {
						return Cascades.CascadeStyle.StyleAll;
					} else if (cascade.Equals("none")) {
						return Cascades.CascadeStyle.StyleNone;
					} else if (cascade.Equals("save/update") || cascade.Equals("save-update")) {
						return Cascades.CascadeStyle.StyleSaveUpdate;
					} else if (cascade.Equals("delete")) {
						return Cascades.CascadeStyle.StyleOnlyDelete;
					} else {
						throw new MappingException("Unspported cascade style: " + cascade);
					}
				}
			}
		}

		public string Cascade {
			get { return cascade; }
			set { cascade = value; }
		}

		public bool IsInsertable {
			get { return insertable; }
			set { insertable = value; }
		}
	}
}