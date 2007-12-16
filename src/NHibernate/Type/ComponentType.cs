using System;
using System.Collections;
using System.Data;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.Tuple;
using NHibernate.Tuple.Component;
using NHibernate.Util;

namespace NHibernate.Type
{
	[Serializable]
	public class ComponentType : AbstractType, IAbstractComponentType
	{
		private readonly System.Type componentClass;
		private readonly IType[] propertyTypes;
		private readonly string[] propertyNames;
		private readonly bool[] propertyNullability;
		private readonly int propertySpan;
		private readonly Cascades.CascadeStyle[] cascade;
		private readonly FetchMode?[] joinedFetch;
		private bool isKey;
		protected internal EntityModeToTuplizerMapping tuplizerMapping;

		public override SqlType[] SqlTypes(IMapping mapping)
		{
			//not called at runtime so doesn't matter if its slow :)
			SqlType[] sqlTypes = new SqlType[GetColumnSpan(mapping)];
			int n = 0;
			for (int i = 0; i < propertySpan; i++)
			{
				SqlType[] subtypes = propertyTypes[i].SqlTypes(mapping);
				for (int j = 0; j < subtypes.Length; j++)
				{
					sqlTypes[n++] = subtypes[j];
				}
			}
			return sqlTypes;
		}

		public override int GetColumnSpan(IMapping mapping)
		{
			int span = 0;
			for (int i = 0; i < propertySpan; i++)
			{
				span += propertyTypes[i].GetColumnSpan(mapping);
			}
			return span;
		}

		public ComponentType(ComponentMetamodel metamodel)
		{
			// "re-flatting" metamodel
			isKey = metamodel.IsKey;
			propertySpan = metamodel.PropertySpan;
			propertyNames = new string[propertySpan];
			propertyTypes = new IType[propertySpan];
			propertyNullability = new bool[propertySpan];
			cascade = new Cascades.CascadeStyle[propertySpan];
			joinedFetch = new FetchMode?[propertySpan];

			for (int i = 0; i < propertySpan; i++)
			{
				StandardProperty prop = metamodel.GetProperty(i);
				propertyNames[i] = prop.Name;
				propertyTypes[i] = prop.Type;
				propertyNullability[i] = prop.IsNullable;
				cascade[i] = prop.CascadeStyle;
				joinedFetch[i] = prop.FetchMode;
			}

			tuplizerMapping = metamodel.TuplizerMapping;
			componentClass = tuplizerMapping.GetTuplizer(EntityMode.Poco).MappedClass;
		}

		/// <summary></summary>
		public override bool IsCollectionType
		{
			get { return false; }
		}

		/// <summary></summary>
		public override bool IsComponentType
		{
			get { return true; }
		}

		/// <summary></summary>
		public override bool IsEntityType
		{
			get { return false; }
		}

