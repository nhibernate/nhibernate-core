using System.Data;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace NHibernate.Type
{
    public class IntBooleanType : BooleanType, IUserType
    {
        public override SqlType SqlType
        {
            get {  return new SqlType(DbType.Int32);}
        }

        public SqlType[] SqlTypes
        {
            get { return new SqlType[] { SqlType};}
        }

        public System.Type ReturnedType
        {
            get { return typeof (bool); }
        }
        public bool Equals(object x, object y)
        {
            if (ReferenceEquals(x, y)) return true;

            if (x == null || y == null) return false;

            return x.Equals(y);
        }

        public int GetHashCode(object x)
        {
            return x.GetHashCode();
        }

        public new void NullSafeSet(IDbCommand cmd, object value, int index)
        {
            var val = !((bool) value) ? 0 : 1;
            NHibernateUtil.Int32.NullSafeSet(cmd, val, index);
        }

        public override void Set(IDbCommand cmd, object value, int index)
        {
            var val = !((bool) value) ? 0 : 1;
            ((IDataParameter) cmd.Parameters[index]).Value = val;
        }

        public object NullSafeGet(IDataReader rs, string[] names, object owner)
        {
            return NHibernateUtil.Int32.NullSafeGet(rs, names[0]);
        }

        public object DeepCopy(object value)
        {
            return value;
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