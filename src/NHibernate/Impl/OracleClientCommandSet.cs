
using System;
using System.Data.OracleClient;
using System.Diagnostics;
using System.Reflection;

namespace NHibernate.Impl
{
    internal class OracleClientCommandSet : DbCommandSet<OracleConnection, OracleCommand>
    {
        private static System.Type oracleCmdSetType;

        static OracleClientCommandSet()
        {
            Assembly sysDataOracleClient = Assembly.Load("System.Data.OracleClient, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
            oracleCmdSetType = sysDataOracleClient.GetType("System.Data.OracleClient.OracleCommandSet");
            Debug.Assert(oracleCmdSetType != null, "Could not find OracleCommandSet!");
        }

        protected override object CreateInternalCommandSet()
        {
            return Activator.CreateInstance(oracleCmdSetType, true);
        }

    }
}

