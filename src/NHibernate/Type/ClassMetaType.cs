using System;
using System.Data;
using System.Xml;
using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// ClassMetaType is a NH specific type to support "any" with meta-type="class"
	/// </summary>
	/// <remarks>
	/// It work like a MetaType where the key is the entity-name it self
	/// </remarks>
	[Serializable]
	public class ClassMetaType : AbstractType
	{
		public override SqlType[] SqlTypes(IMapping mapping)
		{
			return new SqlType[] { NHibernateUtil.String.SqlType };
		}

		public override int GetColumnSpan(IMapping mapping)
		{
			return 1;
		}

		public override System.Type ReturnedClass
		{
			get { return typeof (string); }
		}

		public override object NullSafeGet(IDataReader rs, string[] names, ISessionImplementor session, object owner)
		{
			return NullSafeGet(rs, names[0], session, owner);
		}

		public override object NullSafeGet(IDataReader rs,string name,ISessionImplementor session,object owner)
		{
			int index = rs.GetOrdinal(name);

			if (rs.IsDBNull(index))
			{
				return null;
			}
			else
			{
				string str = (string) NHibernateUtil.String.Get(rs, index);
				return string.IsNullOrEmpty(str) ? null : str;
			}
		}

		public override void NullSafeSet(IDbCommand st, object value, int index, bool[] settable, ISessionImplementor session)
		{
			if (settable[0]) NullSafeSet(st, value, index, session);
		}

		public override void NullSafeSet(IDbCommand st,object value,int index,ISessionImplementor session)
		{
			if (value == null)
			{
				((IDataParameter)st.Parameters[index]).Value = DBNull.Value;
			}
			else
			{
				NHibernateUtil.String.Set(st, value, index);
			}
		}

		public override string ToLoggableString(object value, ISessionFactoryImplementor factory)
		{
			return ToXMLString(value, factory);
		}

		public override string Name
		{
			get { return "ClassMetaType"; }
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
			throw new NotSupportedException();
		}

		public string ToXMLString(object value, ISessionFactoryImplementor factory)
		{
			return (string)value; //value is the entity name
		}
	}
}
