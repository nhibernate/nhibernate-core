using System;
using System.Data;

using NHibernate.Sql;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Type {

	/// <summary>
	/// A one-to-one association to an entity
	/// </summary>
	public class OneToOneType : EntityType, IAssociationType {

		private static readonly DbType[] NoSqlTypes = new DbType[0];
		private readonly ForeignKeyType foreignKeyType;
	
		public override int GetColumnSpan(IMapping session) {
			return 0;
		}

		public override DbType[] SqlTypes(IMapping session) {
			return NoSqlTypes;
		}
	
		public OneToOneType(System.Type persistentClass, ForeignKeyType foreignKeyType) : base(persistentClass) {
			this.foreignKeyType = foreignKeyType;
		}
	
		public override void NullSafeSet(IDbCommand cmd, object value, int index, ISessionImplementor session) {
			//nothing to do
		}
	
		public override bool IsOneToOne {
			get { return true; }
		}
		
		public override bool IsDirty(object old, object current, ISessionImplementor session) {
			return false;
		}
	
		public virtual ForeignKeyType ForeignKeyType {
			get { return foreignKeyType; }
		}
	
		public override object Hydrate(IDataReader rs, string[] names, ISessionImplementor session, object owner) {
			return session.GetEntityIdentifier(owner);
		}
	
		public override object ResolveIdentifier(object value, ISessionImplementor session, Object owner) {
			if (value==null) return null;
		
			System.Type clazz = PersistentClass;
			
			return IsNullable ?
			session.InternalLoadOneToOne(clazz, value) :
			session.InternalLoad(clazz, value);
		}
	
		public virtual bool IsNullable {
			get { return foreignKeyType==ForeignKeyType.ForeignKeyToParent; }
		}
		
	}
}

/*
//$Id$
package net.sf.hibernate.type;

import java.io.Serializable;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;

import net.sf.hibernate.HibernateException;
import net.sf.hibernate.MappingException;
import net.sf.hibernate.engine.Mapping;
import net.sf.hibernate.engine.SessionImplementor;



public class OneToOneType extends EntityType implements AssociationType {
	
												 private static final int[] NO_INTS = new int[0];
	private final AssociationType.ForeignKeyType foreignKeyType;
	
	public int getColumnSpan(Mapping session) throws MappingException {
												  return 0;
}
	
	
public int[] sqlTypes(Mapping session) throws MappingException {
return NO_INTS;
}
	
public OneToOneType(Class persistentClass, AssociationType.ForeignKeyType foreignKeyType) {
super(persistentClass);
this.foreignKeyType = foreignKeyType;
}
	
public void nullSafeSet(PreparedStatement st, Object value, int index, SessionImplementor session) throws HibernateException, SQLException {
		//nothing to do
}
	
public boolean isOneToOne() {
return true;
}
	
	
public boolean isDirty(Object old, Object current, SessionImplementor session) throws HibernateException {
return false;
}
	
public AssociationType.ForeignKeyType getForeignKeyType() {
return foreignKeyType;
}
	
public Object hydrate(
ResultSet rs,
String[] names,
SessionImplementor session,
Object owner
) throws HibernateException, SQLException {
			
return session.getEntityIdentifier(owner);
}
	
public Object resolveIdentifier(Object value, SessionImplementor session, Object owner) throws HibernateException, SQLException {
		
if (value==null) return null;
		
Class clazz = getPersistentClass();
Serializable id = (Serializable) value;
		
return isNullable() ?
session.internalLoadOneToOne(clazz, id) :
session.internalLoad(clazz, id);
}
	
public boolean isNullable() {
return foreignKeyType==AssociationType.FOREIGN_KEY_TO_PARENT;
}
	
}*/