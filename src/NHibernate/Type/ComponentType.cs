using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Xml;
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
		private readonly IType[] propertyTypes;
		private readonly string[] propertyNames;
		private readonly bool[] propertyNullability;
		private readonly int propertySpan;
		private readonly CascadeStyle[] cascade;
		private readonly FetchMode?[] joinedFetch;
		private readonly bool isKey;
		protected internal EntityModeToTuplizerMapping tuplizerMapping;
		private bool overridesGetHashCode;

		public override SqlType[] SqlTypes(IMapping mapping)
		{
			//Not called at runtime so doesn't matter if its slow :)
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
			// for now, just "re-flatten" the metamodel since this is temporary stuff anyway (HHH-1907)
			isKey = metamodel.IsKey;
			propertySpan = metamodel.PropertySpan;
			propertyNames = new string[propertySpan];
			propertyTypes = new IType[propertySpan];
			propertyNullability = new bool[propertySpan];
			cascade = new CascadeStyle[propertySpan];
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
			var tuplizer = tuplizerMapping.GetTuplizerOrNull(EntityMode.Poco);
			if(tuplizer !=null)
			{
				overridesGetHashCode = ReflectHelper.OverridesGetHashCode(tuplizer.MappedClass);
			}
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
			get { return tuplizerMapping.GetTuplizer(EntityMode.Poco).MappedClass; }
		}

		public override int GetHashCode(object x, EntityMode entityMode, ISessionFactoryImplementor factory)
		{
			if (overridesGetHashCode)
				return x.GetHashCode();
			return GetHashCode(x, entityMode);
		}

		public override int GetHashCode(object x, EntityMode entityMode)
		{
			int result = 17;
			object[] values = GetPropertyValues(x, entityMode);
			unchecked
			{
				for (int i = 0; i < propertySpan; i++)
				{
					object y = values[i];
					result *= 37;
					if (y != null)
						result += propertyTypes[i].GetHashCode(y, entityMode);
				}
			}
			return result;
		}

		public override bool IsDirty(object x, object y, ISessionImplementor session)
		{
			if (x == y)
			{
				return false;
			}
			/* 
			 * NH Different behavior : we don't use the shortcut because NH-1101 
			 * let the tuplizer choose how cosiderer properties when the component is null.
			 */
			EntityMode entityMode = session.EntityMode;
			if (entityMode != EntityMode.Poco && (x == null || y == null))
			{
				return true;
			}
			object[] xvalues = GetPropertyValues(x, entityMode);
			object[] yvalues = GetPropertyValues(y, entityMode);
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
			/* 
			 * NH Different behavior : we don't use the shortcut because NH-1101 
			 * let the tuplizer choose how cosiderer properties when the component is null.
			 */
			EntityMode entityMode = session.EntityMode;
			if (entityMode != EntityMode.Poco && (x == null || y == null))
			{
				return true;
			}
			object[] xvalues = GetPropertyValues(x, entityMode);
			object[] yvalues = GetPropertyValues(y, entityMode);
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
			object[] subvalues = NullSafeGetValues(value, session.EntityMode);

			for (int i = 0; i < propertySpan; i++)
			{
				propertyTypes[i].NullSafeSet(st, subvalues[i], begin, session);
				begin += propertyTypes[i].GetColumnSpan(session.Factory);
			}
		}

		public override void NullSafeSet(IDbCommand st, object value, int begin, bool[] settable, ISessionImplementor session)
		{
			object[] subvalues = NullSafeGetValues(value, session.EntityMode);

			int loc = 0;
			for (int i = 0; i < propertySpan; i++)
			{
				int len = propertyTypes[i].GetColumnSpan(session.Factory);
				if (len == 0)
				{
					//noop
				}
				else if (len == 1)
				{
					if (settable[loc])
					{
						propertyTypes[i].NullSafeSet(st, subvalues[i], begin, session);
						begin++;
					}
				}
				else
				{
					bool[] subsettable = new bool[len];
					Array.Copy(settable, loc, subsettable, 0, len);
					propertyTypes[i].NullSafeSet(st, subvalues[i], begin, subsettable, session);
					begin += ArrayHelper.CountTrue(subsettable);
				}
				loc += len;
			}
		}

		private object[] NullSafeGetValues(object value, EntityMode entityMode)
		{
			if (value == null)
			{
				return new object[propertySpan];
			}
			else
			{
				return GetPropertyValues(value, entityMode);
			}
		}

		public override object NullSafeGet(IDataReader rs, string name, ISessionImplementor session, object owner)
		{
			return NullSafeGet(rs, new string[] {name}, session, owner);
		}

		public object GetPropertyValue(object component, int i, EntityMode entityMode)
		{
			return tuplizerMapping.GetTuplizer(entityMode).GetPropertyValue(component, i);
		}

		public object GetPropertyValue(object component, int i, ISessionImplementor session)
		{
			return GetPropertyValue(component, i, session.EntityMode);
		}

		public object[] GetPropertyValues(object component, EntityMode entityMode)
		{
			return tuplizerMapping.GetTuplizer(entityMode).GetPropertyValues(component);
		}

		public object[] GetPropertyValues(object component, ISessionImplementor session)
		{
			return GetPropertyValues(component, session.EntityMode);
		}

		public virtual void SetPropertyValues(object component, object[] values, EntityMode entityMode)
		{
			tuplizerMapping.GetTuplizer(entityMode).SetPropertyValues(component, values);
		}

		/// <summary></summary>
		public IType[] Subtypes
		{
			get { return propertyTypes; }
		}

		/// <summary></summary>
		public override string Name
		{
			get { return "component" + ArrayHelper.ToString(propertyNames); }
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
			IDictionary<string, string> result = new Dictionary<string, string>();
			EntityMode? entityMode = tuplizerMapping.GuessEntityMode(value);
			if (!entityMode.HasValue)
			{
				throw new InvalidCastException(value.GetType().FullName);
			}
			object[] values = GetPropertyValues(value, entityMode.Value);
			for (int i = 0; i < propertyTypes.Length; i++)
			{
				result[propertyNames[i]] = propertyTypes[i].ToLoggableString(values[i], factory);
			}
			return StringHelper.Unqualify(Name) + CollectionPrinter.ToString(result);
		}

		/// <summary></summary>
		public string[] PropertyNames
		{
			get { return propertyNames; }
		}

		public override object DeepCopy(object component, EntityMode entityMode, ISessionFactoryImplementor factory)
		{
			if (component == null)
			{
				return null;
			}

			object[] values = GetPropertyValues(component, entityMode);
			for (int i = 0; i < propertySpan; i++)
			{
				values[i] = propertyTypes[i].DeepCopy(values[i], entityMode, factory);
			}

			object result = Instantiate(entityMode);
			SetPropertyValues(result, values, entityMode);

			//not absolutely necessary, but helps for some
			//equals()/hashCode() implementations
			IComponentTuplizer ct = (IComponentTuplizer)tuplizerMapping.GetTuplizer(entityMode);
			if (ct.HasParentProperty)
			{
				ct.SetParent(result, ct.GetParent(component), factory);
			}

			return result;
		}

		public override object Replace(object original, object target, ISessionImplementor session, object owner,
									   IDictionary copiedAlready)
		{
			if (original == null)
				return null;

			object result = target ?? Instantiate(owner, session);

			EntityMode entityMode = session.EntityMode;
			object[] values = TypeHelper.Replace(GetPropertyValues(original, entityMode), GetPropertyValues(result, entityMode), propertyTypes, session, owner, copiedAlready);

			SetPropertyValues(result, values, entityMode);
			return result;
		}

		public override object Replace(object original, object target, ISessionImplementor session, object owner, IDictionary copyCache, ForeignKeyDirection foreignKeyDirection)
		{
			if (original == null)
				return null;

			object result = target ?? Instantiate(owner, session);

			EntityMode entityMode = session.EntityMode;
			object[] values = TypeHelper.Replace(GetPropertyValues(original, entityMode), GetPropertyValues(result, entityMode), propertyTypes, session, owner, copyCache, foreignKeyDirection);

			SetPropertyValues(result, values, entityMode);
			return result;
		}

		/// <summary> This method does not populate the component parent</summary>
		public object Instantiate(EntityMode entityMode)
		{
			return tuplizerMapping.GetTuplizer(entityMode).Instantiate();
		}

		public virtual object Instantiate(object parent, ISessionImplementor session)
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
		public CascadeStyle GetCascadeStyle(int i)
		{
			return cascade[i];
		}

		/// <summary></summary>
		public override bool IsMutable
		{
			get { return true; }
		}

		public override object Disassemble(object value, ISessionImplementor session, object owner)
		{
			if (value == null)
			{
				return null;
			}
			else
			{
				object[] values = GetPropertyValues(value, session.EntityMode);
				for (int i = 0; i < propertyTypes.Length; i++)
				{
					values[i] = propertyTypes[i].Disassemble(values[i], session, owner);
				}
				return values;
			}
		}

		public override object Assemble(object obj, ISessionImplementor session, object owner)
		{
			if (obj == null)
			{
				return null;
			}
			else
			{
				object[] values = (object[]) obj;
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

		public FetchMode GetFetchMode(int i)
		{
			return joinedFetch[i].GetValueOrDefault();
		}

		public virtual bool IsEmbedded
		{
			get { return false; }
		}

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
				if (val == null)
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

			if (ReturnedClass.IsValueType)
				return values;
			else
				return notNull ? values : null;
		}

		public override object ResolveIdentifier(object value, ISessionImplementor session, object owner)
		{
			if (value != null)
			{
				object result = Instantiate(owner, session);
				object[] values = (object[])value;
				object[] resolvedValues = new object[values.Length]; //only really need new array during semiresolve!
				for (int i = 0; i < values.Length; i++)
				{
					resolvedValues[i] = propertyTypes[i].ResolveIdentifier(values[i], session, owner);
				}
				SetPropertyValues(result, resolvedValues, session.EntityMode);
				return result;
			}
			else
			{
				return null;
			}
		}

		public override object SemiResolve(object value, ISessionImplementor session, object owner)
		{
			//note that this implementation is kinda broken
			//for components with many-to-one associations
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

		public override int Compare(object x, object y, EntityMode? entityMode)
		{
			if (x == y)
			{
				return 0;
			}
			object[] xvalues = GetPropertyValues(x, entityMode.GetValueOrDefault());
			object[] yvalues = GetPropertyValues(y, entityMode.GetValueOrDefault());
			for (int i = 0; i < propertySpan; i++)
			{
				int propertyCompare = propertyTypes[i].Compare(xvalues[i], yvalues[i], entityMode);
				if (propertyCompare != 0)
					return propertyCompare;
			}
			return 0;
		}

		public override object FromXMLNode(XmlNode xml, IMapping factory)
		{
			return xml;
		}

		public override bool IsEqual(object x, object y, EntityMode entityMode)
		{
			if (x == y)
			{
				return true;
			}
			if (x == null || y == null)
			{
				return false;
			}
			object[] xvalues = GetPropertyValues(x, entityMode);
			object[] yvalues = GetPropertyValues(y, entityMode);
			for (int i = 0; i < propertySpan; i++)
			{
				if (!propertyTypes[i].IsEqual(xvalues[i], yvalues[i], entityMode))
				{
					return false;
				}
			}
			return true;
		}

		public override bool IsEqual(object x, object y, EntityMode entityMode, ISessionFactoryImplementor factory)
		{
			if (x == y)
			{
				return true;
			}
			if (x == null || y == null)
			{
				return false;
			}
			object[] xvalues = GetPropertyValues(x, entityMode);
			object[] yvalues = GetPropertyValues(y, entityMode);
			for (int i = 0; i < propertySpan; i++)
			{
				if (!propertyTypes[i].IsEqual(xvalues[i], yvalues[i], entityMode, factory))
				{
					return false;
				}
			}
			return true;
		}

		public virtual bool IsMethodOf(MethodBase method)
		{
			return false;
		}

		public override bool IsSame(object x, object y, EntityMode entityMode)
		{
			if (x == y)
			{
				return true;
			}
			if (x == null || y == null)
			{
				return false;
			}
			object[] xvalues = GetPropertyValues(x, entityMode);
			object[] yvalues = GetPropertyValues(y, entityMode);
			for (int i = 0; i < propertySpan; i++)
			{
				if (!propertyTypes[i].IsSame(xvalues[i], yvalues[i], entityMode))
				{
					return false;
				}
			}
			return true;
		}

		public override void SetToXMLNode(XmlNode node, object value, ISessionFactoryImplementor factory)
		{
			ReplaceNode(node, (XmlNode)value);
		}

		public override bool[] ToColumnNullness(object value, IMapping mapping)
		{
			bool[] result = new bool[GetColumnSpan(mapping)];
			if (value == null)
			{
				return result;
			}
			object[] values = GetPropertyValues(value, EntityMode.Poco);
			int loc = 0;
			for (int i = 0; i < propertyTypes.Length; i++)
			{
				bool[] propertyNullness = propertyTypes[i].ToColumnNullness(values[i], mapping);
				Array.Copy(propertyNullness, 0, result, loc, propertyNullness.Length);
				loc += propertyNullness.Length;
			}
			return result;
		}

		public override bool IsXMLElement
		{
			get { return true; }
		}

		public int GetPropertyIndex(string name)
		{
			string[] names = PropertyNames;
			for (int i = 0; i < names.Length; i++)
			{
				if (names[i].Equals(name))
				{
					return i;
				}
			}
			throw new PropertyNotFoundException(ReturnedClass, name);
		}
	}
}