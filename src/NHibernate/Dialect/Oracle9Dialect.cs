using System;
using System.Collections;
using System.Data;
using System.Text;

using NHibernate.Engine;
using NHibernate.Hql;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Dialect 
{
	/// <summary>
	/// It's a immature version, it just work.
	/// An SQL dialect for Oracle 9
	/// </summary>
	public class Oracle9Dialect : Dialect 
	{

		private readonly IDictionary aggregateFunctions = new Hashtable();

		public Oracle9Dialect() : base() 
		{
//			DefaultProperties[Cfg.Environment.UseStreamsForBinary] = "true";
			DefaultProperties[Cfg.Environment.StatementBatchSize] = DefaultBatchSize;
			DefaultProperties[Cfg.Environment.OuterJoin] = "true";

			Register( DbType.AnsiStringFixedLength, "CHAR(255)");
			Register( DbType.AnsiStringFixedLength, 2000, "CHAR($1)" );
			Register( DbType.AnsiString, "VARCHAR2(255)" );
			Register( DbType.AnsiString, 2000, "VARCHAR2($1)" );
			Register( DbType.AnsiString, 2147483647, "CLOB"); // should use the IType.ClobType
			Register( DbType.Binary, "RAW(2000)");
			Register( DbType.Binary, 2000, "RAW($1)");
			Register( DbType.Binary, 2147483647, "BLOB" );
			Register( DbType.Boolean, "NUMBER(1,0)" ); 
			Register( DbType.Byte, "NUMBER(3,0)" );
			Register( DbType.Currency, "NUMBER(19,1)");
			Register( DbType.Date, "DATE");
			Register( DbType.DateTime, "DATE" );
			Register( DbType.Decimal, "NUMBER(19,5)" ); 
			Register( DbType.Decimal, 19, "NUMBER(19, $1)");
			Register( DbType.Double, "DOUBLE PRECISION" ); 
			//Oracle does not have a guid datatype
			//Register( DbType.Guid, "UNIQUEIDENTIFIER" );
			Register( DbType.Int16, "NUMBER(5,0)" );
			Register( DbType.Int32, "NUMBER(10,0)" );
			Register( DbType.Int64, "NUMBER(20,0)" );
			Register( DbType.Single, "FLOAT" ); 
			Register( DbType.StringFixedLength, "NCHAR(255)");
			Register( DbType.StringFixedLength, 2000, "NCHAR($1)");
			Register( DbType.String, "NVARCHAR2(255)" );
			Register( DbType.String, 2000, "NVARCHAR2($1)" );
			Register( DbType.String, 1073741823, "NCLOB" );
			Register( DbType.Time, "DATE" );

			// add all the functions from the base into this instance
			foreach(DictionaryEntry de in base.AggregateFunctions) 
			{
				aggregateFunctions[de.Key] = de.Value;
			}
			aggregateFunctions["trunc"] = new QueryFunctionStandard();
			aggregateFunctions["round"] = new QueryFunctionStandard();
			aggregateFunctions["abs"] = new QueryFunctionStandard(NHibernate.Int32);
			aggregateFunctions["sign"] = new QueryFunctionStandard(NHibernate.Int32);
			aggregateFunctions["ceil"] = new QueryFunctionStandard(NHibernate.Int32);
			aggregateFunctions["floor"] = new QueryFunctionStandard(NHibernate.Int32);
			aggregateFunctions["sqrt"] = new QueryFunctionStandard();
			aggregateFunctions["exp"] = new QueryFunctionStandard();
			aggregateFunctions["ln"] = new QueryFunctionStandard();
			aggregateFunctions["sin"] = new QueryFunctionStandard();
			aggregateFunctions["sinh"] = new QueryFunctionStandard();
			aggregateFunctions["cos"] = new QueryFunctionStandard();
			aggregateFunctions["cosh"] = new QueryFunctionStandard();
			aggregateFunctions["tan"] = new QueryFunctionStandard();
			aggregateFunctions["tanh"] = new QueryFunctionStandard();
			aggregateFunctions["stddev"] = new QueryFunctionStandard();
			aggregateFunctions["variance"] = new QueryFunctionStandard();
			aggregateFunctions["sysdate"] = new SysdateQueryFunctionInfo();
			aggregateFunctions["lastday"] = new QueryFunctionStandard(NHibernate.Date);
		}
	
		public override string AddColumnString 
		{
			get { return "add"; }
		}

		public override string GetSequenceNextValString(string sequenceName) 
		{
			return "select " + sequenceName + ".nextval from dual";
		}
		
		public override string GetCreateSequenceString(string sequenceName) 
		{
			return "create sequence " + sequenceName + " INCREMENT BY 1 START WITH 1 MAXVALUE 1.0E28 MINVALUE 1 NOCYCLE CACHE 20 NOORDER";
		}
		
		public override string GetDropSequenceString(string sequenceName) 
		{
			return "drop sequence " + sequenceName;
		}
	
		public override string CascadeConstraintsString 
		{
			get { return " cascade constraints"; }
		}
	
		public override bool SupportsForUpdateNoWait 
		{
			get { return true; }
		}

		public override bool SupportsSequences 
		{
			get { return true; }
		}	
		
		public override bool SupportsLimit
		{
			get	{ return true;	}
		}

		public override string GetLimitString(String querySelect)
		{
			StringBuilder pagingSelect = new StringBuilder(100);
			pagingSelect.Append("select * from ( select row_.*, rownum rownum_ from ( ");
			pagingSelect.Append(querySelect);
			pagingSelect.Append(" ) row_ where rownum <= ?) where rownum_ > ?");
			return pagingSelect.ToString();
		}

		public override SqlString GetLimitString(SqlString querySqlString)
		{
			Parameter p1 = new Parameter();
			Parameter p2 = new Parameter();

			p1.Name = "p1";
			p1.SqlType = new Int16SqlType();

			p2.Name = "p2";
			p2.SqlType = new Int16SqlType();

			/*
			 * "select * from ( select row_.*, rownum rownum_ from ( "
			 * sql
			 * " ) row_ where rownum <= ?) where rownum_ > ?"
			 */ 
			SqlStringBuilder pagingBuilder = new SqlStringBuilder();
			pagingBuilder.Add("select * from ( select row_.*, rownum rownum_ from ( ");
			pagingBuilder.Add(querySqlString);
			pagingBuilder.Add(" ) row_ where rownum <= ");
			pagingBuilder.Add(p1);
			pagingBuilder.Add(") where rownum_ > ");
			pagingBuilder.Add(p2);

			return pagingBuilder.ToSqlString();
		}

		public override bool BindLimitParametersInReverseOrder
		{
			get	{ return true; }
		}

		public override bool SupportsForUpdateOf
		{
			get	{ return true; }
		}

		public override IDictionary AggregateFunctions
		{
			get	{ return aggregateFunctions; }
		}

		public override bool UseMaxForLimit
		{
			get	{ return true; }
		}

		[Obsolete("See the Dialect class for reason")]
		public override bool UseNamedParameters 
		{
			get { return true; }
		}

		[Obsolete("See the Dialect class for reason")]
		public override string NamedParametersPrefix 
		{
			get { return ":"; }
		}


		public class SysdateQueryFunctionInfo : IQueryFunctionInfo	
		{
			#region IQueryFunctionInfo Members

			public IType QueryFunctionType(IType columnType, IMapping mapping)
			{
				return NHibernate.Date;
			}

			public bool IsFunctionArgs
			{
				get { return false; }
			}

			public bool IsFunctionNoArgsUseParanthesis
			{
				get { return false; }
			}

			#endregion
		}
	}
}

