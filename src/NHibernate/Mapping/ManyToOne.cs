using System;
using NHibernate.Util;
using NHibernate.Type;

namespace NHibernate.Mapping {
	
	public class ManyToOne : Association {
		
		public ManyToOne(Table table) : base(table) { }

		public override void SetTypeByReflection(System.Type propertyClass, string propertyName) {
			try {
				if (Type==null)
					Type = TypeFactory.ManyToOne( ReflectHelper.GetGetter(propertyClass, propertyName).ReturnType);
			} catch (HibernateException he) {
				throw new MappingException("Problem trying to set association type by reflection", he);
			}
		}

		public override void CreateForeignKey() {
			CreateForeignKeyOfClass( ( (EntityType)Type).PersistentClass);
		}
	}
}
