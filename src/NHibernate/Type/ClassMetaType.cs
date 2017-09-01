using System;
using System.Data.Common;
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
	public partial class ClassMetaType : AbstractType
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

		public override object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner)
		{
			return NullSafeGet(rs, names[0], session, owner);
		}

		public override object NullSafeGet(DbDataReader rs,string name,ISessionImplementor session,object owner)
		{
			int index = rs.GetOrdinal(name);

			if (rs.IsDBNull(index))
			{
				return null;
			}
			else
			{
				string str = (string) NHibernateUtil.String.Get(rs, index, session);
				return string.IsNullOrEmpty(str) ? null : str;
			}
		}

		public override void NullSafeSet(DbCommand st, object value, int index, bool[] settable, ISessionImplementor session)
		{
			if (settable[0]) NullSafeSet(st, value, index, session);
		}

		public override void NullSafeSet(DbCommand st,object value,int index,ISessionImplementor session)
		{
			if (value == null)
			{
				st.Parameters[index].Value = DBNull.Value;
			}
			else
			{
				NHibernateUtil.String.Set(st, value, index, session);
			}
		}

		public override string ToLoggableString(object value, ISessionFactoryImplementor factory)
		{
			return (string)value;
		}

		public override string Name
		{
			get { return "ClassMetaType"; }
		}

		public override object DeepCopy(object value, ISessionFactoryImplementor factory)
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

		public override object Replace(object original, object current, ISessionImplementor session, object owner, System.Collections.IDictionary copiedAlready)
		{
			return original;
		}

		public override bool[] ToColumnNullness(object value, IMapping mapping)
		{
			throw new NotSupportedException();
		}
	}
}
