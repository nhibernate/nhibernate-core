using System;
using System.Data;
using System.Collections;

using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;


namespace NHibernate.Id 
{
	/// <summary>
	/// Generates <c>Int64</c> values using an oracle-style sequence. A higher performance
	/// algorithm is <see cref="SequenceHiLoGenerator"/>
	/// </summary>
	public class SequenceGenerator : IPersistentIdentifierGenerator, IConfigurable 
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(SequenceGenerator));

		/// <summary>
		/// The sequence parameter
		/// </summary>
		public const string Sequence = "sequence";
		public const string Schema = "schema";

		private string sequenceName;
		private System.Type returnClass;
		private string sql;

		public virtual void Configure(IType type, IDictionary parms, Dialect.Dialect dialect) 
		{
			this.sequenceName = PropertiesHelper.GetString(Sequence, parms, "hibernate_sequence");
			string schemaName = (string)parms[Schema]; 
			if ( schemaName!=null && sequenceName.IndexOf(StringHelper.Dot)<0 ) 
				sequenceName = schemaName + '.' + sequenceName; 
			returnClass = type.ReturnedClass; 
			sql = dialect.GetSequenceNextValString(sequenceName); 
		}

		public virtual object Generate(ISessionImplementor session, object obj) 
		{
			IDbCommand cmd = session.Batcher.PrepareCommand( new SqlString(sql) );
			IDataReader reader = null;
			try 
			{
				reader = session.Batcher.ExecuteReader( cmd ); 
				object result = null;
				reader.Read();
				result = IdentifierGeneratorFactory.Get(reader, returnClass);

				log.Debug("sequence ID generated: " + result);
				return result;
			} 
			// TODO: change to SQLException
			catch (Exception e) 
			{
				// TODO: add code to log the sql exception
				throw;
			} 
			finally 
			{
				session.Batcher.CloseCommand( cmd, reader);
			}
		}

		public string[] SqlCreateStrings(Dialect.Dialect dialect) 
		{
			return new string[] {
									dialect.GetCreateSequenceString(sequenceName)
								};
		}

		public string SqlDropString(Dialect.Dialect dialect) 
		{
			return dialect.GetDropSequenceString(sequenceName);
		}

		public object GeneratorKey() 
		{
			return sequenceName;
		}
	}
}
