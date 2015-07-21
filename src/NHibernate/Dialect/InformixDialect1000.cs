using System.Data;
using NHibernate.Cfg;
using NHibernate.Dialect.Function;
using NHibernate.SqlCommand;
using System.Data.Common;
using NHibernate.Exceptions;
using NHibernate.Util;
//using NHibernate.Dialect.Schema;
using Environment = NHibernate.Cfg.Environment;


namespace NHibernate.Dialect
{
    /// <summary>
    /// Summary description for InformixDialect.
    /// This dialect is intended to work with IDS version 10.00
    /// </summary>
    /// <remarks>
    /// The InformixDialect defaults the following configuration properties:
    /// <list type="table">
    ///		<listheader>
    ///			<term>ConnectionDriver</term>
    ///			<description>NHibernate.Driver.OdbcDriver</description>
    ///			<term>PrepareSql</term>
    ///			<description>true</description>
    ///		</listheader>
    ///		<item>
    ///			<term>connection.driver_class</term>
    ///			<description><see cref="NHibernate.Driver.OdbcDriver" /></description>
    ///		</item>
    /// </list>
    /// </remarks>
    public class InformixDialect1000 : InformixDialect0940
    {
        /// <summary></summary>
        public InformixDialect1000()
            : base()
        {
        }

        /// <summary>
        /// Does this Dialect have some kind of <c>LIMIT</c> syntax?
        /// </summary>
        /// <value>False, unless overridden.</value>
        public override bool SupportsLimit
        {
            get { return true; }
        }

        /// <summary>
        /// Does this Dialect support an offset?
        /// </summary>
        public override bool SupportsLimitOffset
        {
            get { return true; }
        }
    }
}