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
			/* TODO:
			getDefaultProperties().setProperty(Environment.USE_STREAMS_FOR_BINARY, "true");
			getDefaultProperties().setProperty(Environment.STATEMENT_BATCH_SIZE, DEFAULT_BATCH_SIZE);
			getDefaultProperties().setProperty(Environment.OUTER_JOIN, "true");
			*/

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
			p1.DbType = DbType.Int16;

			p2.Name = "p2";
			p2.DbType = DbType.Int16;

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


        
		private string SqlTypeToString(string name, int length) 
		{
			return name + "(" + length + ")";
		}

		private string SqlTypeToString(string name, int precision, int scale) 
		{
			if (precision > 19) precision = 19;
			return name + "(" + precision + ", " + scale + ")";
		}

		protected override string SqlTypeToString(AnsiStringFixedLengthSqlType sqlType) 
		{
			return SqlTypeToString("NVARCHAR2", 1000);
		}

		protected override string SqlTypeToString(BinarySqlType sqlType) 
		{			
			if(sqlType.Length <= 8000) 
			{
				return SqlTypeToString("RAW", sqlType.Length);
			}
			else 
			{
				return "BLOB"; // should use the IType.BlobType
			}					
		}
		
		protected override string SqlTypeToString(BooleanSqlType sqlType)
		{
			return "NUMBER(1,0)";
		}

		
		protected override string SqlTypeToString(ByteSqlType sqlType)
		{
			return "NUMBER(3,0)";
		}

		protected override string SqlTypeToString(CurrencySqlType sqlType)
		{
			return "NUMBER(19, 1)";
		}

		protected override string SqlTypeToString(DateSqlType sqlType)
		{
			return "DATE";
		}

		protected override string SqlTypeToString(DateTimeSqlType sqlType)
		{
			return "DATE";
		}

		protected override string SqlTypeToString(DecimalSqlType sqlType)
		{
			return SqlTypeToString("NUMBER", sqlType.Precision, sqlType.Scale);
		}

		protected override string SqlTypeToString(DoubleSqlType sqlType)
		{
			return "DOUBLE PRECISION";
		}

		protected override string SqlTypeToString(Int16SqlType sqlType)
		{
			return "NUMBER(5,0)";
		}

		protected override string SqlTypeToString(Int32SqlType sqlType)
		{
			return "NUMBER(10,0)";
		}

		protected override string SqlTypeToString(Int64SqlType sqlType)
		{
			return "NUMBER(20,0)";
		}

		protected override string SqlTypeToString(SingleSqlType sqlType)
		{
			return "FLOAT";
		}

		protected override string SqlTypeToString(StringFixedLengthSqlType sqlType) 
		{			
			if(sqlType.Length <= 2000) 
			{
				return SqlTypeToString("NVARCHAR2", sqlType.Length);
			}
			else 
			{
				return string.Empty; // should use the IType.ClobType
			}					
		}

		protected override string SqlTypeToString(StringSqlType sqlType) 
		{
			if(sqlType.Length <= 2000) 
			{
				return SqlTypeToString("NVARCHAR2", sqlType.Length);
			}
			else 
			{
				return string.Empty; // should use the IType.ClobType
			}					
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

