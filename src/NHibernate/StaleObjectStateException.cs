using System;
using log4net;

namespace NHibernate
{
	/// <summary>
	/// Thrown when a version number check failed, indicating that the 
	/// <c>ISession</c> contained stale data (when using long transactions with
	/// versioning).
	/// </summary>
	[Serializable]
	public class StaleObjectStateException : HibernateException
	{
		private static readonly ILog log = LogManager.GetLogger( typeof( StaleObjectStateException ) );
		private System.Type persistentType;
		private object identifier;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="persistentType"></param>
		/// <param name="identifier"></param>
		public StaleObjectStateException( System.Type persistentType, object identifier ) : base( "Row was updated or deleted by another transaction" )
		{
			this.persistentType = persistentType;
			this.identifier = identifier;
			LogManager.GetLogger( typeof( StaleObjectStateException ) ).Error( "An operation failed due to stale data", this );
		}

		/// <summary></summary>
		public System.Type PersistentType
		{
			get { return persistentType; }
		}

		/// <summary></summary>
		public object Identifier
		{
			get { return identifier; }
		}

		/// <summary></summary>
		public override string Message
		{
			get { return base.Message + " for " + persistentType.FullName + " instance with identifier: " + identifier; }
		}
	}
}