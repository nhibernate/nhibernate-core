using System;
using System.Data.Common;

namespace NHibernate.Exceptions
{
    public abstract class SqlStateExtracter
    {
        /* Many drivers provide both SqlState and NativeError in the Exception
         * Some of them, like OdbcException, have fields SQLState, NativeError
         * Some of them contain it in Data field, like PsqlException
         * Some of them have only text message
         */

        public int ExtractErrorCode(DbException sqle)
        {
            int errorCode;
            Exception nested;
            errorCode = ExtractSingleErrorCode(sqle);
            nested = sqle.InnerException;
            while (errorCode == 0 && nested != null)
            {
                if (nested is DbException)
                {
                    errorCode = ExtractSingleErrorCode(sqle);
                }
                nested = sqle.InnerException;
            }
            return errorCode;
        }

        public string ExtractSqlState(DbException sqle)
        {
            string sqlState;
            Exception nested;
            sqlState = ExtractSingleSqlState(sqle);
            nested = sqle.InnerException;
            while (sqlState.Length == 0 && nested != null)
            {
                if (nested is DbException)
                {
                    sqlState = ExtractSingleSqlState(sqle);
                }
                nested = sqle.InnerException;
            }
            return sqlState;
        }

        public abstract int ExtractSingleErrorCode(DbException sqle);
        public abstract string ExtractSingleSqlState(DbException sqle);
    }
}
