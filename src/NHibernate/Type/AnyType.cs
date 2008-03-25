using System;
using System.Collections;
using System.Data;
using System.Reflection;
using System.Xml;
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
			: this(NHibernateUtil.String, NHibernateUtil.Serializable)
		{
		}

		public override object DeepCopy(object value, EntityMode entityMode, ISessionFactoryImplementor factory)
		{
			return value;
		}

		public override int GetColumnSpan(IMapping session)
		{
			return 2;
		}

		public override string Name
		{
			get { return "Object"; }
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
			return ResolveAny((string)metaType.NullSafeGet(rs, names[0], session, owner), 
				identifierType.NullSafeGet(rs, names[1], session, owner), session);
		}

		public override object Hydrate(IDataReader rs, string[] names, ISessionImplementor session, object owner)
		{
			string entityName = (string)metaType.NullSafeGet(rs, names[0], session, owner);
			object id = identifierType.NullSafeGet(rs, names[1], session, owner);
			return new ObjectTypeCacheEntry(entityName, id);
		}

		public override object ResolveIdentifier(object value, ISessionImplementor session, object owner)
		{
			ObjectTypeCacheEntry holder = (ObjectTypeCacheEntry) value;
			return ResolveAny(holder.entityName, holder.id, session);
		}

		public override object SemiResolve(object value, ISessionImplementor session, object owner)
		{
			throw new NotSupportedException("any mappings may not form part of a property-ref");
		}

		public override void NullSafeSet(IDbCommand st, object value, int index, bool[] settable, ISessionImplementor session)
		{
			object id;
			string entityName;
			if (value == null)
			{
				id = null;
				entityName = null;
			}
			else
			{
				entityName = session.BestGuessEntityName(value);
				id = ForeignKeys.GetEntityIdentifierIfNotUnsaved(entityName, value, session);
			}

			// metaType is assumed to be single-column type
			if (settable == null || settable[0])
			{
				metaType.NullSafeSet(st, entityName, index, session);
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
			return ArrayHelper.Join(metaType.SqlTypes(mapping), identifierType.SqlTypes(mapping));
		}

		public override string ToLoggableString(object value, ISessionFactoryImplementor factory)
		{
			return value == null ? "null" :
				NHibernateUtil.Entity(NHibernateProxyHelper.GetClassWithoutInitializingProxy(value)).ToLoggableString(value, factory);
		}

		public override object FromXMLNode(XmlNode xml, IMapping factory)
		{
			// TODO NH: We can implement this method if the XML is the result of a serialization in XML
			throw new NotSupportedException(); //TODO: is this right??
		}

		[Serializable]
		public sealed class ObjectTypeCacheEntry
		{
			internal string entityName;
			internal object id;
			internal ObjectTypeCacheEntry(string entityName, object id)
			{
				this.entityName = entityName;
				this.id = id;
			}
		}

		public override object Assemble(object cached, ISessionImplementor session, object owner)
		{
			ObjectTypeCacheEntry e = cached as ObjectTypeCacheEntry;
			return (e == null) ? null : session.InternalLoad(e.entityName, e.id, false, false);
		}

		public override object Disassemble(object value, ISessionImplementor session, object owner)
		{
			return value == null ? null : 
				new ObjectTypeCacheEntry(session.BestGuessEntityName(value), 
				ForeignKeys.GetEntityIdentifierIfNotUnsaved(session.BestGuessEntityName(value), value, session));
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

		public CascadeStyle GetCascadeStyle(int i)
		{
			return CascadeStyle.None;
		}

		public FetchMode GetFetchMode(int i)
		{
			return FetchMode.Select;
		}

		public bool IsEmbedded
		{
			get { return false; }
		}

		public virtual bool IsEmbeddedInXML
		{
			get { return false; }
		}

		private static readonly string[] PROPERTY_NAMES = new string[] { "class", "id" };

		public string[] PropertyNames
		{
			get { return PROPERTY_NAMES; }
		}

		public object GetPropertyValue(Object component, int i, ISessionImplementor session)
		{
			return i == 0 ? session.BestGuessEntityName(component) : Id(component, session);
		}

		public object[] GetPropertyValues(Object component, EntityMode entityMode)
		{
			throw new NotSupportedException();
		}

		public object[] GetPropertyValues(object component, ISessionImplementor session)
		{
			return new object[] { session.BestGuessEntityName(component), Id(component, session) };
		}

		private static object Id(object component, ISessionImplementor session)
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

		public void SetPropertyValues(object component, object[] values, EntityMode entityMode)
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

		public override int GetHashCode()
		{
			return 1; // Originally: System.identityHashCode(this);
		}

		public bool IsAlwaysDirtyChecked
		{
			get { return false; }
		}

		public override bool IsDirty(object old, object current, bool[] checkable, ISessionImplementor session)
		{
			//TODO!!!
			return IsDirty(old, current, session);
		}

		public override bool IsModified(object old, object current, bool[] checkable, ISessionImplementor session)
		{
			if (current == null)
				return old != null;
			if (old == null)
				return current != null;
			ObjectTypeCacheEntry holder = (ObjectTypeCacheEntry)old;
			bool[] idcheckable = new bool[checkable.Length - 1];
			Array.Copy(checkable, 1, idcheckable, 0, idcheckable.Length);
			return (checkable[0] && !holder.entityName.Equals(session.BestGuessEntityName(current))) || 
				identifierType.IsModified(holder.id, Id(current, session), idcheckable, session);
		}

		public bool[] PropertyNullability
		{
			get { return null; }
		}

		public string GetOnCondition(string alias, ISessionFactoryImplementor factory, IDictionary<string, IFilter> enabledFilters)
		{
			throw new NotSupportedException();
		}

		public override int Compare(object x, object y, EntityMode? entityMode)
		{
			return 0; //TODO: entities CAN be compared, by PK and entity name, fix this!
		}

		public virtual bool IsMethodOf(MethodBase method)
		{
			return false;
		}

		public override bool IsSame(object x, object y, EntityMode entityMode)
		{
			return x == y;
		}

		public bool ReferenceToPrimaryKey
		{
			get { return true; }
		}

		private object ResolveAny(string entityName, object id, ISessionImplementor session)
		{
			return entityName == null || id == null ? null : session.InternalLoad(entityName, id, false, false);
		}

		public override void SetToXMLNode(XmlNode xml, object value, ISessionFactoryImplementor factory)
		{
			throw new NotSupportedException("any types cannot be stringified");
		}

		public override bool[] ToColumnNullness(object value, IMapping mapping)
		{
			bool[] result = new bool[GetColumnSpan(mapping)];
			if (value != null)
				ArrayHelper.Fill(result, true);
			return result;
		}

	}
}
