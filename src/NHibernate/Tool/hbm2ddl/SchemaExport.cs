using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Text;
using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Util;

namespace NHibernate.Tool.hbm2ddl
{
	/// <summary>
	/// Generates ddl to export table schema for a configured <c>Configuration</c> to the database
	/// </summary>
	/// <remarks>
	/// This Class can be used directly or the command line wrapper NHibernate.Tool.hbm2ddl.exe can be
	/// used when a dll can not be directly used.
	/// </remarks>
	public class SchemaExport
	{
		private string[ ] dropSQL;
		private string[ ] createSQL;
		private IDictionary connectionProperties;
		private string outputFile = null;
		private Dialect.Dialect dialect;
		private string delimiter = null;

		/// <summary>
		/// Create a schema exported for a given Configuration
		/// </summary>
		/// <param name="cfg">The NHibernate Configuration to generate the schema from.</param>
		public SchemaExport( Configuration cfg ) : this( cfg, cfg.Properties )
		{
		}

		/// <summary>
		/// Create a schema exporter for the given Configuration, with the given
		/// database connection properties
		/// </summary>
		/// <param name="cfg">The NHibernate Configuration to generate the schema from.</param>
		/// <param name="connectionProperties">The Properties to use when connecting to the Database.</param>
		public SchemaExport( Configuration cfg, IDictionary connectionProperties )
		{
			this.connectionProperties = connectionProperties;
			dialect = Dialect.Dialect.GetDialect( connectionProperties );
			dropSQL = cfg.GenerateDropSchemaScript( dialect );
			createSQL = cfg.GenerateSchemaCreationScript( dialect );
		}

		/// <summary>
		/// Set the output filename. The generated script will be written to this file
		/// </summary>
		/// <param name="filename">The name of the file to output the ddl to.</param>
		/// <returns>The SchemaExport object.</returns>
		public SchemaExport SetOutputFile( string filename )
		{
			outputFile = filename;
			return this;
		}

		/// <summary>
		/// Set the end of statement delimiter 
		/// </summary>
		/// <param name="delimiter">The end of statement delimiter.</param>
		/// <returns>The SchemaExport object.</returns>
		public SchemaExport SetDelimiter( string delimiter )
		{
			this.delimiter = delimiter;
			return this;
		}

		/// <summary>
		/// Run the schema creation script
		/// </summary>
		/// <param name="script"><c>true</c> if the ddl should be outputted in the Console.</param>
		/// <param name="export"><c>true</c> if the ddl should be executed against the Database.</param>
		/// <remarks>
		/// This is a convenience method that calls <see cref="Execute(bool, bool, bool, bool)"/> and sets
		/// the justDrop parameter to false and the format parameter to true.
		/// </remarks>
		public void Create( bool script, bool export )
		{
			Execute( script, export, false, true );
		}

		/// <summary>
		/// Run the drop schema script
		/// </summary>
		/// <param name="script"><c>true</c> if the ddl should be outputted in the Console.</param>
		/// <param name="export"><c>true</c> if the ddl should be executed against the Database.</param>
		/// <remarks>
		/// This is a convenience method that calls <see cref="Execute(bool, bool, bool, bool)"/> and sets
		/// the justDrop and format parameter to true.
		/// </remarks>
		public void Drop( bool script, bool export )
		{
			Execute( script, export, true, true );
		}