		/// <summary></summary>
		public override System.Type ReturnedClass
		{
			get { return componentClass; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public override bool Equals(object x, object y)
		{

			if (x == y)
			{
				return true;
			}
			if (x == null || y == null)
			{
				return false;
			}
			object[] xvalues = GetPropertyValues(x);
			object[] yvalues = GetPropertyValues(y);
			for (int i = 0; i < propertySpan; i++)
			{
				if (!propertyTypes[i].Equals(xvalues[i], yvalues[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode(object x, ISessionFactoryImplementor factory)
		{
			unchecked
			{
				int result = 17;
				object[] values = GetPropertyValues(x);
				for (int i = 0; i < propertySpan; i++)
				{
					object y = values[i];
					result *= 37;
					if (y != null)
					{
						result += propertyTypes[i].GetHashCode(y, factory);
					}
				}
				return result;
			}
		}

		public override bool IsDirty(object x, object y, ISessionImplementor session)
		{
			if (x == y)
			{
				return false;
			}
			if (x == null || y == null)
			{
				return true;
			}
			object[] xvalues = GetPropertyValues(x);
			object[] yvalues = GetPropertyValues(y);
			for (int i = 0; i < xvalues.Length; i++)
			{
				if (propertyTypes[i].IsDirty(xvalues[i], yvalues[i], session))
				{
					return true;
				}
			}
			return false;
		}

		public override bool IsDirty(object x, object y, bool[] checkable, ISessionImplementor session)
		{
			if (x == y)
			{
				return false;
			}
			if (x == null || y == null)
			{
				return true;
			}

			object[] xvalues = GetPropertyValues(x);
			object[] yvalues = GetPropertyValues(y);
			int loc = 0;
			for (int i = 0; i < xvalues.Length; i++)
			{
				int len = propertyTypes[i].GetColumnSpan(session.Factory);
				if (len <= 1)
				{
					bool dirty = (len == 0 || checkable[loc]) &&
					             propertyTypes[i].IsDirty(xvalues[i], yvalues[i], session);
					if (dirty)
					{
						return true;
					}
				}
				else
				{
					bool[] subcheckable = new bool[len];
					Array.Copy(checkable, loc, subcheckable, 0, len);
					bool dirty = propertyTypes[i].IsDirty(xvalues[i], yvalues[i], subcheckable, session);
					if (dirty)
					{
						return true;
					}
				}
				loc += len;
			}
			return false;
		}

		public override object NullSafeGet(IDataReader rs, string[] names, ISessionImplementor session, object owner)
		{
			return ResolveIdentifier(Hydrate(rs, names, session, owner), session, owner);

			/*
			int begin = 0;
			bool notNull = false;
			object[ ] values = new object[propertySpan];
			for( int i = 0; i < propertySpan; i++ )
			{
				int length = propertyTypes[ i ].GetColumnSpan( session.Factory );
				string[ ] range = ArrayHelper.Slice( names, begin, length );
				object val = propertyTypes[ i ].NullSafeGet( rs, range, session, owner );
				if( val != null )
				{
					notNull = true;
				}
				values[ i ] = val;
				begin += length;
			}

			if( notNull )
			{
				object result = Instantiate( owner, session );
				for( int i = 0; i < propertySpan; i++ )
				{
					setters[ i ].Set( result, values[ i ] );
				}
				return result;
			}
			else
			{
				return null;
			}
			*/
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="st"></param>
		/// <param name="value"></param>
		/// <param name="begin"></param>
		/// <param name="session"></param>
		public override void NullSafeSet(IDbCommand st, object value, int begin, ISessionImplementor session)
		{
			object[] subvalues = NullSafeGetValues(value);

			for (int i = 0; i < propertySpan; i++)
			{
				propertyTypes[i].NullSafeSet(st, subvalues[i], begin, session);
				begin += propertyTypes[i].GetColumnSpan(session.Factory);
			}
		}

		public override void NullSafeSet(IDbCommand st, object value, int begin, bool[] settable, ISessionImplementor session)
		{
			object[] subvalues = NullSafeGetValues(value);

			int sqlParamIndex = begin;
			int loc = 0;
			for (int i = 0; i < propertySpan; i++)
			{
				int len = propertyTypes[i].GetColumnSpan(session.Factory);
				if (len == 0)
				{
					// noop
				}
				else if (len == 1)
				{
					if (settable[loc])
					{
						propertyTypes[i].NullSafeSet(st, subvalues[i], sqlParamIndex, session);
						sqlParamIndex++;
					}
				}
				else
				{
					bool[] subsettable = new bool[len];
					Array.Copy(settable, loc, subsettable, 0, len);
					propertyTypes[i].NullSafeSet(st, subvalues[i], sqlParamIndex, subsettable, session);
					sqlParamIndex += ArrayHelper.CountTrue(subsettable);
				}
				loc += len;
			}
		}

		private object[] NullSafeGetValues(object value)
		{
			if (value == null)
			{
				return new object[propertySpan];
			}
			else
			{
				return GetPropertyValues(value);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="name"></param>
		/// <param name="session"></param>
		/// <param name="owner"></param>
		/// <returns></returns>
		public override object NullSafeGet(IDataReader rs, string name, ISessionImplementor session, object owner)
		{
			return NullSafeGet(rs, new string[] {name}, session, owner);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="component"></param>
		/// <param name="i"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		public object GetPropertyValue(object component, int i, ISessionImplementor session)
		{
			return GetPropertyValue(component, i);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="component"></param>
		/// <param name="i"></param>
		/// <returns></returns>
		public object GetPropertyValue(object component, int i)
		{
			return tuplizerMapping.GetTuplizer(EntityMode.Poco).GetPropertyValue(component, i);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="component"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		public object[] GetPropertyValues(object component, ISessionImplementor session)
		{
			return GetPropertyValues(component);
		}

		/// <remarks>
		/// Use the access optimizer if available
		/// </remarks>
		public object[] GetPropertyValues(object component)
		{
			return tuplizerMapping.GetTuplizer(EntityMode.Poco).GetPropertyValues(component);
		}

		/// <remarks>
		/// Use the access optimizer if available
		/// </remarks>
		public void SetPropertyValues(object component, object[] values)
		{
			tuplizerMapping.GetTuplizer(EntityMode.Poco).SetPropertyValues(component, values);
		}

		/// <summary></summary>
		public IType[] Subtypes
		{
			get { return propertyTypes; }
		}

		/// <summary></summary>
		public override string Name
		{
			get { return componentClass.Name; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <param name="factory"></param>
		/// <returns></returns>
		public override string ToLoggableString(object value, ISessionFactoryImplementor factory)
		{
			if (value == null)
			{
				return "null";
			}
			IDictionary result = new Hashtable();
			object[] values = GetPropertyValues(value);

			for (int i = 0; i < propertyTypes.Length; i++)
			{
				result[propertyNames[i]] = propertyTypes[i].ToLoggableString(values[i], factory);
			}

			return StringHelper.Unqualify(Name) + CollectionPrinter.ToString(result);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public override object FromString(string xml)
		{
			throw new NotSupportedException();
		}

		/// <summary></summary>
		public string[] PropertyNames
		{
			get { return propertyNames; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="component"></param>
		/// <returns></returns>
		public override object DeepCopy(object component)
		{
			if (component == null)
			{
				return null;
			}

			object[] values = GetPropertyValues(component);
			for (int i = 0; i < propertySpan; i++)
			{
				values[i] = propertyTypes[i].DeepCopy(values[i]);
			}

			object result = Instantiate();
			SetPropertyValues(result, values);

			return result;
		}

		public override object Replace(object original, object target, ISessionImplementor session, object owner,
		                               IDictionary copiedAlready)
		{
			if (original == null)
			{
				return null;
			}

			if (original == target)
			{
				return target;
			}

			object result = target ?? Instantiate(owner, session);

			object[] values = TypeFactory.Replace(
				GetPropertyValues(original), GetPropertyValues(result),
				propertyTypes, session, owner, copiedAlready);

			SetPropertyValues(result, values);
			return result;
		}

		public override object Replace(object original, object target, ISessionImplementor session, object owner, IDictionary copyCache, ForeignKeyDirection foreignKeyDirection)
		{
			if (original == null)
			{
				return null;
			}
			//if ( original == target ) return target;

			object result = target ?? Instantiate(owner, session);

			object[] values = TypeFactory.Replace(GetPropertyValues(original), GetPropertyValues(result), 
				propertyTypes, session, owner, copyCache, foreignKeyDirection);

			SetPropertyValues(result, values);
			return result;
		}

		/// <remarks>
		/// This method does not populate the component parent
		/// </remarks>
		public object Instantiate()
		{
			try
			{
				return Instantiate(EntityMode.Poco);
			}
			catch (Exception e)
			{
				throw new InstantiationException("Could not instantiate component: ", e, componentClass);
			}
		}

		public object Instantiate(EntityMode entityMode)
		{
			return tuplizerMapping.GetTuplizer(entityMode).Instantiate();
		}

		public object Instantiate(object parent, ISessionImplementor session)
		{

			object result = Instantiate(session.EntityMode);

			IComponentTuplizer ct = (IComponentTuplizer)tuplizerMapping.GetTuplizer(session.EntityMode);
			if (ct.HasParentProperty && parent != null)
			{
				ct.SetParent(result, session.PersistenceContext.ProxyFor(parent), session.Factory);
			}

			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public Cascades.CascadeStyle GetCascadeStyle(int i)
		{
			return cascade[i];
		}

		/// <summary></summary>
		public override bool IsMutable
		{
			get { return true; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		public override object Disassemble(object value, ISessionImplementor session)
		{
			if (value == null)
			{
				return null;
			}
			else
			{
				object[] values = GetPropertyValues(value);
				for (int i = 0; i < propertyTypes.Length; i++)
				{
					values[i] = propertyTypes[i].Disassemble(values[i], session);
				}
				return values;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="session"></param>
		/// <param name="owner"></param>
		/// <returns></returns>
		public override object Assemble(object obj, ISessionImplementor session, object owner)
		{
			if (obj == null)
			{
				return null;
			}
			else
			{
				object[] values = (object[])obj;
				object[] assembled = new object[values.Length];
				for (int i = 0; i < propertyTypes.Length; i++)
				{
					assembled[i] = propertyTypes[i].Assemble(values[i], session, owner);
				}
				object result = Instantiate(owner, session);
				SetPropertyValues(result, assembled, session.EntityMode);
				return result;
			}
		}

		/// <summary></summary>
		public override bool HasNiceEquals
		{
			get { return false; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public FetchMode GetFetchMode(int i)
		{
			return joinedFetch[i].GetValueOrDefault();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="names"></param>
		/// <param name="session"></param>
		/// <param name="owner"></param>
		/// <returns></returns>
		public override object Hydrate(IDataReader rs, string[] names, ISessionImplementor session, object owner)
		{
			int begin = 0;
			bool notNull = false;
			object[] values = new object[propertySpan];
			for (int i = 0; i < propertySpan; i++)
			{
				int length = propertyTypes[i].GetColumnSpan(session.Factory);
				string[] range = ArrayHelper.Slice(names, begin, length); //cache this
				object val = propertyTypes[i].Hydrate(rs, range, session, owner);
				if(val==null)
				{
					if (isKey)
					{
						return null; //different nullability rules for pk/fk
					}
				}
				else
				{
					notNull = true;
				}
				values[i] = val;
				begin += length;
			}

			if (componentClass.IsValueType)
				return values;
			else
				return notNull ? values : null;
		}

		public override object ResolveIdentifier(object value, ISessionImplementor session, object owner)
		{
			if (value != null)
			{
				object result = Instantiate(owner, session);
				object[] values = (object[]) value;
				object[] resolvedValues = new object[values.Length]; //only really need new array during semiresolve!
				for (int i = 0; i < values.Length; i++)
				{
					resolvedValues[i] = propertyTypes[i].ResolveIdentifier(values[i], session, owner);
				}
				SetPropertyValues(result, resolvedValues);
				return result;
			}
			else
			{
				return null;
			}
		}

		public override object SemiResolve(object value, ISessionImplementor session, object owner)
		{
			return ResolveIdentifier(value, session, owner);
		}

		public override bool IsModified(object old, object current, bool[] checkable, ISessionImplementor session)
		{
			if (current == null)
			{
				return old != null;
			}
			if (old == null)
			{
				return current != null;
			}
			object[] currentValues = GetPropertyValues(current, session);
			object[] oldValues = (Object[]) old;
			int loc = 0;
			for (int i = 0; i < currentValues.Length; i++)
			{
				int len = propertyTypes[i].GetColumnSpan(session.Factory);
				bool[] subcheckable = new bool[len];
				Array.Copy(checkable, loc, subcheckable, 0, len);
				if (propertyTypes[i].IsModified(oldValues[i], currentValues[i], subcheckable, session))
				{
					return true;
				}
				loc += len;
			}
			return false;
		}

		public bool[] PropertyNullability
		{
			get { return propertyNullability; }
		}

		public virtual object[] GetPropertyValues(object component, EntityMode entityMode)
		{
			return tuplizerMapping.GetTuplizer(entityMode).GetPropertyValues(component);
		}

		public virtual void SetPropertyValues(object component, object[] values, EntityMode entityMode)
		{
			tuplizerMapping.GetTuplizer(entityMode).SetPropertyValues(component, values);
		}

	}
}