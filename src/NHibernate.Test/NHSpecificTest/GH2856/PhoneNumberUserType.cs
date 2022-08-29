using System;
using System.Data.Common;
using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.UserTypes;

namespace NHibernate.Test.NHSpecificTest.GH2856
{
	class PhoneNumberUserType : ICompositeUserType
	{
		public string[] PropertyNames
		{
			get { return new[] {"Number", "Ext"}; }
		}

		public IType[] PropertyTypes
		{
			get { return new IType[] {NHibernateUtil.String, NHibernateUtil.String}; }
		}

		public object GetPropertyValue(object component, int property)
		{
			PhoneNumber phone = (PhoneNumber) component;

			switch (property)
			{
				case 0: return phone.Number;
				case 1: return phone.Ext;
				default: throw new NotImplementedException();
			}
		}

		public void SetPropertyValue(object component, int property, object value)
		{
			throw new NotImplementedException();
		}

		public System.Type ReturnedClass
		{
			get { return typeof(PhoneNumber); }
		}

		public new bool Equals(object x, object y)
		{
			if (ReferenceEquals(x, null) && ReferenceEquals(y, null))
				return true;

			if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
				return false;

			return x.Equals(y);
		}

		public int GetHashCode(object x)
		{
			return x.GetHashCode();
		}

		public object NullSafeGet(DbDataReader dr, string[] names, ISessionImplementor session, object owner)
		{
			return dr.IsDBNull(dr.GetOrdinal(names[0]))
				? null
				: (object) new PhoneNumber(
					(string) NHibernateUtil.String.NullSafeGet(dr, names[0], session, owner),
					(string) NHibernateUtil.String.NullSafeGet(dr, names[1], session, owner));
		}

		public void NullSafeSet(DbCommand cmd, object value, int index, bool[] settable, ISessionImplementor session)
		{
			object number = value == null ? null : ((PhoneNumber) value).Number;
			object ext = value == null ? null : ((PhoneNumber) value).Ext;

			if (settable[0]) NHibernateUtil.String.NullSafeSet(cmd, number, index++, session);
			if (settable[1]) NHibernateUtil.String.NullSafeSet(cmd, ext, index, session);
		}

		public object DeepCopy(object value)
		{
			return value;
		}

		public bool IsMutable
		{
			get { return false; }
		}

		public object Disassemble(object value, ISessionImplementor session)
		{
			return value;
		}

		public object Assemble(object cached, ISessionImplementor session, object owner)
		{
			return cached;
		}

		public object Replace(object original, object target, ISessionImplementor session, object owner)
		{
			return original;
		}
	}
}
