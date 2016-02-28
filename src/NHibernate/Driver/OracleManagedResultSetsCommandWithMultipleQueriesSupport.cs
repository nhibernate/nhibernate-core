using System;
using System.Data;
using System.Linq;
using System.Reflection;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Util;

namespace NHibernate.Driver
{
    /// <summary>
    /// ResultSetsCommand implementation with support for Futures/MultiCriteria/MultiQuery.
    /// Proposed by Arturas Vitkauskas (http://stackoverflow.com/users/1951631/arturas-vitkauskas)
    /// http://stackoverflow.com/questions/10046461/nhibernate-multi-query-futures-with-oracle/14175635#14175635
    /// </summary>
    public class OracleManagedResultSetsCommandWithMultipleQueriesSupport: BasicResultSetsCommand
    {
        private const string DriverAssemblyName = "Oracle.ManagedDataAccess";

        private SqlString _sqlString = new SqlString();
        private int _cursorCount;
        private readonly PropertyInfo _oracleDbType;
        private readonly object _oracleDbTypeRefCursor;

        public OracleManagedResultSetsCommandWithMultipleQueriesSupport(ISessionImplementor session) 
            : base(session)
        {
            System.Type parameterType = ReflectHelper
                .TypeFromAssembly("Oracle.ManagedDataAccess.Client.OracleParameter",
                DriverAssemblyName, false);
            
            _oracleDbType = parameterType.GetProperty("OracleDbType");

            System.Type oracleDbTypeEnum = ReflectHelper
                .TypeFromAssembly("Oracle.ManagedDataAccess.Client.OracleDbType", 
                DriverAssemblyName, false);
            
            _oracleDbTypeRefCursor = Enum.Parse(oracleDbTypeEnum, "RefCursor");
        }

        public override void Append(ISqlCommand command)
        {
            Commands.Add(command);
            _sqlString = _sqlString.Append("\nOPEN :cursor")
                .Append(Convert.ToString(_cursorCount++))
                .Append("\nFOR\n")
                .Append(command.Query).Append("\n;\n");
        }

        public override SqlString Sql
        {
            get { return _sqlString; }
        }

        public override IDataReader GetReader(int? commandTimeout)
        {
            var batcher = Session.Batcher;
            SqlType[] sqlTypes = Commands.SelectMany(c => c.ParameterTypes).ToArray();
            ForEachSqlCommand((sqlLoaderCommand, offset) => sqlLoaderCommand.ResetParametersIndexesForTheCommand(offset));

            _sqlString = _sqlString.Insert(0, "\nBEGIN\n").Append("\nEND;\n");

            var command = batcher.PrepareQueryCommand(CommandType.Text, _sqlString, sqlTypes);
            if (commandTimeout.HasValue)
            {
                command.CommandTimeout = commandTimeout.Value;
            }

            BindParameters(command);

            for (int cursorIndex = 0; cursorIndex < _cursorCount; cursorIndex++)
            {
                IDbDataParameter outCursor = command.CreateParameter();
                _oracleDbType.SetValue(outCursor, _oracleDbTypeRefCursor, null);
                outCursor.ParameterName = ":cursor" + Convert.ToString(cursorIndex);
                outCursor.Direction = ParameterDirection.Output;
                command.Parameters.Add(outCursor);
            }

            return new BatcherDataReaderWrapper(batcher, command);
        }
    }
}
