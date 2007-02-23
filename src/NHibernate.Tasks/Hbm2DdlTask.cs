using System;
using System.Collections;
using System.Reflection;

using NAnt.Core;
using NAnt.Core.Attributes;
using NAnt.Core.Types;

using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace NHibernate.Tasks
{
	/// <summary>
	/// NAnt task for hbm2ddl
	/// </summary>
	/// <remarks>
	/// <p>
	/// Usage example:
	/// <code>
	/// &lt;hbm2ddl
	/// 	connectionprovider="NHibernate.Connection.DriverConnectionProvider"
	/// 	dialect="NHibernate.Dialect.MsSql2000Dialect"
	/// 	connectiondriverclass="NHibernate.Driver.SqlClientDriver"
	/// 	connectionstring="server=(local);uid=sa;pwd=sa;database=MyProject"
	/// 	delimiter=" GO "
	/// 	outputtoconsole="false"
	/// 	exportonly="true"
	/// 	formatnice="true"
	/// 	outputfilename="${nant.project.basedir}/sql/schema.sql"&gt;
	/// 	&lt;assemblies&gt;
	/// 		&lt;include name="${nant.project.basedir}/bin/MyProject.dll" /&gt;
	/// 	&lt;/assemblies&gt;
	/// &lt;/hbm2ddl&gt;
	/// </code>
	/// </p>
	/// <p>
	/// Contributed by James Geurts
	/// </p>
	/// </remarks>
	[TaskName("hbm2ddl")]
	public class Hbm2DdlTask : Task
	{
		private string _connectionProvider = "NHibernate.Connection.DriverConnectionProvider";
		private string _dialect = "NHibernate.Dialect.MsSql2000Dialect";
		private string _connectionDriverClass = "NHibernate.Driver.SqlClientDriver";
		private string _connectionString;
		private string _delimiter = " GO ";
		private bool _outputToConsole;
		private bool _exportOnly;
		private bool _dropOnly;
		private bool _formatNice;
		private FileSet _assemblies = new FileSet();
		private string _outputFilename;

		/// <summary>
		/// Gets or sets the connection provider.  NHibernate.Connection.DriverConnectionProvider is the default
		/// </summary>
		/// <value>The connection provider.</value>
		[TaskAttribute("connectionprovider")]
		public string ConnectionProvider
		{
			get { return this._connectionProvider; }
			set { this._connectionProvider = value; }
		}

		/// <summary>
		/// Gets or sets the dialect.  NHibernate.Dialect.MsSql2000Dialect is the default
		/// </summary>
		[TaskAttribute("dialect")]
		public string Dialect
		{
			get { return this._dialect; }
			set { this._dialect = value; }
		}

		/// <summary>
		/// Gets or sets the connection driver class. NHibernate.Driver.SqlClientDriver is the default.
		/// </summary>
		/// <value>The connection driver class.</value>
		[TaskAttribute("connectiondriverclass")]
		public string ConnectionDriverClass
		{
			get { return this._connectionDriverClass; }
			set { this._connectionDriverClass = value; }
		}

		/// <summary>
		/// Gets or sets the connection string used to access the ddl.
		/// </summary>
		/// <value>The connection string.</value>
		[TaskAttribute("connectionstring", Required=true)]
		public string ConnectionString
		{
			get { return this._connectionString; }
			set { this._connectionString = value; }
		}

		/// <summary>
		/// Gets or sets the delimiter.  GO is the default.
		/// </summary>
		[TaskAttribute("delimiter")]
		public string Delimiter
		{
			get { return this._delimiter; }
			set { this._delimiter = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the schema should be outputted to the console
		/// </summary>
		/// <value><see langword="true" /> to output to the console; otherwise, <see langword="false" />.</value>
		[TaskAttribute("outputtoconsole")]
		public bool OutputToConsole
		{
			get { return this._outputToConsole; }
			set { this._outputToConsole = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the schema ddl script should only be exported
		/// or if it should be executed on the database server.
		/// </summary>
		/// <value><see langword="true" /> if only output the script; otherwise, <see langword="false" /> - Execute the script on the db server.</value>
		[TaskAttribute("exportonly")]
		public bool ExportOnly
		{
			get { return this._exportOnly; }
			set { this._exportOnly = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether only the drop script should be executed
		/// </summary>
		/// <value><see langword="true" /> if only drop objects; otherwise, <see langword="false" /> - Drop and Create objects.</value>
		[TaskAttribute("droponly")]
		public bool DropOnly
		{
			get { return this._dropOnly; }
			set { this._dropOnly = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the ddl script should be formatted nicely
		/// </summary>
		/// <value><see langword="true" /> for nice format; otherwise, <see langword="false" /> - One statement per line.</value>
		[TaskAttribute("formatnice")]
		public bool FormatNice
		{
			get { return this._formatNice; }
			set { this._formatNice = value; }
		}

		/// <summary>
		/// Gets or sets the filename to write the ddl schema script to.
		/// </summary>
		/// <value>The output filename.</value>
		[TaskAttribute("outputfilename")]
		public string OutputFilename
		{
			get { return this._outputFilename; }
			set { this._outputFilename = value; }
		}

		/// <summary>
		/// Gets or sets the assemblies load with embedded *.hbm.xml resources.
		/// </summary>
		/// <value>The assemblies.</value>
		[BuildElement("assemblies", Required=true)]
		public FileSet Assemblies
		{
			get { return _assemblies; }
			set { _assemblies = value; }
		}

		/// <summary>
		/// Executes the task.
		/// </summary>
		protected override void ExecuteTask()
		{
			Configuration config = new Configuration();
			IDictionary properties = new Hashtable();

			properties["hibernate.connection.provider"] = ConnectionProvider;
			properties["hibernate.dialect"] = Dialect;
			properties["hibernate.connection.driver_class"] = ConnectionDriverClass;
			properties["hibernate.connection.connection_string"] = ConnectionString;

			config.AddProperties(properties);

			foreach (string filename in Assemblies.FileNames)
			{
				Log(Level.Info, "Adding assembly file {0}", filename);
				try
				{
					Assembly asm = Assembly.LoadFile(filename);
					config.AddAssembly(asm);
				}
				catch (Exception e)
				{
					Log(Level.Error, "Error loading assembly {0}: {1}", filename, e);
				}
			}

			SchemaExport se = new SchemaExport(config);
			if (!IsStringNullOrEmpty(OutputFilename))
			{
				se.SetOutputFile(OutputFilename);
			}
			se.SetDelimiter(Delimiter);
			Log(Level.Debug, "Exporting ddl schema.");
			se.Execute(OutputToConsole, ExportOnly, DropOnly, FormatNice);

			if (!IsStringNullOrEmpty(OutputFilename))
			{
				Log(Level.Info, "Successful DDL schema output: {0}", OutputFilename);
			}
		}

		private static bool IsStringNullOrEmpty(string input)
		{
			return (input == null || input.Length == 0);
		}
	}
}