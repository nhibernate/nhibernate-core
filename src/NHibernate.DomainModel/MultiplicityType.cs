using System;
using System.Data.Common;
using System.Linq;
using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.UserTypes;

namespace NHibernate.DomainModel
{
	[Serializable]
	public class MultiplicityType : ICompositeUserType
	{
		private static readonly string[] PROP_NAMES = new String[]
			{
				"count", "glarch"
			};

		private static readonly IType[] TYPES = new IType[]
			{
				NHibernateUtil.Int32, NHibernateUtil.Entity(typeof(Glarch))
			};

		public String[] PropertyNames
		{
			get { return PROP_NAMES; }
		}

		public IType[] PropertyTypes
		{
			get { return TYPES; }
		}

		public object GetPropertyValue(object component, int property)
		{
			Multiplicity o = (Multiplicity) component;
			return property == 0 ?
			       (object) o.count :
			       (object) o.glarch;
		}

		public void SetPropertyValue(object component, int property, object value)
		{
			Multiplicity o = (Multiplicity) component;
			if (property == 0)
			{
				o.count = (int) value;
			}
			else
			{
				o.glarch = (Glarch) value;
			}
		}

		public System.Type ReturnedClass
		{
			get { return typeof(Multiplicity); }
		}

		public new bool Equals(object x, object y)
		{
			Multiplicity mx = (Multiplicity) x;
			Multiplicity my = (Multiplicity) y;
			if (mx == my) return true;
			if (mx == null || my == null) return false;
			return mx.count == my.count && mx.glarch == my.glarch;
		}

		public int GetHashCode(object x)
		{
			unchecked
			{
				Multiplicity o = (Multiplicity) x;
				return o.count + o.glarch.GetHashCode();
			}
		}

		public object NullSafeGet(DbDataReader rs, String[] names, ISessionImplementor session, Object owner)
		{
			int c = (int) NHibernateUtil.Int32.NullSafeGet(rs, names[0], session, owner);
			GlarchProxy g = (GlarchProxy) NHibernateUtil.Entity(typeof(Glarch)).NullSafeGet(rs, names[1], session, owner);
			Multiplicity m = new Multiplicity();
			m.count = (c == 0 ? 0 : c);
			m.glarch = g;
			return m;
		}

		public void NullSafeSet(DbCommand st, Object value, int index, bool[] settable, ISessionImplementor session)
		{
			Multiplicity o = (Multiplicity) value;
			GlarchProxy g;
			int c;
			if (o == null)
			{
				g = null;
				c = 0;
			}
			else
			{
				g = o.glarch;
				c = o.count;
			}
			if (settable[0]) NHibernateUtil.Int32.NullSafeSet(st, c, index, session);
			NHibernateUtil.Entity(typeof(Glarch)).NullSafeSet(st, g, index + 1, settable.Skip(1).ToArray(), session);
		}

		public object DeepCopy(object value)
		{
			if (value == null) return null;
			Multiplicity v = (Multiplicity) value;
			Multiplicity m = new Multiplicity();
			m.count = v.count;
			m.glarch = v.glarch;
			return m;
		}

		public bool IsMutable
		{
			get { return true; }
		}

		public object Assemble(object cached, ISessionImplementor session, Object owner)
		{
			if (cached == null)
			{
				return null;
			}

			object[] o = (object[]) cached;
			Multiplicity m = new Multiplicity();
			m.count = (int) o[0];
			m.glarch = o[1] == null ?
			           null :
			           (GlarchProxy) session.InternalLoad(typeof(Glarch).FullName, o[1], false, false);
			return m;
		}

		public object Disassemble(Object value, ISessionImplementor session)
		{
			if (value == null)
			{
				return null;
			}

			Multiplicity m = (Multiplicity) value;
			return new object[] {m.count, ForeignKeys.GetEntityIdentifierIfNotUnsaved(null, m.glarch, session)};
		}

		public object Replace(object original, object target, ISessionImplementor session, object owner)
		{
			return Assemble(Disassemble(original, session), session, owner);
		}
	}
}