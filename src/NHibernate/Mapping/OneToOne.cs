using System;
using System.Collections;
using NHibernate.Util;
using NHibernate.Type;

namespace NHibernate.Mapping {
	
	public class OneToOne : Association {
		private bool constrained;
		private ForeignKeyType foreignKeyType;
		private Value identifier;

		public OneToOne(Table table, Value identifier) : base(table) {
			this.identifier = identifier;
		}

		public override void SetTypeByReflection(System.Type propertyClass, string propertyName) {
			try {
				if (Type==null)
					Type = TypeFactory.OneToOne(ReflectHelper.GetGetter(propertyClass, propertyName).ReturnType, foreignKeyType);
			} catch (HibernateException he) {
				throw new MappingException("Problem trying to set association type by reflection", he);
			}
		}

		public override void CreateForeignKey() {
			if (constrained) CreateForeignKeyOfClass( ((EntityType)Type).PersistentClass );
		}

		public override IList ConstraintColumns {
			get {
				ArrayList list = new ArrayList();
				foreach(object obj in identifier.ColumnCollection) {
					list.Add(obj);
				}
				return list;
			}
		}

		public bool IsConstrained {
			get { return constrained; }
			set { constrained = value; }
		}

		public ForeignKeyType ForeignKeyType {
			get { return foreignKeyType; }
			set { foreignKeyType = value; }
		}

		public Value Identifier {
			get { return identifier; }
			set { identifier = value; }
		}
				
		
	}
}
