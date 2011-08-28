using System.Data;
using System.Data.Common;
using NHibernate.Dialect.Schema;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Dialect
{
	/// <summary>
	/// SQL Dialect for SQL Anywhere 12 - for the NHibernate 3.2.0 distribution
	/// Copyright (C) 2011 Glenn Paulley
	/// Contact: http://iablog.sybase.com/paulley
	///
	/// This NHibernate dialect for SQL Anywhere 12 is a contribution to the NHibernate
	/// open-source project. It is intended to be included in the NHibernate 
	/// distribution and is licensed under LGPL.
	///
	/// This library is free software; you can redistribute it and/or
	/// modify it under the terms of the GNU Lesser General Public
	/// License as published by the Free Software Foundation; either
	/// version 2.1 of the License, or (at your option) any later version.
	///
	/// This library is distributed in the hope that it will be useful,
	/// but WITHOUT ANY WARRANTY; without even the implied warranty of
	/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
	/// Lesser General Public License for more details.
	///
	/// You should have received a copy of the GNU Lesser General Public
	/// License along with this library; if not, write to the Free Software
	/// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
	/// </summary>
	/// <remarks>
	/// The SybaseSQLAnywhere12Dialect uses the SybaseSQLAnywhere11Dialect as its 
	/// base class. SybaseSQLAnywhere12Dialect includes support for ISO SQL standard
	/// sequences, which are defined in the catalog table <tt>SYSSEQUENCE</tt>. 
	/// The dialect uses the SybaseSQLAnywhe11MetaData class for metadata API
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
	///		<description><see cref="NHibernate.Driver.SybaseSQLAnywhereDotNet4Driver" /></description>
	///	</item>
	///	<item>
	///		<term>prepare_sql</term>
	///		<description><see langword="false" /></description>
	///	</item>
	/// </list>
	/// </remarks>
	public class SybaseSQLAnywhere12Dialect : SybaseSQLAnywhere11Dialect
	{
		public SybaseSQLAnywhere12Dialect()
			: base()
		{
			DefaultProperties[Environment.ConnectionDriver] = "NHibernate.Driver.SybaseSQLAnywhereDotNet4Driver";
			RegisterDateTimeTypeMappings();
			RegisterKeywords();
		}

		new protected void RegisterKeywords()
		{
			RegisterKeyword("NEAR");
			RegisterKeyword("LIMIT");
			RegisterKeyword("OFFSET");
			RegisterKeyword("DATETIMEOFFSET");
		}

		new void RegisterDateTimeTypeMappings()
		{
			RegisterColumnType(DbType.DateTimeOffset, "DATETIMEOFFSET");
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