using System;
using System.IO;
using System.Data;
using System.Text;
using System.Collections;
using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Dialect;
using NHibernate.Util;


namespace NHibernate.Tool.hbm2ddl 
{
	/// <summary>
	/// Commandline tool to export table schema for a configured <c>Configuration</c> to the database
	/// </summary>
	/// <remarks>
	/// To compile run "csc SchemaExport.cs /reference:../../bin/debug/NHibernate.dll"
	/// </remarks>
	public class SchemaExport 
	{
		private string[] dropSQL;
		private string[] createSQL;
		private IDictionary connectionProperties;
		private string outputFile = null;
		private Dialect.Dialect dialect;
		private string delimiter = null;

		/// <summary>
		/// Create a schema exported for a given Configuration
		/// </summary>
		public SchemaExport(Configuration cfg) : this(cfg, cfg.Properties) 
		{
		}

		/// <summary>
		/// Create a schema exporter for the given Configuration, with the given
		/// database connection properties
		/// </summary>
		/// <param name="cfg"></param>
		/// <param name="connectionProperties"></param>
		public SchemaExport(Configuration cfg, IDictionary connectionProperties) 
		{
			this.connectionProperties = connectionProperties;
			dialect = Dialect.Dialect.GetDialect(connectionProperties);
			dropSQL = cfg.GenerateDropSchemaScript(dialect);
			createSQL = cfg.GenerateSchemaCreationScript(dialect);
		}

		/// <summary>
		/// Set the output filename. The generated script will be written to this file
		/// </summary>
		public SchemaExport SetOutputFile(string filename) 
		{
			outputFile = filename;
			return this;
		}
		
		/// <summary>
		/// Set the end of statement delimiter 
		/// </summary>
		public SchemaExport SetDelimiter(string delimiter) 
		{
			this.delimiter = delimiter;
			return this;
		}
		/// <summary>
		/// Run the schema creation script
		/// </summary>
		public void Create(bool script, bool export) 
		{
			Execute(script, export, false, true);
		}

		/// <summary>
		/// Run the drop schema script
		/// </summary>
		public void Drop(bool script, bool export) 
		{
			Execute(script, export, true, true);
		}

		private void Execute(bool script, bool export, bool justDrop, bool format) 
		{
			IDbConnection connection  = null;
			IDbTransaction transaction = null;
			StreamWriter fileOutput = null;
			IConnectionProvider connectionProvider = null;
			IDbCommand statement = null;

			IDictionary props = new Hashtable();
			foreach(DictionaryEntry de in dialect.DefaultProperties) 
			{
				props.Add( de.Key, de.Value );
			}
			
			if (connectionProperties!=null) 
			{
				foreach(DictionaryEntry de in connectionProperties) 
				{
					props.Add( de.Key, de.Value );
				}
			}
	
			try 
			{
				if (outputFile!=null) 
				{
					fileOutput = new StreamWriter(outputFile);
				}

				if (export) 
				{
					connectionProvider = ConnectionProviderFactory.NewConnectionProvider(props);
					connection = connectionProvider.GetConnection();
					transaction = connection.BeginTransaction();
					statement = connection.CreateCommand();
				}

				for (int i=0; i<dropSQL.Length; i++) 
				{
					try 
					{
						string formatted = dropSQL[i];
						if (delimiter!=null) formatted += delimiter;
						if (script) Console.WriteLine(formatted);
						if (outputFile!=null) fileOutput.WriteLine( formatted );
						if (export) 
						{
							statement.CommandText = dropSQL[i];
							statement.CommandType = CommandType.Text;
							statement.Transaction = transaction;
							statement.ExecuteNonQuery();
						}
					} 
					catch(Exception e) 
					{
						if (!script) Console.WriteLine( dropSQL[i] );
						Console.WriteLine( "Unsuccessful: " + e.Message );
					}
				}

				if (!justDrop) 
				{
					for (int j=0; j<createSQL.Length; j++) 
					{
						try 
						{
							string formatted = createSQL[j];
							if (delimiter!=null) formatted += delimiter;
							if (script) Console.WriteLine(formatted);
							if (outputFile!=null) fileOutput.WriteLine( formatted );
							if (export) 
							{
								statement.CommandText = createSQL[j];
								statement.CommandType = CommandType.Text;
								statement.Transaction = transaction;
								statement.ExecuteNonQuery();
							}
						} 
						catch(Exception e) 
						{
							if (!script) Console.WriteLine( createSQL[j] );
							Console.WriteLine( "Unsuccessful: " + e.Message );
						}
					}
				}

				// if the parameter export==false then there will be no transaction
				// to commit.
				if (transaction!=null) transaction.Commit();
					
			} 
			catch (Exception e) 
			{
				if (transaction != null) 
				{
					try 
					{
						transaction.Rollback();
					}
					catch {}
				}
				Console.Write(e.StackTrace);
				throw new HibernateException( e.Message );
			} 
			finally 
			{
				try 
				{
					if (statement!=null) statement.Dispose();
					if (connection!=null) 
					{
						connectionProvider.CloseConnection(connection);
					}
				} 
				catch(Exception e) 
				{
					Console.Error.WriteLine( "Could not close connection: " + e.Message );
				}
				if (fileOutput!=null) 
				{
					try 
					{
						fileOutput.Close();
					} 
					catch (Exception ioe) 
					{
						Console.Error.WriteLine( "Error closing output file " + outputFile + ": " + ioe.Message );
					}
				}
			}
		}

