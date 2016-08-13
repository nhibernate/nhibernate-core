﻿using System;
using System.Data;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NHibernate.UserTypes;

namespace NHibernate.Test.NHSpecificTest.NH3874
{
	public class IntWrapperType : IUserType
	{
		public SqlType[] SqlTypes
		{
			get { return new[] { NHNullableType.SqlType }; }
		}

		public System.Type ReturnedType { get { return typeof(object); } }

		public new bool Equals(object x, object y)
		{
			if (ReferenceEquals(x, y)) return true;
			if (x == null || y == null) return false;
			return x.Equals(y);
		}

		public object DeepCopy(object value) { return value; }

		public bool IsMutable { get { return false; } }

		protected NullableType NHNullableType
		{
			get { return NHibernateUtil.Int32; }
		}

		public object NullSafeGet(IDataReader dr, string[] names, object owner)
		{
			object obj = NHNullableType.NullSafeGet(dr, names[0]);
			if (obj == null) return null;
			return new IntWrapper((int)obj);
		}

		public void NullSafeSet(IDbCommand cmd, object obj, int index)
		{
			if (obj == null)
			{
				((IDataParameter)cmd.Parameters[index]).Value = DBNull.Value;
			}
			else
			{
				IntWrapper id = (IntWrapper)obj;
				((IDataParameter)cmd.Parameters[index]).Value = id.Id;
			}
		}

		public int GetHashCode(object x)
		{
			return x.GetHashCode();
		}

		public object Replace(object original, object target, object owner)
		{
			return original;
		}

		public object Assemble(object cached, object owner)
		{
			return cached;
		}

		public object Disassemble(object value)
		{
			return value;
		}
	}
}
