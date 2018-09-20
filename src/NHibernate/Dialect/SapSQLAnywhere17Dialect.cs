using System.Data;
using System.Data.Common;
using NHibernate.Dialect.Schema;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Dialect
{
	/// <remarks>
	/// The SapSQLAnywhere17Dialect uses the SybaseSQLAnywhere12Dialect as its 
	/// base class. SybaseSQLAnywhere17Dialect includes support for ISO SQL standard
	/// sequences, which are defined in the catalog table <tt>SYSSEQUENCE</tt>. 
	/// The dialect uses the SybaseSQLAnywhe12MetaData class for metadata API
	/// calls, which correctly supports reserved words defined by SQL Anywhere.
	/// 
	/// The dialect defaults the following configuration properties:
	/// <list type="table">
	///	<listheader>
	///		<term>Property</term>
	///		<description>Default Value</description>
	///	</listheader>
	///	<item>
	///		<term>connection.driver_class</term>
	///		<description><see cref="NHibernate.Driver.SapSQLAnywhere17Driver" /></description>
	///	</item>
	///	<item>
	///		<term>prepare_sql</term>
	///		<description><see langword="false" /></description>
	///	</item>
	/// </list>
	/// </remarks>
	public class SapSQLAnywhere17Dialect : SybaseSQLAnywhere12Dialect
	{
		public SapSQLAnywhere17Dialect()
			: base()
		{
			DefaultProperties[Environment.ConnectionDriver] = "NHibernate.Driver.SybaseSQLAnywhere17Driver";
		}
	
		protected override void RegisterKeywords()
		{
			base.RegisterKeywords();
		}

		protected override void RegisterDateTimeTypeMappings()
		{
			base.RegisterDateTimeTypeMappings();
		}

		/// <summary> 
		/// SQL Anywhere supports <tt>SEQUENCES</tt> using a primarily SQL Standard 
		/// syntax. Sequence values can be queried using the <tt>.CURRVAL</tt> identifier, and the next
		/// value in a sequence can be retrieved using the <tt>.NEXTVAL</tt> identifier. Sequences
		/// are retained in the SYS.SYSSEQUENCE catalog table. 
		/// </summary>
		public override bool SupportsSequences
		{
			get { return true; }
		}

		/// <summary>
		/// Pooled sequences does not refer to the CACHE parameter of the <tt>CREATE SEQUENCE</tt>
		/// statement, but merely if the DBMS supports sequences that can be incremented or decremented
		/// by values greater than 1. 
		/// </summary>
		public override bool SupportsPooledSequences
		{
			get { return true; }
		}

		/// <summary>Get the <tt>SELECT</tt> command used to retrieve the names of all sequences.</summary>
		/// <returns>The <tt>SELECT</tt> command; or NULL if sequences are not supported.</returns>
		public override string QuerySequencesString
		{
			get { return "SELECT SEQUENCE_NAME FROM SYS.SYSSEQUENCE"; }
		}

		public override string GetSequenceNextValString(string sequenceName)
		{
			return "SELECT " + GetSelectSequenceNextValString(sequenceName) + " FROM SYS.DUMMY";
		}

		public override string GetSelectSequenceNextValString(string sequenceName)
		{
			return sequenceName + ".NEXTVAL";
		}

		public override string GetCreateSequenceString(string sequenceName)
		{
			return "CREATE SEQUENCE " + sequenceName; // by default, is START WITH 1 MAXVALUE 2**63-1
		}

		public override string GetDropSequenceString(string sequenceName)
		{
			return "DROP SEQUENCE " + sequenceName;
		}

		public override IDataBaseSchema GetDataBaseSchema(DbConnection connection)
		{
			return new SybaseAnywhereDataBaseMetaData(connection);
		}
	}
}
