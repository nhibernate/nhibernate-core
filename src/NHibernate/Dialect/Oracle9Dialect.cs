using System.Collections;
using System.Data;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.Hql;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Type;

namespace NHibernate.Dialect
{
	/// <summary>
	/// It's a immature version, it just work.
	/// An SQL dialect for Oracle 9
	/// </summary>
	/// <remarks>
	/// The Oracle9Dialect defaults the following configuration properties:
	/// <list type="table">
	///		<listheader>
	///			<term>Property</term>
	///			<description>Default Value</description>
	///		</listheader>
	///		<item>
	///			<term>hibernate.use_outer_join</term>
	///			<description><c>true</c></description>
	///		</item>
	///		<item>
	///			<term>hibernate.connection.driver_class</term>
	///			<description><see cref="NHibernate.Driver.OracleClientDriver" /></description>
	///		</item>
	/// </list>
	/// </remarks>
	public class Oracle9Dialect : Dialect
	{
		/// <summary></summary>
		public Oracle9Dialect() : base()
		{
//			DefaultProperties[Cfg.Environment.UseStreamsForBinary] = "true";
			DefaultProperties[ Environment.OuterJoin ] = "true";
			DefaultProperties[ Environment.ConnectionDriver ] = "NHibernate.Driver.OracleClientDriver";

			RegisterColumnType( DbType.AnsiStringFixedLength, "CHAR(255)" );
			RegisterColumnType( DbType.AnsiStringFixedLength, 2000, "CHAR($1)" );
			RegisterColumnType( DbType.AnsiString, "VARCHAR2(255)" );
			RegisterColumnType( DbType.AnsiString, 2000, "VARCHAR2($1)" );
			RegisterColumnType( DbType.AnsiString, 2147483647, "CLOB" ); // should use the IType.ClobType
			RegisterColumnType( DbType.Binary, "RAW(2000)" );
			RegisterColumnType( DbType.Binary, 2000, "RAW($1)" );
			RegisterColumnType( DbType.Binary, 2147483647, "BLOB" );
			RegisterColumnType( DbType.Boolean, "NUMBER(1,0)" );
			RegisterColumnType( DbType.Byte, "NUMBER(3,0)" );
			RegisterColumnType( DbType.Currency, "NUMBER(19,1)" );
			RegisterColumnType( DbType.Date, "DATE" );
			RegisterColumnType( DbType.DateTime, "DATE" );
			RegisterColumnType( DbType.Decimal, "NUMBER(19,5)" );
			RegisterColumnType( DbType.Decimal, 19, "NUMBER(19, $1)" );
			// having problems with both ODP and OracleClient from MS not being able
			// to read values out of a field that is DOUBLE PRECISION
			RegisterColumnType( DbType.Double, "DOUBLE PRECISION" ); //"FLOAT(53)" );
			//Oracle does not have a guid datatype
			//RegisterColumnType( DbType.Guid, "UNIQUEIDENTIFIER" );
			RegisterColumnType( DbType.Int16, "NUMBER(5,0)" );
			RegisterColumnType( DbType.Int32, "NUMBER(10,0)" );
			RegisterColumnType( DbType.Int64, "NUMBER(20,0)" );
			RegisterColumnType( DbType.Single, "FLOAT(24)" );
			RegisterColumnType( DbType.StringFixedLength, "NCHAR(255)" );
			RegisterColumnType( DbType.StringFixedLength, 2000, "NCHAR($1)" );
			RegisterColumnType( DbType.String, "NVARCHAR2(255)" );
			RegisterColumnType( DbType.String, 2000, "NVARCHAR2($1)" );
			RegisterColumnType( DbType.String, 1073741823, "NCLOB" );
			RegisterColumnType( DbType.Time, "DATE" );

			RegisterFunction( "abs", new StandardSQLFunction() );
			RegisterFunction( "sign", new StandardSQLFunction( NHibernateUtil.Int32 ) );

			RegisterFunction( "acos", new StandardSQLFunction( NHibernateUtil.Double ) );
			RegisterFunction( "asin", new StandardSQLFunction( NHibernateUtil.Double ) );
			RegisterFunction( "atan", new StandardSQLFunction( NHibernateUtil.Double ) );
			RegisterFunction( "cos", new StandardSQLFunction( NHibernateUtil.Double ) );
			RegisterFunction( "cosh", new StandardSQLFunction( NHibernateUtil.Double ) );
			RegisterFunction( "exp", new StandardSQLFunction( NHibernateUtil.Double ) );
			RegisterFunction( "ln", new StandardSQLFunction( NHibernateUtil.Double ) );
			RegisterFunction( "sin", new StandardSQLFunction( NHibernateUtil.Double ) );
			RegisterFunction( "sinh", new StandardSQLFunction( NHibernateUtil.Double ) );
			RegisterFunction( "stddev", new StandardSQLFunction( NHibernateUtil.Double ) );
			RegisterFunction( "sqrt", new StandardSQLFunction( NHibernateUtil.Double ) );
			RegisterFunction( "tan", new StandardSQLFunction( NHibernateUtil.Double ) );
			RegisterFunction( "tanh", new StandardSQLFunction( NHibernateUtil.Double ) );
			RegisterFunction( "variance", new StandardSQLFunction( NHibernateUtil.Double ) );

			RegisterFunction( "round", new StandardSQLFunction() );
			RegisterFunction( "trunc", new StandardSQLFunction() );
			RegisterFunction( "ceil", new StandardSQLFunction() );
			RegisterFunction( "floor", new StandardSQLFunction() );

			RegisterFunction( "chr", new StandardSQLFunction( NHibernateUtil.Character ) );
			RegisterFunction( "initcap", new StandardSQLFunction() );
			RegisterFunction( "lower", new StandardSQLFunction() );
			RegisterFunction( "ltrim", new StandardSQLFunction() );
			RegisterFunction( "rtrim", new StandardSQLFunction() );
			RegisterFunction( "soundex", new StandardSQLFunction() );
			RegisterFunction( "upper", new StandardSQLFunction() );
			RegisterFunction( "ascii", new StandardSQLFunction( NHibernateUtil.Int32 ) );
			RegisterFunction( "length", new StandardSQLFunction( NHibernateUtil.Int64 ) );

			RegisterFunction( "to_char", new StandardSQLFunction( NHibernateUtil.String ) );
			RegisterFunction( "to_date", new StandardSQLFunction( NHibernateUtil.Timestamp ) );

			RegisterFunction( "lastday", new StandardSQLFunction( NHibernateUtil.Date ) );
			RegisterFunction( "sysdate", new NoArgSQLFunction( NHibernateUtil.Date, false) );
			RegisterFunction( "uid", new NoArgSQLFunction( NHibernateUtil.Int32, false) );
			RegisterFunction( "user", new NoArgSQLFunction( NHibernateUtil.String, false) );

			// Multi-param string dialect functions...
			RegisterFunction( "concat", new StandardSQLFunction( NHibernateUtil.String ) );
			RegisterFunction( "instr", new StandardSQLFunction( NHibernateUtil.String ) );
			RegisterFunction( "instrb", new StandardSQLFunction( NHibernateUtil.String ) );
			RegisterFunction( "lpad", new StandardSQLFunction( NHibernateUtil.String ) );
			RegisterFunction( "replace", new StandardSQLFunction( NHibernateUtil.String ) );
			RegisterFunction( "rpad", new StandardSQLFunction( NHibernateUtil.String ) );
			RegisterFunction( "substr", new StandardSQLFunction( NHibernateUtil.String ) );
			RegisterFunction( "substrb", new StandardSQLFunction( NHibernateUtil.String ) );
			RegisterFunction( "translate", new StandardSQLFunction( NHibernateUtil.String ) );

			// Multi-param numeric dialect functions...
			RegisterFunction( "atan2", new StandardSQLFunction( NHibernateUtil.Single ) );
			RegisterFunction( "log", new StandardSQLFunction( NHibernateUtil.Int32 ) );
			RegisterFunction( "mod", new StandardSQLFunction( NHibernateUtil.Int32 ) );
			RegisterFunction( "nvl", new StandardSQLFunction() );
			RegisterFunction( "power", new StandardSQLFunction( NHibernateUtil.Single ) );

			// Multi-param date dialect functions...
			RegisterFunction( "add_months", new StandardSQLFunction( NHibernateUtil.Date ) );
			RegisterFunction( "months_between", new StandardSQLFunction( NHibernateUtil.Single ) );
			RegisterFunction( "next_day", new StandardSQLFunction( NHibernateUtil.Date ) );
		}

