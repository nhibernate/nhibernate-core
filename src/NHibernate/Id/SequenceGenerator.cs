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
			IDbCommand st = session.Batcher.PrepareCommand( new SqlString(sql) );
			try 
			{
				IDataReader rs = st.ExecuteReader();
				object result = null;
				try 
				{
					rs.Read();
					result = IdentifierGeneratorFactory.Get(rs, returnClass);
				} 
				finally 
				{
					rs.Close();
				}

				log.Debug("sequence ID generated: " + result);
				return result;
			} 
			catch (Exception e) 
			{
				throw e;
			} 
			finally 
			{
				session.Batcher.CloseCommand(st);
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
