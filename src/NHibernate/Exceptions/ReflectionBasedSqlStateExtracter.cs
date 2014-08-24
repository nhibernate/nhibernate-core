using System.Reflection;
using System.Collections;
using System.Data.Common;

namespace NHibernate.Exceptions
{
    class ReflectionBasedSqlStateExtracter: SqlStateExtracter
    {
 
        /* OdbcException, OleDbException, IfxException, Db2Exception, and possible others
         * have Errors collection which contains fields: NativeError and SQLState
         * These fields can be extracted using reflection
         */
        public override int ExtractSingleErrorCode(DbException sqle)
        {
            System.Type type;
            PropertyInfo pi;
            int nativeError;

            type = sqle.GetType();
            pi = type.GetProperty("Errors");
            if (pi == null) // there is no Errors property
            {
                return 0;
            }
            nativeError = 0;
            foreach (object o in (pi.GetValue(sqle, null) as IEnumerable))
            {
                pi = o.GetType().GetProperty("NativeError");
                if (pi == null)
                    return 0;
                nativeError = (int)pi.GetValue(o, null);
                if (nativeError != 0)
                    break;
            }
            return nativeError;
        }

        public override string ExtractSingleSqlState(DbException sqle)
        {
            System.Type type;
            PropertyInfo pi;
            string sqlState;

            type = sqle.GetType();
            pi = type.GetProperty("Errors");
            if (pi == null) // there is no Errors property
            {
                return null;
            }
            sqlState = "";
            foreach (object o in (pi.GetValue(sqle, null) as IEnumerable))
            {
                pi = o.GetType().GetProperty("SQLState");
                if (pi == null)
                    return null;
                sqlState = (string)pi.GetValue(o, null);
                if (sqlState.Length != 0)
                    break;
            }
            return sqlState;
        }
    }
}