		/// <summary>
		/// Format an SQL statement using simple rules:
		/// a) Insert newline after each comma;
		/// b) Indent three spaces after each inserted newline;
		/// If the statement contains single/double quotes return unchanged,
		/// it is too complex and could be broken by simple formatting.
		/// </summary>
		private static string Format(string sql) {

			if ( sql.IndexOf("\"") > 0 || sql.IndexOf("'") > 0) {
				return sql;
			}

			string formatted;

			if ( sql.ToLower().StartsWith("create table") ) {

				StringBuilder result = new StringBuilder(60);
				StringTokenizer tokens = new StringTokenizer( sql, "(,)", true);

				int depth = 0;

				foreach(string tok in tokens) {
					if ( StringHelper.ClosedParen.Equals(tok) ) {
						depth--;
						if (depth==0) result.Append("\n");
					}
					result.Append(tok);
					if ( StringHelper.Comma.Equals(tok) && depth==1 ) result.Append("\n  ");
					if ( StringHelper.OpenParen.Equals(tok) ) {
						depth++;
						if ( depth==1 ) result.Append("\n  ");
					}
				}

				formatted = result.ToString();
			} else {
				formatted = sql;
			}

			return formatted;
		}

		public static void Main(string[] args) {
			try {
				Configuration cfg = new Configuration();

				bool script = true;
				bool drop = false;
				bool export = true;
				string outputFile = null;
				string propFile = null;
				bool formatSQL = false;
				string delimiter = null;

				for ( int i=0; i<args.Length; i++ ) {
					if ( args[i].StartsWith("--") ) {
						if ( args[i].Equals("--quiet") ) {
							script = false;
						} else if ( args[i].Equals("--drop") ) {
							drop = true;
						} else if ( args[i].Equals("--text") ) {
							export = false;
						} else if ( args[i].Equals("--output=") ) {
							outputFile = args[i].Substring(13);
						} else if ( args[i].Equals("--format") ) {
							formatSQL = true;
						} else if ( args[i].Equals("--delimiter=") ) {
							delimiter = args[i].Substring(12);
						} else if ( args[i].Equals("--config=") ) {
							cfg.Configure( args[i].Substring(9) );
						}
					} else {
						string filename = args[i];
						if ( filename.EndsWith( ".dll") || filename.EndsWith( ".exe") ) {
							cfg.AddAssembly( filename );
						} else {
							cfg.AddXmlFile(filename);
						}
					}
				}
				if (propFile!=null) {
					//TODO: load up a props file (xml based probably..)
				} else {
					new SchemaExport(cfg)
						.SetOutputFile(outputFile)
						.SetDelimiter(delimiter)
						.Execute(script, export, drop, formatSQL);
				}
			} catch(Exception e) {
				Console.Error.WriteLine("Error creating schema " + e.Message );
				Console.Error.Write(e.StackTrace);
			}
		}
					
	}
}
