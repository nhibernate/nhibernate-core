using System.Data;
using System.Data.Common;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace NHibernate.Test.NHSpecificTest.GH2704
{
	public class StringBoolToBoolUserType : IEnhancedUserType
	{
		public object Assemble(object cached, object owner) => cached;

		public bool IsMutable => false;
		public object DeepCopy(object value) => value;
		public object Disassemble(object value) => value;
		public object Replace(object original, object target, object owner) => original;

		public object FromXMLString(string xml) => xml;
		public string ToXMLString(object value) => ((bool) value) ? "'S'" : "'N'";
		public string ObjectToSQLString(object value) => ((bool) value) ? "'S'" : "'N'";

		bool IUserType.Equals(object x, object y) => x == null ? false : x.Equals(y);
		public int GetHashCode(object x) => x == null ? typeof(bool).GetHashCode() + 473 : x.GetHashCode();

		public object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner)
		{
			var value = NHibernateUtil.String.NullSafeGet(rs, names[0], session);
			if (value == null) return false;

			return (string) value == "S";
		}

		public void NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session)
		{
			if (value == null)
			{
				NHibernateUtil.String.NullSafeSet(cmd, null, index, session);
				return;
			}

			value = (bool) value ? "S" : "N";
			NHibernateUtil.String.NullSafeSet(cmd, value, index, session);
		}

		public System.Type ReturnedType => typeof(bool);
		public SqlType[] SqlTypes => new SqlType[] { new SqlType(DbType.String) };
	}
}
