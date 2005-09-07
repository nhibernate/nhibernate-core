using System;
using System.Collections;
using System.Data;

using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.Type;

namespace NHibernate.Type
{
	public class MetaType : AbstractType
	{
		private readonly IDictionary values;
		private readonly IDictionary keys;
		private readonly IType baseType;
	
		public MetaType(IDictionary values, IType baseType) 
		{
			this.baseType = baseType;
			this.values = values;
			this.keys = new Hashtable();
			foreach( DictionaryEntry me in values )
			{
				keys[ me.Value ] = me.Key;
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
			get { return typeof( System.Type ); }
		}

		public override bool Equals(object x, object y) 
		{
			return NHibernateUtil.Class.Equals(x, y);
		}

		public override object NullSafeGet(
			IDataReader rs,
			string[] names,
			ISessionImplementor session,
			object owner)
			
		{
			object key = baseType.NullSafeGet(rs, names, session, owner);
			return key==null ? null : values[ key ];
		}

		public override object NullSafeGet(
			IDataReader rs,
			string name,
			ISessionImplementor session,
			object owner)
			
		{
			object key = baseType.NullSafeGet(rs, name, session, owner);
			return key==null ? null : values[ key ];
		}

		public override void NullSafeSet(
			IDbCommand st,
			object value,
			int index,
			ISessionImplementor session)
	
		{
			baseType.NullSafeSet(st, value==null ? null : keys[ value ], index, session);
		}

		public override string ToString(object value, ISessionFactoryImplementor factory)
	
		{
			return NHibernateUtil.Class.ToString(value, factory);
		}

		public override object FromString(string xml)
	
		{
			return NHibernateUtil.Class.FromString(xml);
		}

		public override string Name
		{
			get { return baseType.Name; } //TODO!
		}

		public override object DeepCopy(object value) 
		{
			return NHibernateUtil.Class.DeepCopy(value);
		}

		public override bool IsMutable
		{
			get { return false; }
		}

		public override bool HasNiceEquals
		{
			get { return true; }
		}
	}
}