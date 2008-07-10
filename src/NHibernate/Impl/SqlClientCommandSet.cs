
using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;

namespace NHibernate.Impl
{
    internal class SqlClientCommandSet : DbCommandSet<SqlConnection, System.Data.SqlClient.SqlCommand>
    {
        private static System.Type sqlCmdSetType;

        static SqlClientCommandSet()
        {
            Assembly sysData = Assembly.Load("System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
            sqlCmdSetType = sysData.GetType("System.Data.SqlClient.SqlCommandSet");
            Debug.Assert(sqlCmdSetType != null, "Could not find SqlCommandSet!");
        }

        protected override object CreateInternalCommandSet()
        {
            return Activator.CreateInstance(sqlCmdSetType, true);
        }

    }
}

