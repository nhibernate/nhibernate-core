using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;
using System.Text;
using NHibernate.DomainModel.NHSpecific;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NHibernate.UserTypes;

namespace NHibernate.Test.TypeParameters
{
    public class DefaultValueIntegerType : IUserType, IParameterizedType
    {
        int defaultValue;
        private static NullableType _int32Type = NHibernateUtil.Int32;

        public void SetParameterValues(IDictionary parameters)
        {
            defaultValue = int.Parse((string) parameters["default"]);
        }

        #region IUserType Members

        public new bool Equals(object x, object y)
        {

            if (x == y) return true;

            int lhs = (x == null) ? 0 : (int)x;
            int rhs = (y == null) ? 0 : (int)y;

            return _int32Type.Equals(lhs, rhs);

        }

        public SqlType[] SqlTypes
        {
            get
            {
                return new SqlType[] { _int32Type.SqlType };
            }
        }

        public object DeepCopy(object value)
        {
            return value;
        }

        public void NullSafeSet(System.Data.IDbCommand cmd, object value, int index)
        {
            if (value.Equals(defaultValue))
            {
                ((IDbDataParameter)cmd.Parameters[index]).Value = DBNull.Value;
            }
            else
            {
                _int32Type.Set(cmd, value, index);
            }
        }

        public System.Type ReturnedType
        {
            get { return typeof(System.Int32); }
        }

        public object NullSafeGet(System.Data.IDataReader rs, string[] names, object owner)
        {
            object value = _int32Type.NullSafeGet(rs, names);
            if (value == null)
                return defaultValue;
            return value;
        }

        public bool IsMutable
        {
            get { return _int32Type.IsMutable; }
        }

        #endregion
        
    }
}
