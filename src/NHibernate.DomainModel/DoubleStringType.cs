using System;
using System.Data;

namespace NHibernate.DomainModel
{
	public class DoubleStringType : ICompositeUserType 
	{
	
		public System.Type ReturnedClass
		{
			get
			{
				return typeof(String[]);
			}
		}
	
		public new bool Equals(object x, object y) 
		{
			if (x==y) return true;
			if (x==null || y==null) return false;
			string[] lhs = (string[])x;
			string[] rhs = (string[])y;

			return lhs[0].Equals(rhs[0]) && lhs[1].Equals(rhs[1]);
		}
	
		public Object DeepCopy(Object x) 
		{
			if (x==null) return null;
			String[] result = new String[2];
			String[] input = (String[]) x;
			result[0] = input[0];
			result[1] = input[1];
			return result;
		}
	
		public bool IsMutable
		{
			get { return true; }
		}
	
		public Object NullSafeGet(IDataReader rs, String[] names, Engine.ISessionImplementor session, Object owner)
		{
		
			String first = (String) NHibernateUtil.String.NullSafeGet(rs, names[0], session, owner);
			String second = (String) NHibernateUtil.String.NullSafeGet(rs, names[1], session, owner);
		
			return ( first==null && second==null ) ? null : new String[] { first, second };
		}

	
		public void NullSafeSet(IDbCommand st, Object value, int index, Engine.ISessionImplementor session)
		{		
			String[] strings = (value==null) ? new String[2] : (String[]) value;
		
			NHibernateUtil.String.NullSafeSet(st, strings[0], index, session);
			NHibernateUtil.String.NullSafeSet(st, strings[1], index+1, session);
		}
	
		public String[] PropertyNames
		{
			get
			{
				return new String[] { "s1", "s2" };
			}
		}

		public Type.IType[] PropertyTypes
		{
			get
			{
				return new Type.IType[] { NHibernateUtil.String, NHibernateUtil.String };
			}
		}

		public Object GetPropertyValue(Object component, int property) 
		{
			return ( (String[]) component )[property];
		}

		public void SetPropertyValue(
			Object component,
			int property,
			Object value) 
		{
		
			( (String[]) component )[property] = (String) value;
		}

		public object Assemble(
			object cached,
			Engine.ISessionImplementor session,
			object owner) 
		{
		
			return DeepCopy(cached);
		}

		public object Disassemble(Object value, Engine.ISessionImplementor session) 
		{
			return DeepCopy(value);
		}
	}
}