using System;
using System.Collections;
using System.Data;
using log4net;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Id
{
	/// <summary>
	/// An <see cref="IIdentifierGenerator" /> that generates <c>Int64</c> values using an 
	/// oracle-style sequence. A higher performance algorithm is 
	/// <see cref="SequenceHiLoGenerator"/>.
	/// </summary>
	/// <remarks>
	/// <p>
	///	This id generation strategy is specified in the mapping file as 
	///	<code>
	///	&lt;generator class="sequence"&gt;
	///		&lt;param name="sequence"&gt;uid_sequence&lt;/param&gt;
	///		&lt;param name="schema"&gt;db_schema&lt;/param&gt;
	///	&lt;/generator&gt;
	///	</code>
	/// </p>
	/// <p>
	/// The <c>sequence</c> parameter is required while the <c>schema</c> is optional.
	/// </p>
	/// </remarks>
	public class SequenceGenerator : IPersistentIdentifierGenerator, IConfigurable
	{
		private static readonly ILog log = LogManager.GetLogger( typeof( SequenceGenerator ) );

		/// <summary>
		/// The name of the sequence parameter.
		/// </summary>
		public const string Sequence = "sequence";

		/// <summary>
		/// The name of the schema parameter.
		/// </summary>
		public const string Schema = "schema";

		private string sequenceName;
		private System.Type returnClass;
		private string sql;

		#region IConfigurable Members

		/// <summary>
		/// Configures the SequenceGenerator by reading the value of <c>sequence</c> and
		/// <c>schema</c> from the <c>parms</c> parameter.
		/// </summary>
		/// <param name="type">The <see cref="IType"/> the identifier should be.</param>
		/// <param name="parms">An <see cref="IDictionary"/> of Param values that are keyed by parameter name.</param>
		/// <param name="dialect">The <see cref="Dialect.Dialect"/> to help with Configuration.</param>
		public virtual void Configure( IType type, IDictionary parms, Dialect.Dialect dialect )
		{
			this.sequenceName = PropertiesHelper.GetString( Sequence, parms, "hibernate_sequence" );
			string schemaName = ( string ) parms[ Schema ];
			if( schemaName != null && sequenceName.IndexOf( StringHelper.Dot ) < 0 )
			{
				sequenceName = schemaName + '.' + sequenceName;
			}
			returnClass = type.ReturnedClass;
			sql = dialect.GetSequenceNextValString( sequenceName );
		}

		#endregion

		#region IIdentifierGenerator Members

		/// <summary>
		/// Generate an <see cref="Int16"/>, <see cref="Int32"/>, or <see cref="Int64"/> 
		/// for the identifier by using a database sequence.
		/// </summary>
		/// <param name="session">The <see cref="ISessionImplementor"/> this id is being generated in.</param>
		/// <param name="obj">The entity for which the id is being generated.</param>
		/// <returns>The new identifier as a <see cref="Int16"/>, <see cref="Int32"/>, or <see cref="Int64"/>.</returns>
		public virtual object Generate( ISessionImplementor session, object obj )
		{
			IDbCommand cmd = session.Batcher.PrepareCommand( new SqlString( sql ) );
			IDataReader reader = null;
			try
			{
				reader = session.Batcher.ExecuteReader( cmd );
				object result = null;
				reader.Read();
				result = IdentifierGeneratorFactory.Get( reader, returnClass );

				log.Debug( "sequence ID generated: " + result );
				return result;
			} 
				// TODO: change to SQLException
			catch( Exception e )
			{
				// TODO: add code to log the sql exception
				log.Error( "error generating sequence", e );
				throw;
			}
			finally
			{
				session.Batcher.CloseCommand( cmd, reader );
			}
		}

		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dialect"></param>
		/// <returns></returns>
		public string[ ] SqlCreateStrings( Dialect.Dialect dialect )
		{
			return new string[ ]
				{
					dialect.GetCreateSequenceString( sequenceName )
				};
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dialect"></param>
		/// <returns></returns>
		public string SqlDropString( Dialect.Dialect dialect )
		{
			return dialect.GetDropSequenceString( sequenceName );
		}

		/// <summary></summary>
		public object GeneratorKey()
		{
			return sequenceName;
		}
	}
}