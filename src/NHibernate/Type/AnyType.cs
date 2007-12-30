using System;
using System.Collections;
using System.Data;
using System.Reflection;
using NHibernate.Engine;
using NHibernate.Persister.Entity;
using NHibernate.Proxy;
using NHibernate.SqlTypes;
using NHibernate.Util;
using System.Collections.Generic;

namespace NHibernate.Type
{
	///<summary>
	///	Handles "any" mappings and the old deprecated "object" type.
	///</summary>
	///<remarks>
	///	The identifierType is any NHibernate IType that can be serailized by default.
	///	For example, you can specify the identifierType as an Int32 or a custom identifier
	///	type that you built.  The identifierType matches to one or many columns.
	///	
	///	The metaType maps to a single column.  By default it stores the name of the Type
	///	that the Identifier identifies.  
	///	
	///	For example, we can store a link to any table.  It will have the results
	///	class_name					id_col1
	///	========================================
	///	Simple, AssemblyName			5
	///	DiffClass, AssemblyName			5
	///	Simple, AssemblyName			4
	///	
	///	You can also provide you own type that might map the name of the class to a table
	///	with a giant switch statemet or a good naming convention for your class->table.  The
	///	data stored might look like
	///	class_name					id_col1
	///	========================================
	///	simple_table					5
	///	diff_table						5
	///	simple_table					4
	///	
	///</remarks>
	[Serializable]
	public class AnyType : AbstractType, IAbstractComponentType, IAssociationType
	{
		private readonly IType identifierType;
		private readonly IType metaType;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="metaType"></param>
		/// <param name="identifierType"></param>
		internal AnyType(IType metaType, IType identifierType)
		{
			this.identifierType = identifierType;
			this.metaType = metaType;
		}