		/// <summary></summary>
		public override string AddColumnString
		{
			get { return "add"; }
		}

		/// <summary></summary>
		public override string GetSequenceNextValString( string sequenceName )
		{
			return "select " + sequenceName + ".nextval from dual";
		}

		/// <summary></summary>
		public override string GetCreateSequenceString( string sequenceName )
		{
			return "create sequence " + sequenceName + " INCREMENT BY 1 START WITH 1 MAXVALUE 1.0E28 MINVALUE 1 NOCYCLE CACHE 20 NOORDER";
		}

		/// <summary></summary>
		public override string GetDropSequenceString( string sequenceName )
		{
			return "drop sequence " + sequenceName;
		}

		/// <summary></summary>
		public override string CascadeConstraintsString
		{
			get { return " cascade constraints"; }
		}

		/// <summary></summary>
		public override bool SupportsForUpdateNoWait
		{
			get { return true; }
		}

		/// <summary></summary>
		public override bool SupportsSequences
		{
			get { return true; }
		}

		/// <summary></summary>
		public override bool SupportsLimit
		{
			get { return true; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="querySqlString"></param>
		/// <returns></returns>
		public override SqlString GetLimitString( SqlString querySqlString )
		{
			Parameter p1 = new Parameter( "p1", new Int16SqlType() );
			Parameter p2 = new Parameter( "p2", new Int16SqlType() );

			/*
			 * "select * from ( select row_.*, rownum rownum_ from ( "
			 * sql
			 * " ) row_ where rownum <= ?) where rownum_ > ?"
			 */
			SqlStringBuilder pagingBuilder = new SqlStringBuilder();
			pagingBuilder.Add( "select * from ( select row_.*, rownum rownum_ from ( " );
			pagingBuilder.Add( querySqlString );
			pagingBuilder.Add( " ) row_ where rownum <= " );
			pagingBuilder.Add( p1 );
			pagingBuilder.Add( ") where rownum_ > " );
			pagingBuilder.Add( p2 );

			return pagingBuilder.ToSqlString();
		}

		/// <summary></summary>
		public override bool BindLimitParametersInReverseOrder
		{
			get { return true; }
		}

		/// <summary></summary>
		public override bool SupportsForUpdateOf
		{
			get { return true; }
		}

		/// <summary></summary>
		public override bool UseMaxForLimit
		{
			get { return true; }
		}
	}
}