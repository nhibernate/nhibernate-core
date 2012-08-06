using System.Linq;
using System;
using System.Data;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace NHibernate.Test.NHSpecificTest.NH3237
{
    public class TestEnumUserType : IUserType
    {
        public System.Type ReturnedType
        {
            get { return typeof(TestEnum); }
        }

        public SqlType[] SqlTypes
        {
            get { return new[] { new SqlType(DbType.Int32) }; }
        }

        public object NullSafeGet(IDataReader dr, string[] names, object owner)
        {
            object r = dr[names[0]];
            if (r == DBNull.Value)
            {
                return null;
            }

            var dbValue = (int)r;
            return (TestEnum)dbValue;
        }

        public void NullSafeSet(IDbCommand cmd, object value, int index)
        {
            if (value == null)
            {
                NHibernateUtil.DateTime.NullSafeSet(cmd, null, index);
            }
            else
            {
                var paramVal = (int)value;

                IDataParameter parameter = (IDataParameter)cmd.Parameters[index];
                parameter.Value = paramVal;
            }
        }

        public object Assemble(object cached, object owner)
        {
            return cached;
        }

        public object DeepCopy(object value)
        {
            return value;
        }

        public object Disassemble(object value)
        {
            return value;
        }

        public new bool Equals(object x, object y)
        {
            if (ReferenceEquals(x, null))
            {
                return ReferenceEquals(y, null);
            }
            return x.Equals(y);
        }

        public int GetHashCode(object x)
        {
            if (ReferenceEquals(x, null))
            {
                return 0;
            }
            return x.GetHashCode();
        }

        public bool IsMutable
        {
            get { return false; }
        }

        public object Replace(object original, object target, object owner)
        {
            return original;
        }

        public int Compare(object x, object y)
        {
            return ((DateTimeOffset)x).CompareTo((DateTimeOffset)y);
        }
    }
}