		/// <summary></summary>
		internal AnyType()
			: this(NHibernateUtil.Class, NHibernateUtil.Serializable)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public override object DeepCopy(object value)
		{
			return value;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public override bool Equals(object x, object y)
		{
			return x == y;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		/// <returns></returns>
		public override int GetColumnSpan(IMapping session)
		{
			/*
			 * This is set at 2 in Hibernate to support the old depreciated
			 * version of using type="object".  We are removing that support so
			 * I don't know if we need to keep this in.
			 */
			return 2;
		}

		public override string Name
		{
			get { return "Object"; }
		}

		public override bool HasNiceEquals
		{
			get { return false; }
		}

		public override bool IsMutable
		{
			get { return false; }
		}

		public override object NullSafeGet(IDataReader rs, string name, ISessionImplementor session, object owner)
		{
			throw new NotSupportedException("object is a multicolumn type");
		}

		public override object NullSafeGet(IDataReader rs, string[] names, ISessionImplementor session, object owner)
		{
			return Resolve(
				(System.Type) metaType.NullSafeGet(rs, names[0], session, owner),
				identifierType.NullSafeGet(rs, names[1], session, owner),
				session);
		}

		public override object Hydrate(IDataReader rs, string[] names, ISessionImplementor session, object owner)
		{
			System.Type clazz = (System.Type) metaType.NullSafeGet(rs, names[0], session, owner);
			object id = identifierType.NullSafeGet(rs, names[1], session, owner);
			return new ObjectTypeCacheEntry(clazz, id);
		}

		public override object ResolveIdentifier(object value, ISessionImplementor session, object owner)
		{
			ObjectTypeCacheEntry holder = (ObjectTypeCacheEntry) value;
			return Resolve(holder.clazz, holder.id, session);
		}

		public override object SemiResolve(object value, ISessionImplementor session, object owner)
		{
			throw new NotSupportedException("any mappings may not form part of a property-ref");
		}

		private object Resolve(System.Type clazz, object id, ISessionImplementor session)
		{
			return (clazz == null || id == null) ?
			       null :
			       session.InternalLoad(clazz.FullName, id, false, false);
		}

		public override void NullSafeSet(IDbCommand st, object value, int index, bool[] settable, ISessionImplementor session)
		{
			object id;
			string entityName;
			System.Type clazz; // TODO entityName : remove 

			if (value == null)
			{
				id = null;
				entityName = null;
				clazz = null;
			}
			else
			{
				entityName = session.BestGuessEntityName(value);
				id = ForeignKeys.GetEntityIdentifierIfNotUnsaved(entityName, value, session);
				clazz = NHibernateProxyHelper.GuessClass(value);
			}

			// metaType is assumed to be single-column type
			if (settable == null || settable[0])
			{
				metaType.NullSafeSet(st, clazz, index, session);
			}

			if (settable == null)
			{
				identifierType.NullSafeSet(st, id, index + 1, session);
			}
			else
			{
				bool[] idsettable = new bool[settable.Length - 1];
				Array.Copy(settable, 1, idsettable, 0, idsettable.Length);
				identifierType.NullSafeSet(st, id, index + 1, idsettable, session);
			}
		}

		public override void NullSafeSet(IDbCommand st, object value, int index, ISessionImplementor session)
		{
			NullSafeSet(st, value, index, null, session);
		}

		public override System.Type ReturnedClass
		{
			get { return typeof(object); }
		}

		public override SqlType[] SqlTypes(IMapping mapping)
		{
			return ArrayHelper.Join(
				metaType.SqlTypes(mapping),
				identifierType.SqlTypes(mapping));
		}

		public override string ToLoggableString(object value, ISessionFactoryImplementor factory)
		{
			return value == null ?
			       "null" :
			       NHibernateUtil.Entity(NHibernateProxyHelper.GuessClass(value))
			       	.ToLoggableString(value, factory);
		}

		public override object FromString(string xml)
		{
			throw new NotSupportedException(); //TODO: is this right??
		}

		[Serializable]
		public sealed class ObjectTypeCacheEntry
		{
			public System.Type clazz;
			public object id;

			public ObjectTypeCacheEntry(System.Type clazz, object id)
			{
				this.clazz = clazz;
				this.id = id;
			}
		}

		public override object Assemble(object cached, ISessionImplementor session, object owner)
		{
			ObjectTypeCacheEntry e = (ObjectTypeCacheEntry) cached;
			return (cached == null) ? null : session.InternalLoad(e.clazz.FullName, e.id, false, false);
		}

		public override object Disassemble(object value, ISessionImplementor session)
		{
			return (value == null) ?
			       null :
			       new ObjectTypeCacheEntry(
			       	NHibernateProxyHelper.GuessClass(value),
			       	session.GetContextEntityIdentifier(value));
		}

		public override object Replace(object original, object current, ISessionImplementor session, object owner,
		                               IDictionary copiedAlready)
		{
			if (original == null)
			{
				return null;
			}
			else
			{
				string entityName = session.BestGuessEntityName(original);
				object id = ForeignKeys.GetEntityIdentifierIfNotUnsaved(entityName, original, session);
				return session.InternalLoad(entityName, id, false, false);
			}
		}

		public override bool IsAnyType
		{
			get { return true; }
		}

		public Cascades.CascadeStyle GetCascadeStyle(int i)
		{
			return Cascades.CascadeStyle.StyleNone;
		}

		public FetchMode GetFetchMode(int i)
		{
			return FetchMode.Select;
		}

		public bool IsEmbedded
		{
			get { return false; }
		}

		public bool IsMethodOf(MethodInfo method)
		{
			return false;
		}

		private static readonly string[] PROPERTY_NAMES = new string[] {"class", "id"};

		public string[] PropertyNames
		{
			get { return PROPERTY_NAMES; }
		}

		public object GetPropertyValue(Object component, int i, ISessionImplementor session)
		{
			return (i == 0) ?
			       NHibernateProxyHelper.GuessClass(component) :
			       GetIdentifier(component, session);
		}

		public object[] GetPropertyValues(Object component, ISessionImplementor session)
		{
			return new object[] { session.BestGuessEntityName(component), GetIdentifier(component, session) };
		}

		public object[] GetPropertyValues(object component, EntityMode entityMode)
		{
			throw new NotSupportedException();
		}

		public void SetPropertyValues(object component, object[] values, EntityMode entityMode)
		{
			throw new NotSupportedException();
		}

		private object GetIdentifier(object component, ISessionImplementor session)
		{
			try
			{
				return ForeignKeys.GetEntityIdentifierIfNotUnsaved(session.BestGuessEntityName(component), component, session);
			}
			catch (TransientObjectException)
			{
				return null;
			}
		}

		public IType[] Subtypes
		{
			get { return new IType[] {metaType, identifierType}; }
		}

		public void SetPropertyValues(object component, object[] values)
		{
			throw new NotSupportedException();
		}

		public override bool IsComponentType
		{
			get { return true; }
		}

		public ForeignKeyDirection ForeignKeyDirection
		{
			get
			{
				//return AssociationType.FOREIGN_KEY_TO_PARENT; //TODO: this is better but causes a transient object exception... 
				return ForeignKeyDirection.ForeignKeyFromParent;
			}
		}

		public override bool IsAssociationType
		{
			get { return true; }
		}

		/// <summary>
		/// Not really relevant to AnyType, since it cannot be "joined"
		/// </summary>
		public bool UseLHSPrimaryKey
		{
			get { return false; }
		}

		public IJoinable GetAssociatedJoinable(ISessionFactoryImplementor factory)
		{
			throw new InvalidOperationException("any types do not have a unique referenced persister");
		}

		public string[] GetReferencedColumns(ISessionFactoryImplementor factory)
		{
			throw new InvalidOperationException("any types do not have unique referenced columns");
		}

		public System.Type GetAssociatedClass(ISessionFactoryImplementor factory)
		{
			throw new InvalidOperationException("any types do not have a unique referenced persister");
		}

		public string GetAssociatedEntityName(ISessionFactoryImplementor factory)
		{
			throw new InvalidOperationException("any types do not have a unique referenced persister");
		}

		public string LHSPropertyName
		{
			get { return null; }
		}

		public string RHSUniqueKeyPropertyName
		{
			get { return null; }
		}

		public override bool Equals(object obj)
		{
			return this == obj;
		}

		public override int GetHashCode()
		{
			return 1; // Originally: System.identityHashCode(this);
		}

		public bool IsAlwaysDirtyChecked
		{
			get { return false; }
		}

		public override bool IsDirty(object old, object current, ISessionImplementor session)
		{
			if (current == null)
			{
				return old != null;
			}

			if (old == null)
			{
				return true;
			}

			return (NHibernateProxyHelper.GetClassWithoutInitializingProxy(old) != NHibernateProxyHelper.GuessClass(current)) ||
				   identifierType.IsDirty(GetIdentifier(old, session), GetIdentifier(current, session), session);
		}

		public override bool IsDirty(object old, object current, bool[] checkable, ISessionImplementor session)
		{
			if (current == null)
			{
				return old != null;
			}
			
			if (old == null)
			{
				return true;
			}

			bool[] idcheckable = new bool[checkable.Length - 1];
			Array.Copy(checkable, 1, idcheckable, 0, idcheckable.Length);
			return (checkable[0] && NHibernateProxyHelper.GetClassWithoutInitializingProxy(old) != NHibernateProxyHelper.GuessClass(current)) ||
				   identifierType.IsDirty(GetIdentifier(old, session), GetIdentifier(current, session), idcheckable, session);
		}

		public override bool IsModified(object old, object current, bool[] checkable, ISessionImplementor session)
		{
			if (current == null)
			{
				return old != null;
			}
			if (old == null)
			{
				return true;
			}
			ObjectTypeCacheEntry holder = (ObjectTypeCacheEntry) old;
			bool[] idcheckable = new bool[checkable.Length - 1];
			Array.Copy(checkable, 1, idcheckable, 0, idcheckable.Length);
			return (checkable[0] && holder.clazz != NHibernateProxyHelper.GuessClass(current)) ||
			       identifierType.IsModified(holder.id, GetIdentifier(current, session), idcheckable, session);
		}

		public bool[] PropertyNullability
		{
			get { return null; }
		}

		public string GetOnCondition(string alias, ISessionFactoryImplementor factory, IDictionary<string, IFilter> enabledFilters)
		{
			throw new NotSupportedException();
		}

		public bool IsEmbeddedInXML
		{
			get { return false; }
		}

	}
}
