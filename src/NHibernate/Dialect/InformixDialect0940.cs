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
    /// This dialect is intended to work with IDS version 9.40
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
    public class InformixDialect0940 : InformixDialect
    {
        /// <summary></summary>
        public InformixDialect0940()
            : base()
        {
            RegisterColumnType(DbType.AnsiString, 2147483647, "CLOB");
            RegisterColumnType(DbType.Binary, 2147483647, "BLOB");
            RegisterColumnType(DbType.Binary, "BLOB");
            RegisterColumnType(DbType.String, 2147483647, "CLOB");
        }


        /// <summary> Get the select command used retrieve the names of all sequences.</summary>
        /// <returns> The select command; or null if sequences are not supported. </returns>
        public override string QuerySequencesString
        {
            get
            {
                return "select tabname from systables where tabtype='Q'";
            }
        }

        /// <summary>
        /// Does this dialect support sequences?
        /// </summary>
        public override bool SupportsSequences
        {
            get { return true; }
        }

        /// <summary> 
        /// Does this dialect support "pooled" sequences.  Not aware of a better
        /// name for this.  Essentially can we specify the initial and increment values? 
        /// </summary>
        /// <returns> True if such "pooled" sequences are supported; false otherwise. </returns>
        public override bool SupportsPooledSequences
        {
            get { return true; }
        }

        /// <summary> 
        /// Generate the appropriate select statement to to retreive the next value
        /// of a sequence.
        /// </summary>
        /// <param name="sequenceName">the name of the sequence </param>
        /// <returns> String The "nextval" select string. </returns>
        /// <remarks>This should be a "stand alone" select statement.</remarks>
        public override string GetSequenceNextValString(string sequenceName)
        {
					return "select " + GetSelectSequenceNextValString(sequenceName) + " from systables where tabid=1";
        }

        public override string GetDropSequenceString(string sequenceName)
        {
            return "drop sequence " + sequenceName;
        }
        /// <summary> 
        /// Generate the select expression fragment that will retrieve the next
        /// value of a sequence as part of another (typically DML) statement.
        /// </summary>
        /// <param name="sequenceName">the name of the sequence </param>
        /// <returns> The "nextval" fragment. </returns>
        /// <remarks>
        /// This differs from <see cref="GetSequenceNextValString"/> in that this
        /// should return an expression usable within another statement.
        /// </remarks>
        public override string GetSelectSequenceNextValString(string sequenceName)
        {
            return sequenceName + ".nextval";
        }
        public override string GetCreateSequenceString(string sequenceName)
        {
            return "create sequence " + sequenceName;
            // +
            //" INCREMENT BY 1 START WITH 1 MINVALUE 1 NOCYCLE CACHE 20 NOORDER";

        }

        // in .NET overloaded version cannot be overriden (in Java allowed)
        //protected override string GetCreateSequenceString(string sequenceName, int initialValue, int incrementSize)
        //{
        //    return "create sequence " + sequenceName +
        //        " INCREMENT BY " + incrementSize.ToString() +
        //            " START WITH " + initialValue.ToString() +
        //            " MINVALUE 1 NOCYCLE CACHE 20 NOORDER";
        //}

        /// <summary> 
        /// Create a <see cref="JoinFragment"/> strategy responsible
        /// for handling this dialect's variations in how joins are handled. 
        /// </summary>
        /// <returns> This dialect's <see cref="JoinFragment"/> strategy. </returns>
        public override JoinFragment CreateOuterJoinFragment()
        {
            // ANSI join exist from 9.21 but CROSS, RIGHT and FULL were introduced in 9.40;
            return new ANSIJoinFragment();
        }

        /// <summary>
        /// Does this Dialect have some kind of <c>LIMIT</c> syntax?
        /// </summary>
        /// <value>False, unless overridden.</value>
        public override bool SupportsLimit
        {
            get { return false; }
        }

        /// <summary>
        /// Does this Dialect support an offset?
        /// </summary>
        public override bool SupportsLimitOffset
        {
            get { return false; }
        }

    };
}