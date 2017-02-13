using System;
using System.Data.Common;
using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.UserTypes;

namespace NHibernate.Test.NHSpecificTest.NH2394
{
	class PhoneNumberUserType : ICompositeUserType
	{
		public string[] PropertyNames
		{
			get { return new[] { "CountryCode", "Number" }; }
		}

		public IType[] PropertyTypes
		{
            get { return new IType[] { NHibernateUtil.Int32, NHibernateUtil.String }; }
		}

		public object GetPropertyValue(object component, int property)
		{
			PhoneNumber phone = (PhoneNumber)component;

			switch (property)
			{
				case 0: return phone.CountryCode;
				case 1: return phone.Number;
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

		bool ICompositeUserType.Equals(object x, object y)
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
			if (dr.IsDBNull(dr.GetOrdinal(names[0])))
				return null;

			return new PhoneNumber(
				(int)NHibernateUtil.Int32.NullSafeGet(dr, names[0], session, owner),
				(string)NHibernateUtil.String.NullSafeGet(dr, names[1], session, owner));
		}

		public void NullSafeSet(DbCommand cmd, object value, int index, bool[] settable, ISessionImplementor session)
		{
			object countryCode = value == null ? null : (int?)((PhoneNumber)value).CountryCode;
			object number = value == null ? null : ((PhoneNumber)value).Number;

			if (settable[0]) NHibernateUtil.Int32.NullSafeSet(cmd, countryCode, index++, session);
			if (settable[1]) NHibernateUtil.String.NullSafeSet(cmd, number, index, session);
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
