using System;
using System.Data;

namespace NHibernate.DomainModel
{

	public class MultiplicityType : ICompositeUserType 
	{
		private static string[] PROP_NAMES = new String[] { 
															  "count", "glarch" 
														  };
		private static Type.IType[] TYPES = new Type.IType[] { 
														   NHibernate.Int32, NHibernate.GetSerializable(typeof(Glarch))
													   };
		public String[] PropertyNames
		{
			get
			{
				return PROP_NAMES;
			}
		}
		public Type.IType[] PropertyTypes
		{
			get
		{
			return TYPES;
		}
		}
		public object GetPropertyValue(object component, int property) 
		{
			Multiplicity o = (Multiplicity) component;
			return property==0 ? 
				(object)o.count : 
				(object)o.glarch;
		}

		public void SetPropertyValue(object component, int property, object value) 
		{
			Multiplicity o = (Multiplicity) component;
			if (property==0) 
			{
				o.count = (int)value;
			}
			else 
			{
				o.glarch = (Glarch) value;
			}
		}
		public System.Type ReturnedClass
		{
			get
			{
				return typeof(Multiplicity);
			}
		}
		public new bool Equals(object x, object y) 
		{
			Multiplicity mx = (Multiplicity) x;
			Multiplicity my = (Multiplicity) y;
			if (mx==my) return true;
			if (mx==null || my==null) return false;
			return mx.count==my.count && mx.glarch==my.glarch;
		}

		public object NullSafeGet(IDataReader rs, String[] names, Engine.ISessionImplementor session, Object owner)
		{
			int c = (int) NHibernate.Int32.NullSafeGet( rs, names[0], session, owner);
			GlarchProxy g = (GlarchProxy) NHibernate.GetSerializable(typeof(Glarch)).NullSafeGet(rs, names[1], session, owner);
			Multiplicity m = new Multiplicity();
			m.count = c==0 ? 0 : c;
			m.glarch = g;
			return m;
		}

		public void NullSafeSet(IDbCommand st, Object value, int index, Engine.ISessionImplementor session)
		{
			Multiplicity o = (Multiplicity) value;
			GlarchProxy g;
			int c;
			if (o==null) 
			{
				g=null;
				c=0;
			}
			else 
			{
				g = o.glarch;
				c =o.count;
			}
			NHibernate.Int32.NullSafeSet(st, c, index, session);
			NHibernate.GetSerializable(typeof(Glarch)).NullSafeSet(st, g, index+1, session);
		}
		public object DeepCopy(object value) 
		{
			if (value==null) return null;
			Multiplicity v = (Multiplicity) value;
			Multiplicity m = new Multiplicity();
			m.count = v.count;
			m.glarch = m.glarch;
			return m;
		}
		public bool IsMutable
		{
			get
			{
				return true;
			}
		}
		public object Assemble(object cached, Engine.ISessionImplementor session, Object owner) 
		{
			throw new InvalidOperationException();
		}
		public object Disassemble(Object value, Engine.ISessionImplementor session) 
		{
			throw new InvalidOperationException();
		}
	}
}