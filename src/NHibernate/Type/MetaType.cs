using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Xml;
using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	[Serializable]
	public class MetaType : AbstractType
	{
		private readonly IDictionary<object, string> values;
		private readonly IDictionary<string, object> keys;
		private readonly IType baseType;

		public MetaType(IDictionary<object, string> values, IType baseType)
		{
			this.baseType = baseType;
			this.values = values;
			keys = new Dictionary<string, object>();
			foreach (KeyValuePair<object, string> me in values)
			{
				keys[me.Value] = me.Key;
			}
		}

		public override SqlType[] SqlTypes(IMapping mapping)
		{
			return baseType.SqlTypes(mapping);
		}

		public override int GetColumnSpan(IMapping mapping)
		{
			return baseType.GetColumnSpan(mapping);
		}

		public override System.Type ReturnedClass
		{
			get { return typeof (string); }
		}

		public override object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner)
		{
			object key = baseType.NullSafeGet(rs, names, session, owner);
			return key == null ? null : values[key];
		}

		public override object NullSafeGet(DbDataReader rs,string name,ISessionImplementor session,object owner)
		{
			object key = baseType.NullSafeGet(rs, name, session, owner);
			return key == null ? null : values[key];
		}

		public override void NullSafeSet(DbCommand st, object value, int index, bool[] settable, ISessionImplementor session)
		{
			if (settable[0]) NullSafeSet(st, value, index, session);
		}

		public override void NullSafeSet(DbCommand st,object value,int index,ISessionImplementor session)
		{
			baseType.NullSafeSet(st, value == null ? null : keys[(string)value], index, session);
		}

		public override string ToLoggableString(object value, ISessionFactoryImplementor factory)
		{
			return ToXMLString(value, factory);
		}

		public override string Name
		{
			get { return baseType.Name; } //TODO!
		}

		public override object DeepCopy(object value, EntityMode entityMode, ISessionFactoryImplementor factory)
		{
			return value;
		}

		public override bool IsMutable
		{
			get { return false; }
		}

		public override bool IsDirty(object old, object current, bool[] checkable, ISessionImplementor session)
		{
			return checkable[0] && IsDirty(old, current, session);
		}

		public override object FromXMLNode(XmlNode xml, IMapping factory)
		{
			return FromXMLString(xml.Value, factory);
		}

		public object FromXMLString(string xml, IMapping factory)
		{
			return xml; //xml is the entity name
		}

		public override object Replace(object original, object current, ISessionImplementor session, object owner, System.Collections.IDictionary copiedAlready)
		{
			return original;
		}

		public override void SetToXMLNode(XmlNode node, object value, ISessionFactoryImplementor factory)
		{
			node.Value = ToXMLString(value, factory);
		}

		public override bool[] ToColumnNullness(object value, IMapping mapping)
		{
			return baseType.ToColumnNullness(value, mapping);
		}

		public string ToXMLString(object value, ISessionFactoryImplementor factory)
		{
			return (string)value; //value is the entity name
		}

		internal object GetMetaValue(string className)
		{
			return keys[className];
		}
	}
}