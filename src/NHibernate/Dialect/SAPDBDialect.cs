using System;
using System.Data;
using System.Text;

using NHibernate.Sql;
using NHibernate.Util;

namespace NHibernate.Dialect {

	/// <summary>
	/// An SQL dialect compatible with SAP DB.
	/// </summary>
	public class SAPDBDialect {

		public SAPDBDialect() : base() {

			/* Java mapping was:
			
			Types.BIT, "BOOLEAN" );
			Types.BIGINT, "FIXED(19,0)" );
			Types.SMALLINT, "SMALLINT" );
			Types.TINYINT, "FIXED(3,0)" );
			Types.INTEGER, "INT" );
			Types.CHAR, "CHAR(1)" );
			Types.VARCHAR, "VARCHAR($l)" );
			Types.FLOAT, "FLOAT" );
			Types.DOUBLE, "DOUBLE PRECISION"
			Types.DATE, "DATE" );
			Types.TIME, "TIME" );
			Types.TIMESTAMP, "TIMESTAMP" );
			Types.VARBINARY, "LONG BYTE" );
			Types.NUMERIC, "FIXED(19,$l)" );
			Types.CLOB, "LONG VARCHAR" );
			Types.BLOB, "LONG BYTE" );
			*/
			
			/*
			getDefaultProperties().setProperty(Environment.OUTER_JOIN, "true");
			getDefaultProperties().setProperty(Environment.STATEMENT_BATCH_SIZE, DEFAULT_BATCH_SIZE);
			*/
		}

		public bool HasAlterTable() {
			return true;
		}
	
		public bool SupportsForUpdate() {
			return false;
		}
	
		public bool DropConstraints() {
			return false;
		}
	
		public string GetAddColumnString() {
			return "add";
		}
	
		public string GetAddForeignKeyConstraintString(string constraintName, string[] foreignKey, string referencedTable, string[] primaryKey) {
			return new StringBuilder(30)
				.Append(" foreign key ")
				.Append(constraintName)
				.Append(" (")
				.Append( StringHelper.Join(StringHelper.CommaSpace, foreignKey) )
				.Append(") references ")
				.Append(referencedTable)
				.ToString();
		}
	
		public string GetAddPrimaryKeyConstraintString(string constraintName) {
			return " primary key ";
		}
	
		public string GetNullColumnString() {
			return " null";
		}
	
		public string GetSequenceNextValString(string sequenceName) {
			return  string.Concat( "select ", sequenceName, ".nextval from dual");
		}
	
		public string GetCreateSequenceString(string sequenceName) {
			return "create sequence " + sequenceName;
		}
	
		public string GetDropSequenceString(string sequenceName) {
			return "drop sequence " + sequenceName;
		}
	
		public string GetQuerySequencesString() {
			return "select SEQUENCE_NAME from DOMAIN.SEQUENCES";
		}
	
		public OuterJoinFragment CreateOuterJoinFragment() {
			return new OracleOuterJoinFragment();
		}
		
		public bool SupportsSequences() {
			return true;
		}

		public CaseFragment CreateCaseFragment() {
			return new DecodeCaseFragment();
		}
	}
}