		/// <summary>
		/// Executes the Export of the Schema.
		/// </summary>
		/// <param name="script"><c>true</c> if the ddl should be outputted in the Console.</param>
		/// <param name="export"><c>true</c> if the ddl should be executed against the Database.</param>
		/// <param name="justDrop"><c>true</c> if only the ddl to drop the Database objects should be executed.</param>
		/// <param name="format"><c>true</c> if the ddl should be nicely formatted instead of one statement per line.</param>
		/// <remarks>
		/// This method allows for both the drop and create ddl script to be executed.
		/// </remarks>
		public void Execute( bool script, bool export, bool justDrop, bool format )
		{
			IDbConnection connection = null;
			StreamWriter fileOutput = null;
			IConnectionProvider connectionProvider = null;
			IDbCommand statement = null;

			IDictionary props = new Hashtable();
			foreach( DictionaryEntry de in dialect.DefaultProperties )
			{
				props[ de.Key ] = de.Value;
			}

			if( connectionProperties != null )
			{
				foreach( DictionaryEntry de in connectionProperties )
				{
					props[ de.Key ] = de.Value;
				}
			}

			try
			{
				if( outputFile != null )
				{
					fileOutput = new StreamWriter( outputFile );
				}

				if( export )
				{
					connectionProvider = ConnectionProviderFactory.NewConnectionProvider( props );
					connection = connectionProvider.GetConnection();
					statement = connection.CreateCommand();
				}

				for( int i = 0; i < dropSQL.Length; i++ )
				{
					try
					{
						string formatted;
						if( format )
						{
							formatted = Format( dropSQL[ i ] );
						}
						else
						{
							formatted = dropSQL[ i ];
						}

						if( delimiter != null )
						{
							formatted += delimiter;
						}
						if( script )
						{
							Console.WriteLine( formatted );
						}
						if( outputFile != null )
						{
							fileOutput.WriteLine( formatted );
						}
						if( export )
						{
							statement.CommandText = dropSQL[ i ];
							statement.CommandType = CommandType.Text;
							statement.ExecuteNonQuery();
						}
					}
					catch( Exception e )
					{
						if( !script )
						{
							Console.WriteLine( dropSQL[ i ] );
						}
						Console.WriteLine( "Unsuccessful: " + e.Message );
					}
				}

				if( !justDrop )
				{
					for( int j = 0; j < createSQL.Length; j++ )
					{
						try
						{
							string formatted;
							if( format )
							{
								formatted = Format( createSQL[ j ] );
							}
							else
							{
								formatted = createSQL[ j ];
							}
							if( delimiter != null )
							{
								formatted += delimiter;
							}
							if( script )
							{
								Console.WriteLine( formatted );
							}
							if( outputFile != null )
							{
								fileOutput.WriteLine( formatted );
							}
							if( export )
							{
								statement.CommandText = createSQL[ j ];
								statement.CommandType = CommandType.Text;
								statement.ExecuteNonQuery();
							}
						}
						catch( Exception e )
						{
							if( !script )
							{
								Console.WriteLine( createSQL[ j ] );
							}
							Console.WriteLine( "Unsuccessful: " + e.Message );
						}
					}
				}

			}
			catch( Exception e )
			{
				Console.Write( e.StackTrace );
				throw new HibernateException( e.Message, e );
			}
			finally
			{
				try
				{
					if( statement != null )
					{
						statement.Dispose();
					}
					if( connection != null )
					{
						connectionProvider.CloseConnection( connection );
						connectionProvider.Close();
					}
				}
				catch( Exception e )
				{
					Console.Error.WriteLine( "Could not close connection: " + e.Message );
				}
				if( fileOutput != null )
				{
					try
					{
						fileOutput.Close();
					}
					catch( Exception ioe )
					{
						Console.Error.WriteLine( "Error closing output file " + outputFile + ": " + ioe.Message );
					}
				}
			}
		}

		/// <summary>
		/// Format an SQL statement using simple rules
		/// </summary>
		/// <param name="sql">The string containing the sql to format.</param>
		/// <returns>A string that contains formatted sql.</returns>
		/// <remarks>
		/// The simple rules to used when formatting are:
		/// <list type="number">
		///		<item>
		///			<description>Insert a newline after each comma</description>
		///		</item>
		///		<item>
		///			<description>Indent three spaces after each inserted newline</description>
		///		</item>
		///		<item>
		///			<description>
		///			If the statement contains single/double quotes return unchanged because
		///			it is too complex and could be broken by simple formatting.
		///			</description>
		///		</item>
		/// </list>
		/// </remarks>
		private static string Format( string sql )
		{
			if( sql.IndexOf( "\"" ) > 0 || sql.IndexOf( "'" ) > 0 )
			{
				return sql;
			}

			string formatted;

			if( sql.ToLower().StartsWith( "create table" ) )
			{
				StringBuilder result = new StringBuilder( 60 );
				StringTokenizer tokens = new StringTokenizer( sql, "(,)", true );

				int depth = 0;

				foreach( string tok in tokens )
				{
					if( StringHelper.ClosedParen.Equals( tok ) )
					{
						depth--;
						if( depth == 0 )
						{
							result.Append( "\n" );
						}
					}
					result.Append( tok );
					if( StringHelper.Comma.Equals( tok ) && depth == 1 )
					{
						result.Append( "\n  " );
					}
					if( StringHelper.OpenParen.Equals( tok ) )
					{
						depth++;
						if( depth == 1 )
						{
							result.Append( "\n  " );
						}
					}
				}

				formatted = result.ToString();
			}
			else
			{
				formatted = sql;
			}

			return formatted;
		}
	}
}